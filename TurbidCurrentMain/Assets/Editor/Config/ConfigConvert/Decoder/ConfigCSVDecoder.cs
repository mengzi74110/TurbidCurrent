using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConfigCSVDecoder : ConfigFile.IDecoder
{
    static string[] ReadValue(string line)
    {
        if (string.IsNullOrEmpty(line))
            throw new ArgumentException("line");

        line = line + ',';

        var words = new List<string>();
        int quoteCount = 0;
        string word = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                quoteCount++;
                continue;
            }

            if (c == ',' && quoteCount % 2 == 0)
            {
                if (i != 0)
                {
                    words.Add(word);
                    word = "";
                }
                continue;
            }

            word += c;
        }

        return words.ToArray();
    }

    public void DecodeFile(string folder, string fileName, out string[] fieldNames, out string[] types, out string[] annotations, out string[][] valueLines)
    {
        string fullPath = Path.Combine(folder, fileName);

        if (!File.Exists(fullPath))
            throw new InvalidOperationException("File not exist, path: " + fullPath);

        string[] lines = File.ReadAllLines(fullPath, Encoding.Default);
        if (lines.Length < 3)
            throw new InvalidOperationException("Lines length < 3, path: " + fullPath);

        if (!lines[0].StartsWith("$"))
            throw new InvalidOperationException("Line 1 is not start with $, path: " + fullPath);
        if (!lines[1].StartsWith("!"))
            throw new InvalidOperationException("Line 2 is not start with !, path: " + fullPath);
        if (!lines[2].StartsWith("#"))
            throw new InvalidOperationException("Line 3 is not start with #, path: " + fullPath);

        types = lines[0].Split(new char[] { ',', '$', '!', '#' }, StringSplitOptions.RemoveEmptyEntries);
        fieldNames = lines[1].Split(new char[] { ',', '$', '!', '#' }, StringSplitOptions.RemoveEmptyEntries);
        annotations = lines[2].Split(new char[] { ',', '$', '!', '#' }, StringSplitOptions.RemoveEmptyEntries);

        if (types.Length != fieldNames.Length || types.Length != annotations.Length)
            throw new InvalidOperationException("Length of types,names,annotations are not the same, path: " + fullPath);

        var values = new List<string[]>();
        for (int i = 3; i < lines.Length; i++)
        {
            string line = lines[i];
            if (!string.IsNullOrEmpty(line))
            {
                var v = ReadValue(line);
                if (v.Length != types.Length)
                {
                    string error = string.Format("Length of types,value are not the same, file: {0}, line: {1}, type length: {2}, value length: {3}, line: {4}",
                        fullPath, i + 1, types.Length, v.Length, line);
                    throw new InvalidOperationException(error);
                }
                values.Add(v);
            }
        }

        valueLines = values.ToArray();
    }
}

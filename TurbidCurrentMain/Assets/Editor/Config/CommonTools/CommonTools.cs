using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;

public class CommonTools : MonoBehaviour
{
    [MenuItem("CustomToolbar/Common/UTF-8/Select Text Convert UTF-8")]
    static void ConvertSelectTextUTF8()
    {
        List<string> paths = new List<string>();
        List<string> directories = new List<string>();

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            var obj = Selection.objects[i];
            string path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
                paths.Add(path);
            else
                directories.Add(path);
        }

        if (directories.Count > 0)
        {
            string[] guids = AssetDatabase.FindAssets("", directories.ToArray());
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                paths.Add(path);
            }
        }

        for (int i = 0; i < paths.Count; i++)
        {
            string path = paths[i];
            ConvertToUTF8(paths[i], System.Text.Encoding.GetEncoding("GB2312"));
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        UnityEngine.Debug.Log($"Scripts Files Total Number:{paths.Count},Scripts Convert UTF-8 is Done");
    }

    [MenuItem("CustomToolbar/Common/UTF-8/All Scripts Convert UTF-8")]
    static void ConvertAllScriptsToUTF8()
    {
        var encoding = System.Text.Encoding.GetEncoding("GB2312"); //VSĬ�ϵĴ�ģʽ����GB2312-80
        string scriptsPath = AllPathConfig.ScriptsPath;//��Ҫת����·����

        if (Directory.Exists(scriptsPath))
        {
            DirectoryInfo info = new DirectoryInfo(scriptsPath);
            FileInfo[] files = info.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".cs"))
                {
                    //string content = File.ReadAllText(files[i].FullName, encoding);
                    //File.WriteAllText(files[i].FullName, content, Encoding.UTF8);
                    ConvertToUTF8(files[i].FullName, encoding);
                }
            }
            UnityEngine.Debug.Log($"Scripts Files Total Number:{files.Length},Scripts Convert UTF-8 is Done");
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    static void ConvertToUTF8(string filePath, Encoding encoding)
    {
        string content;
        try
        {
            if (File.Exists(filePath))
            {
                content = File.ReadAllText(filePath, encoding);
                File.WriteAllText(filePath, content, Encoding.UTF8);
            }
        }
        catch (Exception ex)
        {
            string msg = string.Format("convert to utf8 failed, path: {0}, messenge: \n{1}", filePath, ex.Message);
            Debug.LogError(msg);
            return;
        }
    }
}

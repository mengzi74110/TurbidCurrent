using System.Collections.Generic;
using UnityEngine;
using System.IO;
using OfficeOpenXml;

public class ConfigExcelDecoder : ConfigFile.IDecoder
{
    public void DecodeFile(string folder, string fileName, out string[] fieldNames, out string[] types, out string[] annotations, out string[][] valueLines)
    {
        string path = Path.Combine(folder, fileName);
        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (ExcelPackage excel = new ExcelPackage(fileStream))
            {
                ExcelWorksheet sheet = excel.Workbook.Worksheets[1];

                List<string> listAnnotations = new List<string>();
                for (int c = 1; c <= sheet.Dimension.End.Column; c++)
                {
                    ExcelRange excelRange = sheet.Cells[1, c];
                    string value = (excelRange.Value ?? "").ToString().Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        ExcelComment comment = excelRange.Comment;
                        string commentString = comment != null ? string.Format("({0})", comment.Text.Replace("\n", " ").Replace("\r", " ")) : "";
                        string stringAnnotaion = string.Format("{0}{1}", value, commentString);
                        listAnnotations.Add(stringAnnotaion);
                    }
                    else
                        break;
                }
                int maxColum = listAnnotations.Count;

                annotations = listAnnotations.ToArray();
                fieldNames = new string[maxColum];
                types = new string[maxColum];
                for (int c = 1; c <= maxColum; c++)
                {
                    int index = c - 1;
                    fieldNames[index] = (sheet.Cells[2, c].Value ?? "").ToString();          // 字段名
                    types[index] = (sheet.Cells[3, c].Value ?? "").ToString();               // 类型
                }

                List<string[]> listValue = new List<string[]>();
                for (int r = 5; r <= sheet.Dimension.End.Row; r++)
                {
                    object idObj = sheet.Cells[r, 1].Value;
                    if (idObj != null)
                    {
                        string[] valueArray = new string[maxColum];
                        valueArray[0] = idObj.ToString();
                        for (int c = 2; c <= maxColum; c++)
                            valueArray[c - 1] = (sheet.Cells[r, c].Text ?? "").ToString();

                        listValue.Add(valueArray);
                    }
                    else
                    {
                        break;
                    }
                }
                valueLines = listValue.ToArray();
            }
        }
    }
}

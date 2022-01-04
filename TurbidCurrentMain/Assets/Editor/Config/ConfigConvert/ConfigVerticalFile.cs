using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public class ConfigVerticalFile : ConfigFile
{
    public ConfigVerticalFile(string folder, string fileName, IDecoder decoder)
        : base(folder, fileName, decoder)
    {

    }

    protected override string GetLuaString()
    {
        StringBuilder strbld = new StringBuilder();
        strbld.AppendLine("-- 本文件中的代码为生成的代码，不允许手动修改。");
        strbld.AppendLine(DBClassName + " =");
        strbld.AppendLine("{");
        for (int i = 0; i < m_annotations.Length; i++)
        {
            string fieldName = FixFieldName(m_fieldNames[i]);
            strbld.AppendFormat("\t-- {0}: {1}", fieldName, m_annotations[i]);
            strbld.AppendLine();
        }

        for (int i = 0; i < m_valueLines.Length; i++)
        {
            strbld.Append("\t{ ");

            string[] line = m_valueLines[i];
            for (int j = 0; j < line.Length; j++)
            {
                string type = m_types[j];
                string name = FixFieldName(m_fieldNames[j]);
                string word = line[j];

                if (type.ToLower() != "skip")
                {
                    string formatValue = null;
                    try
                    {
                        formatValue = GetLuaValue(word, type);
                    }
                    catch (Exception ex)
                    {
                        string error = string.Format("Translate to lua failed, error: {0}, \r\nfile: {1}.csv, line: {2}, number: {3}, source value: {4}, name:{5}, type: {6}, annotation: {7}",
                            ex.Message, m_fileName, i + 4, j + 1, word, name, type, m_annotations[j]);
                        throw new InvalidOperationException(error);
                    }

                    strbld.AppendFormat("{0} = {1}, ", name, formatValue);
                }
            }

            strbld.Append("};");
            strbld.AppendLine();
        }
        strbld.AppendLine("};");
        strbld.AppendLine(string.Format("return {0};", DBClassName));

        return strbld.ToString();
    }

    protected override string GetCSharpClassString()
    {
        StringBuilder sbDecodeValue = new StringBuilder();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// 本文件中的代码为生成的代码，不允许手动修改。");
        sb.AppendLine("using System;");
        sb.AppendLine("using Common;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();

        // item class
        sb.AppendFormat("public partial class {0}", ItemClassName);
        sb.AppendLine();
        sb.AppendLine("{");

        for (int i = 0; i < m_types.Length; i++)
        {
            string type = m_types[i].ToLower();
            if (type == "skip")
                continue;

            string name = m_fieldNames[i];
            string annotation = m_annotations[i];
            string cSharpType = null;
            bool isContainsCN;
            string[] values = new string[m_valueLines.Length];
            for (int j = 0; j < m_valueLines.Length; j++)
                values[j] = m_valueLines[j][i];

            try
            {
                cSharpType = GetCSharpType(type, values, out isContainsCN);
            }
            catch (Exception ex)
            {
                string error = string.Format("Translate to c# failed, error: {0}, \r\nfile: {1}.csv, type: {2}, name: {3}, annotation: {4}",
                    ex.Message, m_fileName, type, name, annotation);
                throw new InvalidOperationException(error);
            }

            bool isNeedLoc = isContainsCN && m_fileName != "t_name";

            if (isNeedLoc)
            {
                string fixedFieldName = GetLocalizeStringFieldName(name);
                sb.AppendFormat("\t/// <summary>\n\t/// {0} (未翻译的原始文本)\n\t/// </summary>\n\tpublic string {1};", annotation, fixedFieldName);
                sb.AppendLine();
                sb.AppendFormat("\t/// <summary>\n\t/// {0}\n\t/// </summary>\n\tpublic string {1} {{ get {{ return Localization.Get({2}); }} }}", annotation, name, fixedFieldName);
                sb.AppendLine();
            }
            else
            {
                sb.AppendFormat("\t/// <summary>\n\t/// {2}\n\t/// </summary>\n\tpublic {0} {1};", cSharpType, name, annotation);
                sb.AppendLine();
            }

            string strValue = string.Format("words[{0}]", i);
            string strField = isNeedLoc ? "item.{0}_old" : "item.{0}";
            strField = string.Format(strField, name);
            string strDecodeValue = GetDecodeValueCode(cSharpType, strValue, strField);
            sbDecodeValue.AppendLine("\t\t\t" + strDecodeValue);
        }

        // enum
        sb.AppendLine("}");

        sb.AppendFormat(@"
public partial class {0}
{{
    public {1}[] Array;

    static {0} s_singleton;
    public static {0} Singleton
    {{
        get
        {{
            if (s_singleton == null)
                s_singleton = new {0}();
            return s_singleton;
        }}
    }}

    private {0}()
    {{
        TextAsset asset = ConfigAssetItem.Singleton.GetConfig(""{2}"");
        Decode(asset.text);
    }}

    void Decode(string text)
    {{
        string[] lines = text.Split(new char[] {{ '\n', '\r' }}, StringSplitOptions.RemoveEmptyEntries);
        Array = new {1}[lines.Length - 1];

        for (int i = 1; i < lines.Length; ++i)
        {{
            string line = lines[i];
            string[] words = line.Split('\t');
            var item = new {1}();
            Array[i - 1] = item;

{3}
        }}
    }}

}}", DBClassName, ItemClassName, m_fileName, sbDecodeValue.ToString());

        return sb.ToString();
    }

    protected string GetLocalizeStringFieldName(string oldFieldName)
    {
        return string.Format("{0}_old", oldFieldName);
    }

    protected override void SetAssetData(Assembly assembly, object dbObj)
    {
        ArrayList arrayList = new ArrayList();
        Type itemType = assembly.GetType(ItemClassName);
        var itemConstructor = itemType.GetConstructor(Type.EmptyTypes);

        for (int r = 0; r < m_valueLines.Length; r++)
        {
            string[] line = m_valueLines[r];
            var itemObj = itemConstructor.Invoke(null);
            for (int c = 0; c < line.Length; c++)
            {
                if (m_types[c].ToLower() != "skip")
                {
                    string name = m_fieldNames[c];
                    string srcValue = line[c];

                    var field = itemType.GetField(name);
                    if (field == null)
                    {
                        string locName = GetLocalizeStringFieldName(name);
                        field = itemType.GetField(locName);
                    }

                    if (field != null)
                    {
                        object valueObj = GetCSharpValue(srcValue, field.FieldType);
                        field.SetValue(itemObj, valueObj);
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning(string.Format("field {0}.{1} is not exists.", ItemClassName, name));
                    }
                }
            }
            arrayList.Add(itemObj);
        }

        Type dbType = assembly.GetType(DBClassName);
        FieldInfo fieldInfo = dbType.GetField("Array");
        fieldInfo.SetValue(dbObj, arrayList.ToArray(itemType));
    }

    protected override string GetTableString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(string.Join("\t", m_fieldNames));
        for (int i = 0; i < m_valueLines.Length; i++)
        {
            string[] line = m_valueLines[i];
            sb.AppendLine(string.Join("\t", line));
        }
        return sb.ToString();
    }
}

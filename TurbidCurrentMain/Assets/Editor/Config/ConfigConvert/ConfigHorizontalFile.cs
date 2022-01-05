using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public class ConfigHorizontalFile : ConfigFile
{
    public ConfigHorizontalFile(string folder, string fileName, IDecoder decoder)
        : base(folder, fileName, decoder)
    {

    }

    protected override string GetLuaString()
    {
        StringBuilder strbld = new StringBuilder();
        strbld.AppendLine("-- 本文件中的代码为生成的代码，不允许手动修改，生成日期：" + DateTime.Now.Date);
        strbld.AppendLine(DBClassName + " =");
        strbld.AppendLine("{");

        for (int i = 0; i < m_valueLines.Length; i++)
        {
            string[] line = m_valueLines[i];
            string name = line[0];
            string type = line[1];
            string srcValue = line[2];
            string fieldName = FixFieldName(name);

            string formatValue = null;
            try
            {
                formatValue = GetLuaValue(srcValue, type);
            }
            catch (Exception ex)
            {
                string error = string.Format("Translate to lua failed, error: {0}, \r\nfile: {1}.csv, line: {2}, source value: {3}, name:{4}, type: {5}",
                    ex.Message, m_fileName, i + 4, srcValue, name, type);
                throw new InvalidOperationException(error);
            }

            strbld.AppendFormat("\t{0} = {1};\t\t\t\t--{2}", fieldName, formatValue, line[3]);
            strbld.AppendLine();
        }

        strbld.AppendLine("}");

        return strbld.ToString();
    }

    protected override string GetCSharpClassString()
    {
        StringBuilder sbDecodeValue = new StringBuilder();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// 本文件中的代码为生成的代码，不允许手动修改");
        //sb.AppendLine("using Common;");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.IO;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();

        // item class
        sb.AppendFormat("public partial class {0}", DBClassName);
        sb.AppendLine();
        sb.AppendLine("{");

        for (int i = 0; i < m_valueLines.Length; i++)
        {
            string[] line = m_valueLines[i];
            string name = line[0];
            string type = line[1];
            string srcValue = line[2];
            string fieldName = FixFieldName(name);
            bool isContainsCN;
            string cSharpType = GetCSharpType(type, new string[] { srcValue }, out isContainsCN);

            sb.AppendFormat("\t/// <summary>\n\t/// {0}\n\t/// </summary>\n\tpublic {1} {2};", line[3], cSharpType, fieldName);
            sb.AppendLine();

            string codeField = name;
            string codeValue = string.Format("lines[{0}].Split('\\t')[1]", i);
            string codeDocValue = GetDecodeValueCode(cSharpType, codeValue, codeField);
            sbDecodeValue.AppendLine("\t\t" + codeDocValue);
        }

        sb.AppendFormat(@"

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
{3}
    }}

}}", DBClassName, ItemClassName, m_fileName, sbDecodeValue.ToString());

        return sb.ToString();
    }

    protected override void SetAssetData(Assembly assembly, object dbObj)
    {
        Type dbType = assembly.GetType(DBClassName);

        for (int i = 0; i < m_valueLines.Length; i++)
        {
            string[] line = m_valueLines[i];
            string fieldName = FixFieldName(line[0]);

            var field = dbType.GetField(fieldName);
            if (field == null)
                throw new InvalidOperationException(string.Format("field {0}.{1} is not exists.", DBClassName, fieldName));

            string srcValue = line[2];
            object valueObj = GetCSharpValue(srcValue, field.FieldType);

            field.SetValue(dbObj, valueObj);
        }
    }

    protected override string GetTableString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < m_valueLines.Length; i++)
        {
            string[] line = m_valueLines[i];
            sb.AppendLine(string.Format("{0}\t{1}", line[0], line[2]));
        }
        return sb.ToString();
    }
}

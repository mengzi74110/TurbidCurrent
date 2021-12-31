using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using System.Text.RegularExpressions;

//将Excel 转换为 CSV，再转换为CS或者Lua的基类
public abstract class ConfigFile
{
    protected string m_fileName;                    // 文件名
    protected string[] m_fieldNames;                // 变量名
    protected string[] m_types;                     // 类型名
    protected string[] m_annotations;               // 注释
    protected string[][] m_valueLines;              // 值

    public interface IDecoder
    {
        void DecodeFile(string folder, string fileName, out string[] fieldNames, out string[] types, out string[] annotations, out string[][] valueLines);
    }
    public ConfigFile(string folder, string fileName, IDecoder decoder)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("fileName");
        if (decoder == null)
            throw new ArgumentNullException("decoder");

        m_fileName = Path.GetFileNameWithoutExtension(fileName);

        decoder.DecodeFile(folder, fileName, out m_fieldNames, out m_types, out m_annotations, out m_valueLines);
    }

    protected string DBClassName
    {
        get { return string.Format("Config_{0}", m_fileName); }
    }

    protected string ItemClassName
    {
        get { return string.Format("Item_{0}", m_fileName); }
    }

    protected virtual string TargetTblFolder
    {
        get { return AllEditorPathConfig.TargetTblFolder; }
    }


    #region static

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

    protected static string GetLuaValue(string srcValue, string type)
    {
        srcValue = srcValue ?? "";
        if (string.IsNullOrEmpty(type))
            throw new ArgumentException("type");

        type = type.Trim();
        string typeLower = type.ToLower();
        string target;

        switch (typeLower)
        {
            case "int32":
            case "uint32":
            case "byte":
            case "ushort":
            case "uint16":
            case "uint":
            case "int":
                srcValue = srcValue.Replace(" ", "");
                if (string.IsNullOrEmpty(srcValue))
                    target = default(int).ToString();
                else
                {
                    long l;
                    if (!long.TryParse(srcValue, out l))
                        throw new InvalidOperationException(srcValue + " is not a number.");
                    target = srcValue;
                }
                break;

            case "float":
                srcValue = srcValue.Replace(" ", "");
                if (string.IsNullOrEmpty(srcValue))
                    target = default(float).ToString();
                else
                {
                    float f;
                    if (!GameHelper.TryParseFloat(srcValue, out f))
                        throw new InvalidOperationException(srcValue + " is not a int.");
                    target = srcValue;
                }
                break;

            case "string":
                srcValue = srcValue.TrimStart('"').TrimEnd('"').Replace("\n", "\\n");
                target = string.Format("\"{0}\"", srcValue);
                break;

            case "bool":
                if (string.IsNullOrEmpty(srcValue))
                    target = default(bool).ToString();
                else
                {
                    bool b;
                    if (!bool.TryParse(srcValue.ToLower(), out b))
                        throw new InvalidOperationException(srcValue + " is not a boolean.");
                    target = srcValue.ToLower();
                }
                break;

            default:
                throw new InvalidOperationException("Unknown type: " + type);
        }

        return target;
    }

    protected static string GetCSharpType(string type, string[] values, out bool isContainsCN)
    {
        type = type.Trim();
        string target;
        string typeLower = type.ToLower();
        isContainsCN = false;

        switch (typeLower)
        {
            case "uint64":
            case "ulong":
                target = "ulong";
                break;

            case "int32":
            case "int":
                target = "int";
                break;

            case "uint32":
            case "uint":
                target = "uint";
                break;

            case "byte":
                target = "byte";
                break;

            case "ushort":
            case "uint16":
                target = "ushort";
                break;

            case "float":
                target = "float";
                break;

            case "string":
                foreach (var v in values)
                {
                    if (ContainsChinese(v))
                    {
                        isContainsCN = true;
                        break;
                    }
                }
                target = "string";
                break;

            case "bool":
                target = "bool";
                break;

            default:
                throw new InvalidOperationException("Unknown type: " + type);
        }

        return target;
    }

    public static string GetDecodeValueCode(string csharpType, string strValue, string fieldName)
    {
        switch (csharpType)
        {
            case "ulong":
                return string.Format("ulong.TryParse({0}, out {1});", strValue, fieldName);
            case "int":
                return string.Format("int.TryParse({0}, out {1});", strValue, fieldName);
            case "uint":
                return string.Format("uint.TryParse({0}, out {1});", strValue, fieldName);
            case "byte":
                return string.Format("byte.TryParse({0}, out {1});", strValue, fieldName);
            case "ushort":
                return string.Format("ushort.TryParse({0}, out {1});", strValue, fieldName);
            case "float":
                return string.Format("FBGameHelper.TryParseFloat({0}, out {1});", strValue, fieldName);

            case "string":
                return string.Format("{0} = {1};", fieldName, strValue);

            case "bool":
                return string.Format("{0} = {1} == \"1\";", fieldName, strValue);

            default:
                throw new InvalidOperationException("Unknown c# type: " + csharpType);
        }
    }

    public static bool ContainsChinese(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;
        string pattern = "[\u4e00-\u9fbb]";
        return Regex.IsMatch(text, pattern);
    }

    protected static object GetCSharpValue(string srcValue, Type type)
    {
        object target;

        switch (type.ToString())
        {
            // ulong
            case "System.UInt64":
                srcValue = srcValue.Replace(" ", "");
                if (string.IsNullOrEmpty(srcValue))
                    target = default(ulong);
                else
                {
                    ulong ul;
                    if (!ulong.TryParse(srcValue, out ul))
                        throw new InvalidOperationException(srcValue + " is not a ulong.");
                    target = ul;
                }
                break;

            // int
            case "System.Int32":
                srcValue = srcValue.Replace(" ", "");
                if (string.IsNullOrEmpty(srcValue))
                    target = default(int);
                else
                {
                    int i;
                    if (!int.TryParse(srcValue, out i))
                        throw new InvalidOperationException(srcValue + " is not a int.");
                    target = i;
                }
                break;

            // uint
            case "System.UInt32":
                srcValue = srcValue.Replace(" ", "");
                if (string.IsNullOrEmpty(srcValue))
                    target = default(uint);
                else
                {
                    uint i;
                    if (!uint.TryParse(srcValue, out i))
                        throw new InvalidOperationException(srcValue + " is not a uint.");
                    target = i;
                }
                break;

            // ushort
            case "System.UInt16":
                srcValue = srcValue.Replace(" ", "");
                if (string.IsNullOrEmpty(srcValue))
                    target = default(ushort);
                else
                {
                    ushort b;
                    if (!ushort.TryParse(srcValue, out b))
                        throw new InvalidOperationException(string.Format("{0} is not a ushort", srcValue));
                    target = b;
                }
                break;

            // byte
            case "System.Byte":
                srcValue = srcValue.Replace(" ", "");
                if (string.IsNullOrEmpty(srcValue))
                    target = default(byte);
                else
                {
                    byte b;
                    if (!byte.TryParse(srcValue, out b))
                        throw new InvalidOperationException(srcValue + " is not a byte.");
                    target = b;
                }
                break;

            // float
            case "System.Single":
                srcValue = srcValue.Replace(" ", "");
                if (string.IsNullOrEmpty(srcValue))
                    target = default(float);
                else
                {
                    float f;
                    if (!GameHelper.TryParseFloat(srcValue, out f))
                        throw new InvalidOperationException(srcValue + " is not a float.");
                    target = f;
                }
                break;

            // string
            case "System.String":
                target = srcValue.TrimStart('"').TrimEnd('"');
                break;

            // bool
            case "System.Boolean":
                if (string.IsNullOrEmpty(srcValue))
                    target = default(bool);
                else
                {
                    bool b;
                    if (!bool.TryParse(srcValue.ToLower(), out b))
                        throw new InvalidOperationException(srcValue + " is not a boolean.");
                    target = b;
                }
                break;

            default:
                target = srcValue;
                throw new InvalidOperationException("Unexpected c# type: " + type.ToString());
        }

        return target;
    }

    #endregion


    protected abstract string GetLuaString();
    protected abstract string GetCSharpClassString();
    protected abstract void SetAssetData(Assembly assembly, object dbObj);
    protected abstract string GetTableString();

    // 转换成lua
    public void ConvertToLua()
    {
        string luaString = GetLuaString();
        string assetPath = string.Format("{0}{1}.lua.txt", AllEditorPathConfig.LuaFolder, DBClassName);
        string fullPath = PathHelper.GetFullPath(assetPath);

        if (File.Exists(fullPath))
        {
            string oldText = File.ReadAllText(fullPath);
            if (oldText == luaString)
                return;
        }

        PathHelper.CreateDirectory(fullPath);
        File.WriteAllText(fullPath, luaString);
    }

    // 转换成 c#
    public void ConvertToCS()
    {
        string itemClassString = GetCSharpClassString();
        string assetPath = string.Format("{0}{1}.cs", AllEditorPathConfig.CSharpFolder, DBClassName);
        string fullPath = PathHelper.GetFullPath(assetPath);

        if (File.Exists(fullPath))
        {
            string oldText = File.ReadAllText(fullPath);
            if (oldText == itemClassString)
                return;
        }

        PathHelper.CreateDirectory(fullPath);
        File.WriteAllText(fullPath, itemClassString);
    }

    // 转换成 asset
    public void ConvertToAsset()
    {
        Assembly assembly = Assembly.Load("Assembly-CSharp");
        Type dbType = assembly.GetType(DBClassName);
        if (dbType == null)
            throw new InvalidOperationException(string.Format("c# class {0} is not exists.", DBClassName));

        // create asset
        string assetPath = string.Format("{0}{1}.asset", AllEditorPathConfig.TargetAssetFolder, m_fileName);
        var asset = AssetDatabase.LoadAssetAtPath(assetPath, dbType);
        if (asset == null)
        {
            var obj = ScriptableObject.CreateInstance(DBClassName);
            PathHelper.CreateDirectory(assetPath);
            AssetDatabase.CreateAsset(obj, assetPath);
            asset = AssetDatabase.LoadAssetAtPath(assetPath, dbType);
        }

        SetAssetData(assembly, asset);
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
    }

    //TODO: 提取中文
   

    protected string FixFieldName(string oldFieldName)
    {
        return string.Format("{0}{1}", char.ToUpper(oldFieldName[0]), oldFieldName.Substring(1));
    }

    public void ConvertToTbl()
    {
        string tblPath = string.Format("{0}{1}.txt", TargetTblFolder, m_fileName);
        string text = GetTableString();
        File.WriteAllText(tblPath, text);
    }
}

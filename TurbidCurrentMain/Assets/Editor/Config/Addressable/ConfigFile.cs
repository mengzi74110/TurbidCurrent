using System.Collections.Generic;
using UnityEngine;

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
}

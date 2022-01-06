using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AllEditorPathConfig
{
    //打包相关路径
    public static string Folder_main = "Assets/MainProject/BuildAssets/";



    //Excel转换配置文件路径；
    public static string SettingFilePath = "Assets/Editor/Config/ConfigConvert/ConfigConverterSettings_Config.csv";

    //Excel转换为txt路径
    public static string TargetTblFolder = "Assets/MainProject/BuildAssets/Config/Tbl/";
    //脚本文件路径
    public static string ScriptsPath = "Assets/MainProject/Scripts";
    //Excel转换生成 Class文件路径
    public static string CSharpFolder = "Assets/MainProject/Scripts/Config/Define/";
    //Excel转换生成Lua文件路径
    public const string LuaFolder = "Assets/MainProject/Scripts/Config/LuaScripts/";

    public const string TargetAssetFolder = "Assets/MainProject/BuildAssets/Config/Asset/";
}

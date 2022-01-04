using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics; //调用Windows进程函数；
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
public class ConfigConverter : ScriptableObject
{
    static ConfigFile CreateExcelFile(string folder, ConfigConverterSettings.Item item)
    {
        ConfigFile file = null;
        var decoder = new ConfigExcelDecoder();

        if (item != null)
        {
            if (item.IsHorizontal)
                file = new ConfigHorizontalFile(folder, item.Name, decoder);
            else
                file = new ConfigVerticalFile(folder, item.Name, decoder);
        }

        return file;
    }

    [MenuItem("CustomToolbar/Config/Generate C# Class (Danger !!!)")]
    public static void GenerateCSharp()
    {
        bool ok = EditorUtility.DisplayDialog("生成c#", "慎用！！！打包机上禁止使用此功能，否则可能会造成AB包的C#代码与游戏包体的C#代码不一致。\n确定要重新生成配置对应的 C# 类吗？", "确定", "取消");
        if (!ok)
            return;

        // 计时
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        string folder;
        var settings = ConfigConverterSettings.ReadSettings(out folder);
        UnityEngine.Debug.Log("SourceConfigFolder:" + folder);

        foreach (var item in settings)
        {
            if (item.IsToTable)
            {
                ConfigFile csv = CreateExcelFile(folder, item);
                csv.ConvertToCS();
            }
        }

        stopwatch.Stop();
        string dialogText = string.Format("耗时： {0}。C# 类生成成功！", GetTimeString(stopwatch.ElapsedMilliseconds));
        EditorUtility.DisplayDialog("生成c#", dialogText, "确定");

        AssetDatabase.Refresh();
    }

    public static void GenerateCSharp(List<ConfigConverterSettings.Item> list)
    {
        // 计时
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        string folder;
        var settings = ConfigConverterSettings.ReadSettings(out folder);

        foreach (var item in list)
        {
            if (item.IsToTable && item.IsGenerateCSharp)
            {
                ConfigFile csv = CreateExcelFile(folder, item);
                if (csv != null)
                    csv.ConvertToCS();
            }
        }

        stopwatch.Stop();
        string dialogText = string.Format("耗时： {0}。C# 类生成成功！", GetTimeString(stopwatch.ElapsedMilliseconds));
        EditorUtility.DisplayDialog("生成c#", dialogText, "确定");

        AssetDatabase.Refresh();
    }

    [MenuItem("CustomToolbar/Config/Convert All")]
    static void Convert()
    {
        bool ok = EditorUtility.DisplayDialog("转换配置", "确定要转换配置吗？\n\n注意：要等代码编译完才能执行此步骤...", "确定", "取消");
        if (!ok)
            return;

        string dialogText = ConvertAllConfig();
        UnityEngine.Debug.Log(dialogText);
        EditorUtility.DisplayDialog("成功", dialogText, "确定");
    }

    public static string ConvertAllConfig()
    {
        string folder;
        var settings = ConfigConverterSettings.ReadSettings(out folder);
        return ConvertConfig(settings);
    }

    public static string ConvertConfig(List<ConfigConverterSettings.Item> list)
    {
        // 计时
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        string folder = ConfigConverterSettings.GetSourceConfigFolder();

        List<ConfigConverterSettings.Item> temp = new List<ConfigConverterSettings.Item>();
        foreach (var item in list)
        {
            if (item.IsToLua || item.IsToTable)
                temp.Add(item);
        }

        for (int i = 0; i < temp.Count; i++)
        {
            float progress = (float)i / temp.Count;
            EditorUtility.DisplayProgressBar($"转化配置{progress:p2}", $"正在转化({i}/{temp.Count})...", progress);

            var item = temp[i];
            ConfigFile configFile = CreateExcelFile(folder, item);

            if (item.IsToLua)
                configFile.ConvertToLua();
            if (item.IsToTable)
            {
                try
                {
                    //configFile.ConvertToAsset();
                    configFile.ConvertToTbl();
                }
                catch (Exception ex)
                {
                    string errorMsg = string.Format("Failed! File: {0}, Try use [CSV/Generate c# class] first please.\r\nerror: {0}, file: {1}", item.Name, ex.Message);
                    UnityEngine.Debug.LogError(errorMsg);
                    throw;
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        stopwatch.Stop();
        return string.Format("耗时： {0}。配置转换成功！", GetTimeString(stopwatch.ElapsedMilliseconds));
    }

    [MenuItem("CustomToolbar/Config/Convert Lua Only")]
    public static void ConvertLuaOnly()
    {
        bool ok = EditorUtility.DisplayDialog("转换Lua", "确定要转换Lua吗？", "确定", "取消");
        if (!ok)
            return;

        // 计时
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        string folder;
        var settings = ConfigConverterSettings.ReadSettings(out folder);

        foreach (var item in settings)
        {
            if (item.IsToLua)
            {
                ConfigFile configFile = CreateExcelFile(folder, item);
                configFile.ConvertToLua();
            }
        }

        AssetDatabase.Refresh();

        stopwatch.Stop();
        string dialogText = string.Format("耗时： {0}。配置Lua成功！", GetTimeString(stopwatch.ElapsedMilliseconds));
        EditorUtility.DisplayDialog("成功", dialogText, "确定");
    }

    //[MenuItem("Localize/Get/Get Chinese of Config")]
    #region 不使用本地化文件相关代码
    //public static void GetChinese_Config(List<string[]> allLines)
    //{
    //    bool ok = EditorUtility.DisplayDialog("提取本地化文本", "确定要提取本地化文本？", "确定", "取消");
    //    if (!ok)
    //        return;

    //    // 计时
    //    Stopwatch stopwatch = new Stopwatch();
    //    stopwatch.Start();

    //    // get string from config
    //    List<string> allText = new List<string>();
    //    string folder;
    //    var settings = ConfigConverterSettings.ReadSettings(out folder);
    //    foreach (var item in settings)
    //    {
    //        if (item.IsToLua || item.IsToTable)
    //        {
    //            string itemName = Path.GetFileNameWithoutExtension(item.Name);
    //            itemName = itemName.ToLower();
    //            bool donotLocalize =
    //                itemName == "t_system_config"
    //                || itemName == "t_drop"
    //                || itemName == "t_music"
    //                || itemName == "t_redeem_code"
    //                || itemName == "t_game_config"
    //                || itemName == "t_name";

    //            if (donotLocalize)
    //                continue;

    //            ConfigFile configFile = CreateExcelFile(folder, item);
    //            configFile.GetLocalizationStrings(allLines, ref allText);
    //        }
    //    }

    //    // get old config string
    //    string path = "Assets/localize/Text/ConfigLocalization.csv";
    //    List<string[]> oldLines = new List<string[]>();
    //    string oldText = null;
    //    if (File.Exists(path))
    //    {
    //        LocalizationTools.DecodeLocCSV(path, ref oldLines, Encoding.UTF8);
    //        oldText = File.ReadAllText(path);
    //    }

    //    StringBuilder sb = new StringBuilder();
    //    if (oldText != null)
    //        sb.Append(oldText);
    //    else
    //        sb.AppendLine(",Chinese,English");

    //    foreach (var s in allText)
    //    {
    //        bool repeat = oldLines.Any(l => l.Length > 0 && l[0] == s);
    //        if (!repeat)
    //        {
    //            string content = s.Contains(",") ? string.Format("\"{0}\"", s) : s;
    //            sb.AppendLine(string.Format("{0},{0},", content));
    //        }
    //    }

    //    // save to loc config
    //    string newText = sb.ToString().Replace("\\n", "&");
    //    File.WriteAllText(path, newText);
    //    AssetDatabase.Refresh();

    //    stopwatch.Stop();
    //    string dialogText = string.Format("耗时： {0}。提取本地化文本成功！\n目标文件在：{1}\n", GetTimeString(stopwatch.ElapsedMilliseconds), path);
    //    UnityEngine.Debug.Log(dialogText);
    //    EditorUtility.DisplayDialog("成功", dialogText, "确定");
    //} 
    #endregion

    static string GetTimeString(long milliseconds)
    {
        int totalSeconds = (int)(milliseconds / 1000f);
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = totalSeconds % 60;
        return string.Format("{0} 分 {1} 秒", minutes, seconds);
    }
}


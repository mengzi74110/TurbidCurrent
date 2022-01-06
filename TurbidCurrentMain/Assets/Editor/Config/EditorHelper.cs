using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEditor;

//编辑器辅助类
public class EditorHelper
{
    public static bool IsImage(string path)
    {
        string ext = Path.GetExtension(path);
        return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".tga";
    }


    /// <summary>
    /// 获取指定目录的资源
    /// </summary>
    /// <param name="filter">过滤器：
    /// 若以t:开头，表示用unity的方式过滤; 
    /// 若以f:开头，表示用windows的SearchPattern方式过滤; 
    /// 若以r:开头，表示用正则表达式的方式过滤。</param>
    public static string[] GetAssets(string folder, string filter)
    {
        if (string.IsNullOrEmpty(folder))
            throw new ArgumentException("Folder");
        if (string.IsNullOrEmpty(filter))
            throw new ArgumentException("Filter");
        folder = folder.TrimEnd('/').TrimEnd('\\');

        if (filter.StartsWith("t:"))
        {
            string[] guids = AssetDatabase.FindAssets(filter, new string[] { folder });
            string[] paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            return paths;
        }
        else if (filter.StartsWith("f:"))
        {
            string folderFullPath = GetFullPath(folder);
            string searchPattern = filter.Substring(2);
            string[] files = Directory.GetFiles(folderFullPath, searchPattern, SearchOption.AllDirectories);
            string[] paths = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
                paths[i] = GetAssetPath(files[i]);
            return paths;
        }
        else if (filter.StartsWith("r:"))
        {
            string folderFullPath = GetFullPath(folder);
            string pattern = filter.Substring(2);
            string[] files = Directory.GetFiles(folderFullPath, "*.*", SearchOption.AllDirectories);
            List<string> list = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                string name = Path.GetFileName(files[i]);
                if (Regex.IsMatch(name, pattern))
                {
                    string p = GetAssetPath(files[i]);
                    list.Add(p);
                }
            }
            return list.ToArray();
        }
        else
        {
            throw new InvalidOperationException("Unexpected filter: " + filter);
        }
    }
    // asset path 转 full path
    public static string GetFullPath(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
            return "";

        string p = Application.dataPath + assetPath.Substring(6);
        return p.Replace("\\", "/");
    }
    // Fullpath 转 Unity Assetpath
    public static string GetAssetPath(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
            return "";

        fullPath = fullPath.Replace("\\", "/");
        return fullPath.StartsWith("Assets/") ?
            fullPath :
            "Assets" + fullPath.Substring(Application.dataPath.Length);
    }
    public static string GetAddress_RelativePath(string assetPath, string targetFolder)
    {
        targetFolder = targetFolder.TrimEnd('/') + '/';
        string ext = Path.GetExtension(assetPath);
        int startIndex = targetFolder.Length;
        int len = assetPath.Length - targetFolder.Length - ext.Length;
        string address = assetPath.Substring(startIndex, len);
        address = address.Replace("/", "@").Replace("\\", "@");
        return address;
    }

}

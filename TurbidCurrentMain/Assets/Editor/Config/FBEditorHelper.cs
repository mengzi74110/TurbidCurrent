using System.Collections.Generic;
using UnityEngine;
using System.IO;

//编辑器辅助类
public class FBEditorHelper
{
    public static bool IsImage(string path)
    {
        string ext = Path.GetExtension(path);
        return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".tga";
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
}

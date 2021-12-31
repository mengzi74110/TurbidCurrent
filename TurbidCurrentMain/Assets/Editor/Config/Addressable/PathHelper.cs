using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PathHelper
{
    public static string GetFullPath(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
            throw new ArgumentException("assetPath");

        if (!assetPath.StartsWith("Assets/"))
            throw new ArgumentException("error format: " + assetPath);

        string path = assetPath.Substring(7);
        string fullPath = string.Format("{0}/{1}", Application.dataPath, path);
        return fullPath.Replace("\\", "/");
    }

    public static void CreateDirectory(string path)
    {
        string dirPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
    }
}
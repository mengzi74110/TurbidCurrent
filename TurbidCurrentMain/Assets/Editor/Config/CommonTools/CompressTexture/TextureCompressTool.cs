using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class TextureCompressTool : ScriptableObject
{
    [MenuItem("CustomToolbar/Common/Select Compress Texture")]
    static void CompressSlectTexture()
    {
        List<string> list = new List<string>();

        foreach (var so in Selection.objects)
        {
            bool isFolder = so is DefaultAsset;
            string selectObjPath = AssetDatabase.GetAssetPath(so);
            if (isFolder)
            {
                string[] files = Directory.GetFiles(selectObjPath, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    string filepath = files[i];
                    if (EditorHelper.IsImage(filepath))
                    {
                        string assetpath = EditorHelper.GetAssetPath(filepath);
                        if (!list.Contains(assetpath))
                            list.Add(assetpath);
                    }
                }
            }
            else
            {
                if (!list.Contains(selectObjPath))
                    list.Add(selectObjPath);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            string assetpath = list[i];
            CompressTexture(assetpath);
            EditorUtility.DisplayProgressBar("批量处理图片", assetpath, (float)i / list.Count);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("All done, count: " + list.Count);
    }

    static void CompressTexture(string assetpath)
    {
        // 根据路径获得文件目录，设置图集的packagingTag
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetpath) as TextureImporter;
        CompressTexture(textureImporter);
    }
    public static void CompressTexture(TextureImporter textureImporter)
    {
        // 根据路径获得文件目录，设置图集的packagingTag
        if (!textureImporter)
        {
            Debug.LogWarning("textureImporter == null");
            return;
        }
        if (textureImporter.textureType != TextureImporterType.Default)
        {
            Debug.LogWarning("This texture is not Default Type: " + textureImporter.assetPath, textureImporter);
            return;
        }
        bool haveAlpha = textureImporter.DoesSourceTextureHaveAlpha();

        bool isChanged = false;

        if (textureImporter.alphaIsTransparency != haveAlpha
            | textureImporter.mipmapEnabled != false
            | textureImporter.isReadable != false
            | textureImporter.npotScale != TextureImporterNPOTScale.ToNearest)
        {
            textureImporter.alphaIsTransparency = haveAlpha;
            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = false;
            //textureImporter.textureType = TextureImporterType.Default;
            //textureImporter.wrapMode = TextureWrapMode.Clamp;
            textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;

            isChanged = true;
        }

        // Android 端单独设置
        string androidName = "Android";
        var targetAndroidFormat = haveAlpha ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC_RGB4;
        textureImporter.GetPlatformTextureSettings(androidName, out int _, out TextureImporterFormat format);
        if (format != targetAndroidFormat)
        {
            TextureImporterPlatformSettings settingAndroid = new TextureImporterPlatformSettings()
            {
                name = androidName,
                overridden = true,
                format = targetAndroidFormat,
            };
            textureImporter.SetPlatformTextureSettings(settingAndroid);
            isChanged = true;
        }

        // IOS端单独设置
        string iosName = "iOS";
        var targetIosFormat = TextureImporterFormat.ASTC_6x6;
        textureImporter.GetPlatformTextureSettings(iosName, out _, out format);
        if (format != targetIosFormat)
        {
            TextureImporterPlatformSettings settingIOS = new TextureImporterPlatformSettings()
            {
                name = iosName,
                overridden = true,
                format = targetIosFormat,
            };
            textureImporter.SetPlatformTextureSettings(settingIOS);
            isChanged = true;
        }

        if (isChanged)
        {
            textureImporter.SaveAndReimport();
            Debug.Log("Compress texture done: " + textureImporter.assetPath);
        }
    }
}

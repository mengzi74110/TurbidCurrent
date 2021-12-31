using UnityEngine;
using UnityEditor;

//自动导入Unity资源之后执行的脚本；
public class AssetImporterProcessor : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
        TextureCompressTool.CompressTexture(assetImporter as TextureImporter);
    }
}

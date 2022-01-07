using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using static UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema;
using System.Text;
using System;
using System.IO;

public class AddressableGroupSetter : ScriptableObject
{
    public static AddressableAssetSettings CurSettings => AddressableAssetSettingsDefaultObject.Settings;
    static List<AddressableGroupData> s_listGroupData;
    static StringBuilder s_sb = new StringBuilder();

    public enum GroupType
    {
        Local,              // 打到本地
        RemoteLogin,        // 打到远端，登陆时下载
        RemoteDynamic,      // 打到远端，动态下载，或者静默下载
    }

    [MenuItem("CustomToolbar/Addressable/Clear Groups")]
    static void ClearGroups()
    {
        List<string> guids = new List<string>();
        guids.Clear();
        //只移除groups下面的条目，不移除groups。（在Mac下，移除Groups以后，Groups的设置 莫名其妙重置的bug）
        foreach (var group in CurSettings.groups)
        {
            foreach (var item in group.entries)
            {
                guids.Add(item.guid);
            }
        }
        foreach (var item in guids)
        {
            CurSettings.RemoveAssetEntry(item);
        }
        Debug.Log("Clear AddressableGroups Entries Done!");
        //1:移除Groups
        //var listGroups = CurSettings.groups;
        //for (int i = 0; i < listGroups.Count; i++)
        //{
        //    CurSettings.RemoveGroup(listGroups[i]);
        //}
        //2：移除Groups
        //CurSettings.groups.Clear();
    }

    [MenuItem("CustomToolbar/Addressable/Reset Groups")]
    static void ResetGroups_Main()
    {
        List<AddressableGroupData> groupDatas = GetGroupDatas(false);
        foreach (var item in groupDatas)
        {
            item.CreatGroup();
        }
        Debug.Log("~~ Reset Groups Done ~~");
    }
  
    [MenuItem("CustomToolbar/Addressable/Find AddressName By GUID")]
    static void FindAddressNameByGuid()
    {
        bool isFindTarget = false;
        string guid = "???";
        foreach (var group in CurSettings.groups)
        {
            foreach (var entry in group.entries)
            {
                if (entry.guid == guid)
                {
                    isFindTarget = true;
                    Debug.Log($"Find Target,Address: {entry.address} ,Path: {entry.AssetPath} ,GUID: {entry.guid} ");
                }
            }
        }

        if (isFindTarget == false)
            Debug.Log($"Don't Find Address By GUID: {guid} ");
    }
    static List<AddressableGroupData> GetGroupDatas(bool calCHash)
    {
        if (s_listGroupData != null)
            s_listGroupData.Clear();
        ResetAllGroups();
        if (calCHash)
        {
            foreach (var item in s_listGroupData)
            {
                //计算哈希值；
                item.CalcHash();
            }
        }
        return s_listGroupData;
    }
    #region 分组相关信息设置

    static void ResetAllGroups()
    {
        // prefab
        ResetGroup<GameObject>("main_prefab", GroupType.RemoteDynamic, BundlePackingMode.PackTogether, $"{AllEditorPathConfig.Folder_main}Prefab/", "f:*.prefab", assetPath =>
        {
            Debug.Log($"assetPath:{assetPath}");
            return EditorHelper.GetAddress_RelativePath(assetPath, AllEditorPathConfig.Folder_main + "Prefab/");
        });

        // config
        ResetGroup<TextAsset>("config", GroupType.RemoteLogin, BundlePackingMode.PackTogether, AllEditorPathConfig.TargetTblFolder, "f:*.txt", assetPath =>
        {
            return EditorHelper.GetAddress_RelativePath(assetPath, AllEditorPathConfig.TargetTblFolder);
        });

        //// ai
        //ResetGroup<ScriptableObject>("main_ai", BundlePackingMode.PackTogether, $"{Folder_main}AI/", "f:*.asset", assetPath =>
        //{
        //    return EditorHelper.GetAddress_RelativePath(assetPath, Folder_main);
        //});

        //// anim
        //ResetGroup<RuntimeAnimatorController>("main_anim", BundlePackingMode.PackTogether, $"{Folder_main}Anim/", "f:*.controller", assetPath =>
        //{
        //    return EditorHelper.GetAddress_RelativePath(assetPath, Folder_main);
        //});
        //ResetGroup<RuntimeAnimatorController>("music_anim", BundlePackingMode.PackTogether, $"{Folder_music}Anim/female/", "f:*.anim", assetPath =>
        //{
        //    return EditorHelper.GetAddress_RelativePath(assetPath, Folder_music);
        //});

        ///*
        //// scene
        //ResetGroup<RuntimeAnimatorController>("art_scene", BundlePackingMode.PackTogether, $"{Folder_art}Scenes/", "f:*.unity", assetPath =>
        //{
        //    return GetAddress_RelativePath(assetPath, Folder_art);
        //});*/

        //// scene
        //ResetGroup<RuntimeAnimatorController>("art_image", BundlePackingMode.PackTogether, $"{Folder_art}Image/", "t:Texture2D", assetPath =>
        //{
        //    return EditorHelper.GetAddress_RelativePath(assetPath, Folder_art);
        //});

        //// mat
        //ResetGroup<RuntimeAnimatorController>("main_material", BundlePackingMode.PackTogether, $"{Folder_main}Material/", "f:*.mat", assetPath =>
        //{
        //    return EditorHelper.GetAddress_RelativePath(assetPath, Folder_main);
        //});

        //// dance music
        //ResetGroup<AudioClip>("dance_music", BundlePackingMode.PackTogether, $"{Folder_music}DanceMusic/", "f:*.mp3", assetPath =>
        //{
        //    return EditorHelper.GetAddress_RelativePath(assetPath, Folder_music);
        //});

        //// dance data
        //ResetGroup<ScriptableObject>("dance_data", BundlePackingMode.PackTogether, $"{Folder_music}DanceData/", "f:*.asset", assetPath =>
        //{
        //    return EditorHelper.GetAddress_RelativePath(assetPath, Folder_music);
        //});
    }

    static void ResetGroup<T>(string groupName, GroupType groupType, BundlePackingMode packMode, string assetFolder, string filter, Func<string, string> getAddress)
    {
        Debug.Log("暂时把所有的GroupType 设置为GroupType.Local");
        groupType = GroupType.Local;
#if UNITY_EDITOR_OSX
        if (groupType == GroupType.RemoteLogin)
            groupType = GroupType.Local;
#endif

        AddressableGroupData data = new AddressableGroupData(groupName, groupType, packMode, typeof(T));
        data.GetAssets(assetFolder, filter, getAddress);
        RegisterGroupData(data);
    }

    public static void RegisterGroupData(AddressableGroupData data)
    {
        if (data == null)
            return;
        if (s_listGroupData == null)
            s_listGroupData = new List<AddressableGroupData>();
        s_listGroupData.Add(data);
    }


    #endregion



    #region 线上维护打包
    [MenuItem("CustomToolbar/OnLinePacke/Reset Groups Main")]
    static void Package_ResetGoupsMain()
    {
        var groupDatas = GetGroupDatas(true);
        foreach (var item in groupDatas)
        {
            item.CreatGroup();
        }
        s_sb.Length = 0;
        Debug.Log("groupDatas numbers: " + groupDatas.Count);
        foreach (var item in groupDatas)
        {
            s_sb.Append(item.ToString());
        }
        string groupDatafilePath= $"{AllEditorPathConfig.GroupDataFileFolder}main_{EditorHelper.GetPlatformString()}.txt";
        EditorHelper.TryCreatDir(groupDatafilePath);

        File.WriteAllText(groupDatafilePath, s_sb.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();
        Debug.Log("Write group data file done: " + groupDatafilePath);
    } 
    #endregion

}

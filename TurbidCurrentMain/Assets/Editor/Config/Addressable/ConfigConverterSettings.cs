﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class ConfigConverterSettings : ScriptableObject
{
    public static AddressableAssetSettings CurSettings => AddressableAssetSettingsDefaultObject.Settings;


    [MenuItem("CustomToolbar/Addressable/Clear Groups")]
    static void ClearGroups()
    {
        List<string> guids = new List<string>();
        //只移除groups下面的条目，不移除groups。（在Mac下，移除Groups以后，Groups的设置 莫名其妙重置的bug）
        foreach (var group in CurSettings.groups)
        {
            guids.Clear();
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
        //移除Groups
        //var listGroups = CurSettings.groups;
        //for (int i = 0; i < listGroups.Count; i++)
        //{
        //    CurSettings.RemoveGroup(listGroups[i]);
        //}
    }

    [MenuItem("CustomToolbar/Addressable/Reset Groups")]
    static void ResetGroups_Main()
    { 

    }
}

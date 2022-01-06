using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema;
public class AddressableGroupData 
{
    static StringBuilder s_sb = new StringBuilder();
    string m_groupName;
    AddressableGroupSetter.GroupType m_groupType;
    BundlePackingMode m_packMode;
    Type m_assetType;

    string m_assetFolder;
    string m_assetFilter;
    Func<string, string> m_getAddress;

    string[] m_assets;
    string[] m_addressNames;


    public void GetAssets(string assetFolder,string filter, Func<string, string> getAddress)
    {
        this.m_assetFilter = filter;
        this.m_assetFolder = assetFolder;
        this.m_getAddress = getAddress;
        m_assets= EditorHelper.GetAssets(assetFolder, filter);
        m_addressNames = new string[m_assets.Length];
        for (int i = 0; i < m_assets.Length; i++)
        {
            m_addressNames[i] = getAddress(m_assets[i].ToLower().Replace(" ", ""));
        }
    }
    public AddressableGroupData(string groupName,AddressableGroupSetter.GroupType groupType,BundlePackingMode packingMode,Type assetType)
    {
        m_groupName = groupName;
        m_groupType = groupType;
        m_packMode = packingMode;
        m_assetType = assetType;
    }
}

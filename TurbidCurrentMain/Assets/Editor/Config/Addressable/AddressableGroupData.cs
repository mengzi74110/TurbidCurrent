using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using static UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema;
public class AddressableGroupData 
{
    static StringBuilder s_sb = new StringBuilder();
    string m_groupName;
    AddressableGroupSetter.GroupType m_groupType; //加载方式： 本地、远端、静默；
    BundlePackingMode m_packMode; //打包模式
    Type m_assetType; //资源类型；

    string m_assetFolder;
    string m_assetFilter;
    Func<string, string> m_getAddress;

    string[] m_assets;
    string[] m_addressNames;
    string[] m_assetsHash;
    public string[] Assets => m_assets;
    public string[] AddressNames => m_addressNames;
    public string GroupName => m_groupName;
    public AddressableGroupSetter.GroupType GroupType => m_groupType;
    public BundlePackingMode PackMode => m_packMode;
    static AddressableAssetSettings CurSettings => AddressableGroupSetter.CurSettings;
    public void CreatGroup()
    {
        if (Assets == null)
        {
            Debug.LogError("Assets==null");
            return;
        }
        AddressableAssetGroup group = CreateGroup(GroupName, GroupType, PackMode, m_assetType);
        for (int i = 0; i < Assets.Length; i++)
        {
            string assetPath = Assets[i];
            string address = AddressNames[i];
            AddAssetEntry(group, assetPath, address);
        }
    }

    #region Addressable 分组设置以及 Schemas
    //创建Group组；
    public static AddressableAssetGroup CreateGroup(string groupName, AddressableGroupSetter.GroupType groupType, BundlePackingMode packMode, Type typeAsset)
    {
#if UNITY_EDITOR_OSX
        if (groupType == AddressableGroupSetter.GroupType.RemoteLogin)
        {
            groupType = AddressableGroupSetter.GroupType.Local;
        }
#endif

        AddressableAssetGroup group = CurSettings.FindGroup(groupName);
        if (group == null)
        {
            List<AddressableAssetGroupSchema> schemas = new List<AddressableAssetGroupSchema>()
            {
                CurSettings.DefaultGroup.Schemas[0],
                CurSettings.DefaultGroup.Schemas[1],
            };
            group = CurSettings.CreateGroup(groupName, false, false, false, schemas, typeAsset);
        }

        // label
        CurSettings.AddLabel(groupName, false);

        // schema
        BundledAssetGroupSchema schemaLoading = (BundledAssetGroupSchema)group.Schemas[1];
        if (groupType == AddressableGroupSetter.GroupType.Local)
        {
            schemaLoading.BuildPath.SetVariableByName(group.Settings, AddressableAssetSettings.kLocalBuildPath);
            schemaLoading.LoadPath.SetVariableByName(group.Settings, AddressableAssetSettings.kLocalLoadPath);
        }
        else
        {
            schemaLoading.BuildPath.SetVariableByName(group.Settings, AddressableAssetSettings.kRemoteBuildPath);
            schemaLoading.LoadPath.SetVariableByName(group.Settings, AddressableAssetSettings.kRemoteLoadPath);
        }

        schemaLoading.Compression = BundleCompressionMode.LZ4;
        schemaLoading.ForceUniqueProvider = false;
        schemaLoading.UseAssetBundleCache = groupType != AddressableGroupSetter.GroupType.Local;
        schemaLoading.UseAssetBundleCrc = false;
        schemaLoading.UseAssetBundleCrcForCachedBundles = false;
        schemaLoading.Timeout = 0;
        schemaLoading.ChunkedTransfer = false;
        schemaLoading.RedirectLimit = -1;
        schemaLoading.RetryCount = 0;
        schemaLoading.BundleMode = packMode;
        schemaLoading.BundleNaming = BundleNamingStyle.NoHash;

        return group;
    }

    //给分组添加资源
    public static AddressableAssetEntry AddAssetEntry(AddressableAssetGroup group, string assetPath, string address)
    {
        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        AddressableAssetEntry entry = group.entries.FirstOrDefault(e => e.guid == guid);
        if (entry == null)
        {
            entry = CurSettings.CreateOrMoveEntry(guid, group, false, false);
        }

        entry.address = address.ToLower();
        entry.SetLabel(group.Name, true, false, false);

        return entry;
    }
    #endregion

    //计算哈希值；
    public void CalcHash()
    {
        if (Assets == null)
        {
            Debug.LogError("Assets == null");
            return;
        }
        m_assetsHash = new string[Assets.Length];
        for (int i = 0; i < Assets.Length; i++)
        {
            //如果使用依赖项的方法计算哈希，热更会扫描出很多不需要的文件；
            //m_assetsHash[i] = EditorHelper.ComputeHashWithDependencies(Assets[i]);
            
            //只计算单个资源的哈希码；
            m_assetsHash[i] = EditorHelper.ComputeHashByAssetpath(Assets[i]);
        }
    }
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

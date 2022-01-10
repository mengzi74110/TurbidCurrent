using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Common
{
    public class BundleConfig : BundleBase<TextAsset>
    {
        static BundleConfig s_single;
        public static BundleConfig Single => s_single ?? new BundleConfig();

        readonly Dictionary<string, TextAsset> m_dic = new Dictionary<string, TextAsset>();

        public override bool IsLoadComplete => m_dic.Count > 0;
        public override string AddressName => "config";

        protected override async void AddressableLoad()
        {
            var assets = await Addressables.LoadAssetsAsync<TextAsset>(AddressName,null).Task;
            m_dic.Clear();
            for (int i = 0; i < assets.Count; i++)
            {
                TextAsset asset = assets[i];
                m_dic[asset.name] = asset;
            }
            OnLoad();
        }

        public TextAsset GetConfig(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("name is empty");
                return null;
            }
            if (m_dic.TryGetValue(name, out TextAsset asset))
                return asset;
            Debug.LogError($"Config file is not exist,fileName : {name}");
            return null;
          
        }
        private BundleConfig() { }
    }
    
}

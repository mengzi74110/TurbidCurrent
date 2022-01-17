using UnityEngine.U2D;
using UnityEngine;
namespace Common
{
    public class BundleAtlas : BundleBase<SpriteAtlas>
    {
        string m_packageName;
        string m_atlasName;
        public override string AddressName
        {
            get
            {
                string address = $"{m_packageName}/{m_atlasName}";
                address = address.ToLower().Replace("\\", "@").Replace("/", "@");
                return address;
            }
        }

        public Sprite GetSprite(string spriteName)
        {
            if (IsLoadComplete)
            {
                return AssetObj.GetSprite(spriteName);
            }
            return null;
        }



        public BundleAtlas(string packName,string atlasName)
        {
            this.m_packageName = packName;
            this.m_atlasName = atlasName;
        }
    }
}

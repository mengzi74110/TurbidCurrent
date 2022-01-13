using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class BundleUIPrefab : BundleBase<GameObject>
    {
        string m_packageName ; //UIPrefab保存的二级文件夹路径；
        string m_uiName ; //UIFlag 
        public override string AddressName
        {
            get
            {
                string address = $"{m_packageName}/{m_uiName}";
                address = address.ToLower().Replace("\\", "@").Replace("/", "@");
                return address;
            }
        }


        public GameObject CloneObj()
        {
            return UnityEngine.Object.Instantiate(AssetObj, null);
        }

        public GameObject CloneObj(Transform parentTrans)
        {
            return UnityEngine.Object.Instantiate(AssetObj, parentTrans);
        }
        public GameObject CloneObj(Transform parentTrans, Vector3 pos, Quaternion qua)
        {
            return UnityEngine.Object.Instantiate(AssetObj, pos, qua, parentTrans);
        }




        public BundleUIPrefab(string packName,string assetName) 
        {
            this.m_packageName = packName;
            this.m_uiName = assetName;
        }
    }
}
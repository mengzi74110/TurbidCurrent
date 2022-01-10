using UnityEngine;
namespace Common
{
    public class BundlePrefab : BundleNormal<GameObject>
    {
        public BundlePrefab(string path) : base(path) { }


        public GameObject CloneObj()
        {
            return UnityEngine.Object.Instantiate(AssetObj, null);
        }

        public GameObject CloneObj(Transform parentTrans)
        {
            return UnityEngine.Object.Instantiate(AssetObj, parentTrans);
        }
        public GameObject CloneObj(Transform parentTrans,Vector3 pos,Quaternion qua)
        {
            return UnityEngine.Object.Instantiate(AssetObj, pos,qua,parentTrans);
        }
    }  
}

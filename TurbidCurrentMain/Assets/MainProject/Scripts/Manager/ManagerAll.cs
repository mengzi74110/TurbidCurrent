using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TurbidCurrent
{

    public class ManagerAll :SingleWithMon<ManagerAll>
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        public void InitAllManager()
        {
            DirectorManager.Instance.CreatManager().transform.SetParent(Instance.transform);
            PostEffectManager.Instance.CreatManager().transform.SetParent(Instance.transform);
            AudioManager.Instance.CreatManager().transform.SetParent(Instance.transform);
            UIManager.Instance.CreatManager().transform.SetParent(Instance.transform);
            LittleRedPointManager.Instance.CreatManager().transform.SetParent(Instance.transform);
            UICoroutine.Instance.CreatManager().transform.SetParent(Instance.transform);
        }
        public void Clear(bool isGC)
        {
            UIManager.Instance.Clear();
            AudioManager.Instance.Clear();
            if (isGC)
            {
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
            }
        }
    }
}
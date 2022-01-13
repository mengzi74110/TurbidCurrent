using System;
using System.Collections.Generic;
using System.Collections;
using Common;
using UnityEngine;
namespace TurbidCurrent
{
    public class UIManager : SingleWithMon<UIManager>
    {
        //还没有销毁的全屏界面；（用于倒退到上一个打开的界面）
        private Stack<UIBase> m_ShowedStack=new Stack<UIBase>();
        //所有存在Hierarchy的UI界面；
        private Dictionary<UIFlag, UIBase> m_AllUIBaseDic = new Dictionary<UIFlag, UIBase>();
        //正在加载的UI界面；
        private List<UIFlag> cachingUI = new List<UIFlag>();
        //所有加载过的BundleUIPrefab
        private Dictionary<UIFlag, BundleUIPrefab> AssetsDic = new Dictionary<UIFlag, BundleUIPrefab>();

        public void ShowUIAsync(string packName, UIFlag flag,object uiData)
        {     
            UICoroutine.Instance.StartCoroutine(ShowUIFlagSync(packName,flag, uiData));       
        }

        IEnumerator ShowUIFlagSync(string packeName, UIFlag flag,object data)
        {
            if (IsCachingUI(flag))
                yield break;

            //TODO:增加一个方法遮罩整个界面；

            cachingUI.Add(flag);//开始加载UI，加入缓存列表；加载结束从列表中清除；
            BundleUIPrefab asset;
            if (!AssetsDic.TryGetValue(flag, out asset))
            {
                asset = new BundleUIPrefab(packeName,flag.ToString());
                AssetsDic.Add(flag, asset);
            }
          
            bool loaded = false;
            asset.Load(() =>
            {
                loaded = true;
            });
            while (!loaded)
            {
                yield return null;
            }

            GameObject go = asset.CloneObj();
            go.transform.SetAsLastSibling();
            UIBase uibase = go.GetComponent<UIBase>();
            uibase.OnShow(data);

            cachingUI.Remove(flag);

            //TODO:加载完成取消遮罩；
        }

        //当前UI是否正在加载中；
        public bool IsCachingUI(UIFlag flag)
        {
            if (cachingUI != null)
            {
                for (int i = 0; i < cachingUI.Count; i++)
                {
                    if (cachingUI[i] == flag)
                        return true;
                }
            }
            return false;
        }

        //当前UI是否在Hierarchy面板中；
        public bool IsShowingUI(UIFlag flag)
        {
            if (m_AllUIBaseDic.ContainsKey(flag))
            {
                return true;
            }
            return false;
        }
        //退出当前界面，自动显示上一个全屏界面；
        public void GoBack(object data)
        {
            if (m_ShowedStack.Count < 1)
            {
                MDebug.LogError("m_ShowedStack.Count == 0");
                return;

            }
            UIBase currentUI= m_ShowedStack.Pop();
            if (currentUI != null)
            {
                currentUI.OnDestroy();
            }
        }

        //销毁所有UI界面；
        public void Clear(bool isGC)
        {
            //关闭协同程序；
            UICoroutine.instacne.gameObject.SetActive(false);
            foreach (var item in AssetsDic.Values)
            {
                item.Release();
            }
            AssetsDic.Clear();

            m_ShowedStack.Clear();
            cachingUI.Clear();
            foreach (var uibase in m_AllUIBaseDic.Values)
            {
                uibase.OnDestroy();
            }
            m_AllUIBaseDic.Clear();
            if (isGC)
                GC.Collect();
        }

        public void RegistDic(UIFlag flag,UIBase uibase)
        {
            if(flag==UIFlag.None && uibase == null)
            {
                MDebug.LogError($"请检查注册进UIManger的UIPanel :{flag.ToString()}");
            }
            if (m_AllUIBaseDic.ContainsKey(flag))
                return;
            m_AllUIBaseDic.Add(flag, uibase);
        }


        public void RemoveDic(UIFlag flag)
        {
            if (m_AllUIBaseDic.ContainsKey(flag))
                m_AllUIBaseDic.Remove(flag);
        }
        public void DestoryUI(UIFlag flag)
        {
            UIBase targetUI;
            if(m_AllUIBaseDic.TryGetValue(flag,out targetUI))
            {
                targetUI.OnDestroy();
            }
            m_AllUIBaseDic.Remove(flag);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Common;

namespace TurbidCurrent
{
    public abstract class UIBase : MonoBehaviour
    {
        protected UIFlag m_uiFlag;
        public UIFlag FlagEnum => m_uiFlag;

        protected ScreenFlag m_screenFlag;
        public ScreenFlag ScreenEnum => m_screenFlag;

        public List<SpriteAtlas> currentAtlasList;//不需要使用list，用什么加载什么就可以了；更换sprite需要指定的atlas；
        public List<BundleAtlas> currentBundleList=new List<BundleAtlas>();

        protected void Awake()
        {           
            SetUIFlag();
            SetScreenFlag();
            UIManager.Instance.RegistDic(m_uiFlag, this);
            OnAwake();
        }

        protected void Start()
        {
            OnStart();
        }
        public virtual bool ActiveSelf
        {
            get
            {
                return gameObject.activeSelf;
            }
            set
            {
                gameObject.SetActive(value);
            }
        }

        public virtual void OnShow(object data)
        {
            if (m_uiFlag != UIFlag.RedPoint)
            {
                if (!ActiveSelf)
                    ActiveSelf = true;
            }
        }
        protected abstract void OnAwake();
        protected virtual void OnStart() { }
        
        protected abstract void SetUIFlag();
        protected abstract void SetScreenFlag();

        public virtual void DestroySelf()
        {
            for (int i = 0; i < currentBundleList.Count; i++)
            {
                currentBundleList[i].Release();
            }
            currentBundleList.Clear();
            currentAtlasList.Clear();
            UIManager.Instance.DestoryUI(this.m_uiFlag);
            UIManager.Instance.RemoveDic(this.m_uiFlag);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurbidCurrent
{
    public abstract class UIBase : MonoBehaviour
    {
        protected UIFlag m_uiFlag;
        public UIFlag FlagEnum => m_uiFlag;

        protected ScreenFlag m_screenFlag;
        public ScreenFlag ScreenEnum => m_screenFlag;

        protected void Awake()
        {
            OnAwake();
            SetUIFlag();
            SetScreenFlag();
            UIManager.Instance.RegistDic(m_uiFlag, this);
        }

        protected void Start()
        {
            OnStart();
        }
        public void OnDestroy()
        {
            Destroy();
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
            if (!ActiveSelf)
                ActiveSelf = true;
        }
        protected abstract void OnAwake();
        protected abstract void OnStart();
        
        protected abstract void SetUIFlag();
        protected abstract void SetScreenFlag();

        protected virtual void Destroy()
        {
            UIManager.Instance.RemoveDic(this.m_uiFlag);
        }
    }
}
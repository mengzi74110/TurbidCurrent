using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UObject = UnityEngine.Object;

namespace Common
{
    // 加载中的资源，使用 Addressable Asset System
    public abstract class BundleBase<TObject> : IProgressBar
        where TObject : UObject
    {
        protected Action m_onLoaded;

        bool m_isDeleteOnLoad;
        AsyncOperationHandle<TObject>? m_asyncHandle;


        public abstract string AddressName { get; }


        public virtual bool IsLoadComplete => AssetObj;

        public TObject AssetObj
        {
            get;
            protected set;
        }

        public Action OnLoadedEvent
        {
            set
            {
                if (value == null)
                    return;

                if (IsLoadComplete)
                    value();
                else
                    m_onLoaded += value;
            }
        }

        public float PercentComplete
        {
            get
            {
                if (IsLoadComplete)
                    return 1;
                else if (m_asyncHandle != null)
                    return m_asyncHandle.Value.PercentComplete;
                else
                    return 0;
            }
        }


        public void Load()
        {
            m_isDeleteOnLoad = false;
            AddressableLoad();
        }

        // 加载资源
        public void Load(Action callback)
        {
            OnLoadedEvent = callback;
            Load();
        }

        // 异步加载资源
        protected async virtual void AddressableLoad()
        {
            m_asyncHandle = Addressables.LoadAssetAsync<TObject>(AddressName);
            AssetObj = await m_asyncHandle.Value.Task;
            if (!AssetObj)
            {
                UnityEngine.Debug.LogError("异步加载AddressableLoad: AssetObj await");
            }
            OnLoad();
            m_asyncHandle = null;
        }

        protected virtual void AddressableRelease()
        {
            if (AssetObj)
                Addressables.Release<TObject>(AssetObj);
        }

        protected void OnLoad()
        {
            // 加载完毕
            if (!m_isDeleteOnLoad)
                OnLoadSucceed();
            else
                Release();
        }

        protected virtual void OnLoadSucceed()
        {
            if (m_onLoaded != null)
            {
                m_onLoaded();
                m_onLoaded = null;
            }
        }

        // 销毁资源
        public void Release()
        {
            if (IsLoadComplete)
                AddressableRelease();
            else
                m_isDeleteOnLoad = true;
        }

    }
}

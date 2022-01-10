using UnityEngine;

namespace TurbidCurrent
{
    //导演类基类
    public abstract class DirectorBase 
    {
        public void EnterDierctor()
        {
            Awake();
            OnStart();
        }
        //进入导演类最先执行的方法；
        public void Awake()
        {
            OnAwake();
        }
        //其次执行的方法
        public void Start()
        {
            OnStart();
        }

        //刷新的方法
        public void Update()
        {
            OnUpdate();
        }

        //退出导演类执行的方法；
        public void ExitDirector()
        {
            OnExite();
        }

        protected abstract void OnAwake();
        protected abstract void OnStart();
        protected abstract void OnUpdate();
        protected abstract void OnExite();

    }
}
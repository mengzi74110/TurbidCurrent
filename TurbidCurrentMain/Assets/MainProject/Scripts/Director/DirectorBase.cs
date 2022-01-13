using UnityEngine;

namespace TurbidCurrent
{
    //导演类基类
    public abstract class DirectorBase 
    {
        public void EnterDierctor()
        {
            OnAwake();
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
            OnExit();
        }
        //进入导演类最先执行的方法；
        protected abstract void OnAwake();
        //其次执行的方法
        protected abstract void OnStart();
        protected abstract void OnExit();
        protected abstract void OnUpdate();

    }
}
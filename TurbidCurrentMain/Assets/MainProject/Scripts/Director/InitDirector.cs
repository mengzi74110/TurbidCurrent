using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TurbidCurrent
{
    //进入游戏的第一个导演类，初始化游戏
    public class InitDirector : DirectorBase
    {
        protected override void OnAwake()
        {
            ManagerAll.Instance.InitAllManager();
        }

        protected override void OnStart()
        {
            UIManager.Instance.ShowUIAsync("Logo", UIFlag.UI_LogoWnd, null);
            LittleRedPointUI.LoadRedPointUI();
        }

        protected override void OnExit()
        {
           
        }

        protected override void OnUpdate()
        {
           
        }
    }
}

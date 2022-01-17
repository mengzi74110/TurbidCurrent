using System;
using UnityEngine;
using System.Collections;
using Common;

namespace TurbidCurrent
{
    public class LittleRedPoint
    {
        Func<bool> m_condition;

        LittleRedPointUI littleRed;

        public LittleRedPoint(Transform parent, Vector3 localPos,Func<bool> condition)
        {
            littleRed = LittleRedPointUI.CreatRedPoint(parent,localPos);
            this.m_condition = condition;
        }

        public void RefreshRedPoint()
        {
            if (littleRed.transform.parent.gameObject.activeInHierarchy)
            {
                MDebug.Log("TODO:游戏某些系统需要增加等级或者条件限定，系统不开启，红点不显示");
                bool showRed = m_condition();  //增加游戏的系统开启的条件；
                littleRed.IsShow = showRed;
            }
        }
        public bool Exist()
        {
            return littleRed;
        }
    }
}

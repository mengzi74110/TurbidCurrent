using System.Collections.Generic;
using UnityEngine;
using Common;
namespace TurbidCurrent
{
    public class LittleRedPointUI : UIBase
    {
        [SerializeField]
        Sprite m_RedIcon;

        Transform m_parent;

        public bool IsShow
        {
            get
            {
                return ActiveSelf;
            }
            set
            {
                ActiveSelf = value;
            }
        }
        protected override void OnAwake()
        {
            
        }

        protected override void OnStart()
        {
            
        }

        protected override void SetScreenFlag()
        {
            
        }

        protected override void SetUIFlag()
        {
            
        }

        //加载小红点模板
        public static void LoadPrefab()
        {
            BundleUIPrefab redPointTemplate = new BundleUIPrefab("", "");
        }
    }
}
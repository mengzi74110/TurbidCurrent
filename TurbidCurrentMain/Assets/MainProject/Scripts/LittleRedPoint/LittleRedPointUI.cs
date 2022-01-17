using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;
using UnityEngine.U2D;
namespace TurbidCurrent
{
    public class LittleRedPointUI : UIBase
    {
        [SerializeField]
         Image m_RedIcon;
         
        static LittleRedPointUI templateLittleRedPoint;

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
            if (m_RedIcon == null)
            {
                m_RedIcon = gameObject.GetComponent<Image>();
            }
            IsShow = false;
        }

        protected override void OnStart()
        {
            IsShow = false;
        }

        protected override void SetScreenFlag()
        {
            this.m_screenFlag = ScreenFlag.RedPoint;
        }

        protected override void SetUIFlag()
        {
            this.m_uiFlag = UIFlag.RedPoint;
        }


     

        public void SetLocalPosition(Vector3 localPos)
        {
            transform.localPosition = localPos;
        }
        public void SetLocalScale(Vector3 scale)
        {
            transform.localScale = scale;
        }
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
        public void SetQuation(Quaternion quaternion)
        {
            transform.rotation = quaternion;
        }
        public void SetRedSprite(Sprite redSprite)
        {
            m_RedIcon.sprite = redSprite;
        }
        //游戏初始化只加载一次以后，不在销毁；
        public static void LoadRedPointUI()
        {
            BundleUIPrefab redPointBundle = new BundleUIPrefab("Common", "RedPointTemplate");
            redPointBundle.Load(() =>
            {
                GameObject template = redPointBundle.CloneObj();
                templateLittleRedPoint = template.GetComponent<LittleRedPointUI>();
                template.name = "[LittleRedPoint]_Template";
                MDebug.Log("TODO:需要设置红点模板的父物体Canvas");
            });
        }

        public static LittleRedPointUI CreatRedPoint(Transform parent,Vector3 localPos)
        {
            LittleRedPointUI target= Instantiate<GameObject>(templateLittleRedPoint.gameObject).GetComponent<LittleRedPointUI>();
            target.SetParent(parent);
            target.gameObject.name = "[LittleRedPoint]";
            target.SetLocalPosition(localPos);
            return target;
        }
        public static LittleRedPointUI CreatRedPoint(Vector3 localPos, Vector3 localScal, Transform parent, Quaternion quaternion)
        {
            LittleRedPointUI target = Instantiate<GameObject>(templateLittleRedPoint.gameObject).GetComponent<LittleRedPointUI>();
            target.SetLocalPosition(localPos);
            target.SetLocalScale(localScal);
            target.SetParent(parent);
            target.SetQuation(quaternion);
            target.gameObject.name = "[LittleRedPoint]";
            return target;
        }

    }
}
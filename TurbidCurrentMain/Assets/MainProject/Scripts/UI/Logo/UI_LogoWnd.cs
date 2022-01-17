using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace TurbidCurrent
{
    public class UI_LogoWnd : UIBase
    {
        [SerializeField]
        private Text m_contentUp;
        [SerializeField]
        private Text m_contextDown;
        [SerializeField]
        private Image m_GameLogo;
        [SerializeField]
        private Image m_BgImage;
        Sequence m_quence;

        IEnumerator m_IEShowTVImage;

        protected override void OnAwake()
        {
            InitProperty();
        }

        public override void OnShow(object data)
        {
            base.OnShow(data);
            InitTweenSequence();
        }
        private void InitProperty()
        {
            AudioManager.Instance.LoadAudioClip(AudioSourEnum.BackGround, "DefaultBg");//应该提前加载，否则在加载途中删除就会出问题；
            m_GameLogo.transform.localScale = new Vector3(1, 0, 1);
            m_contentUp.color = Color.black;
            m_contentUp.text = "";
            m_contextDown.color = Color.black;
            m_contextDown.text = string.Empty;
            m_quence = null;
            m_quence = DOTween.Sequence();
            m_IEShowTVImage = ShowTVImage();
        }
        private void InitTweenSequence()
        {
            m_quence.Append(m_contentUp.DOText("愿每一份付出都有回报", 2.0f));
            m_quence.Append(m_contextDown.DOText("愿每一份善良都被温柔相待", 2.2f));
            m_quence.onComplete = (() =>
              {
                  m_contentUp.DOFade(0, 2.0f);
                  m_contextDown.DOFade(0, 2.0f);
              });
            m_quence.PlayForward();
            if (m_IEShowTVImage != null)
                StartCoroutine(m_IEShowTVImage);
        }

        IEnumerator ShowTVImage()
        {
            yield return new WaitForSeconds(7.0f);
            m_GameLogo.transform.DOScaleY(1, 1.0f);
            yield return new WaitForSeconds(1.0f);
            PostEffectManager.Instance.GetFromDic(ShaderEnum.Interference_PE).SetActive = false;
            StopCoroutine(m_IEShowTVImage);
            DestroySelf();
        }
        public override void DestroySelf()
        {
            PostEffectManager.Instance.GetFromDic(ShaderEnum.Interference_PE).SetActive = false;
            base.DestroySelf();
        }
        protected override void SetUIFlag()
        {
            this.m_uiFlag = UIFlag.UI_LogoWnd;
            
        }

        protected override void SetScreenFlag()
        {
            this.m_screenFlag = ScreenFlag.FullScreen;
        }

      
    }
}
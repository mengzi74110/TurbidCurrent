using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
namespace TurbidCurrent
{
    public class LoadingPanelWnd :MonoBehaviour
    {
        [SerializeField]
        private Image m_LoadingBg;
        [SerializeField]
        private Text m_ProgressNum;
        [SerializeField]
        private RawImage m_UVMotionImage;
        [SerializeField]
        private Slider m_SliderBar;

        BundleNormal<Sprite> bgBundle;
        BundleNormal<Texture> uvmotionBundle; 


        private void Awake()
        {
            if (m_LoadingBg == null)
            {
                m_LoadingBg = transform.FincChildWithName("BackGround").GetComponent<Image>();
            }
            if (m_ProgressNum == null)
            {
                m_ProgressNum = transform.FincChildWithName("Progress").GetComponent<Text>();
            }
            if (m_UVMotionImage == null)
            {
                m_UVMotionImage = transform.FincChildWithName("Handle").GetComponent<RawImage>();
            }
            if (m_SliderBar == null)
            {
                m_SliderBar = transform.FincChildWithName("LoadingSlider").GetComponent<Slider>();
            }
            LoadAssetTexture();
            m_ProgressNum.text = "";
            m_SliderBar.value = 0;
        }

        public  void OnShow(float progress)
        {
            m_SliderBar.value = progress;
            m_ProgressNum.text = (progress * 100).ToString() + "%";
        }
        private void LoadAssetTexture()
        {
            if (bgBundle == null)
            {
                bgBundle = new BundleNormal<Sprite>("spritetexture@defaultloadingbg");
            }
            bgBundle.Load(() =>
            {
                m_LoadingBg.sprite = bgBundle.AssetObj;
            });
            if (uvmotionBundle == null)
            {
                uvmotionBundle = new BundleNormal<Texture>("effectimage@eff_buffer");
            }
            uvmotionBundle.Load(() =>
            {
                m_UVMotionImage.texture = uvmotionBundle.AssetObj;
            });
        }
        private void OnDestroy()
        {
            if (bgBundle != null)
            {
                bgBundle.Release();
                bgBundle = null;
                m_LoadingBg.sprite = null;
            }
            if (uvmotionBundle != null)
            {
                uvmotionBundle.Release();
                uvmotionBundle = null;
                m_UVMotionImage.texture = null;
            }

        }

    }
}

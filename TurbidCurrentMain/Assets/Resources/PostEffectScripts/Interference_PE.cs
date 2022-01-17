using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurbidCurrent
{

    public class Interference_PE : PostEffectBase
    {
        public override string ShaderName => "MyShader/PostEffect/Interference";

        [Range(-2, 2)]
        public float Distortion = 0.0f;
        [Range(0.01f, 1)]
        public float Scale = 1.0f;
        [Range(-1, 1)]
        public float Brightness = 0.15f;
        [Range(0, 2)]
        public float Saturation = 1;
        [Range(0, 2)]
        public float Contrasrt = 1;
        [Range(0, 10)]
        public float VignetteFalloff = 1;
        [Range(0, 100)]
        public float VignetteIntensity = 1.5f;
        [Range(0, 10)]
        public float NoiseAmount = 2.5f;

        private float RandomValue;
        public Color Tint = Color.white;
        [SerializeField]
        private Texture2D m_noise;

        public Texture2D Noise
        {
            get
            {
                if (m_noise == null)
                {
                    m_noise = Resources.Load<Texture2D>("Shader/ShaderRenderTexture/NightVisionNoise");
                }
                return m_noise;
            }
        }
        private void Awake()
        {
            if (Noise == null)
            {
                MDebug.Log("Shader/ShaderRenderTexture/NightVisionNoise");
                m_noise = Resources.Load<Texture2D>("Shader/ShaderRenderTexture/NightVisionNoise");
            }
        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (Mat)
            {
                Mat.SetFloat("_Distortion", Distortion);
                Mat.SetFloat("_Scale", Scale);

                Mat.SetFloat("_Brightness", Brightness);
                Mat.SetFloat("_Saturation", Saturation);
                Mat.SetFloat("_Contrast", Contrasrt);

                Mat.SetColor("_Tint", Tint);

                Mat.SetFloat("_VignetteFalloff", VignetteFalloff);
                Mat.SetFloat("_VignetteIntensity", VignetteIntensity);
            
                if (Noise != null)
                {
                    Mat.SetTexture("_Noise", Noise);
                    Mat.SetFloat("_NoiseAmount", NoiseAmount);
                    Mat.SetFloat("_RandomValue", RandomValue);
                }
                else
                {
                    MDebug.Log("Noise is Null");
                }

                Graphics.Blit(source, destination, Mat);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
        private void Update()
        {
            //随机生成范围中的数值
            RandomValue = Random.Range(-3.14f, 3.14f);
        }
    }
}
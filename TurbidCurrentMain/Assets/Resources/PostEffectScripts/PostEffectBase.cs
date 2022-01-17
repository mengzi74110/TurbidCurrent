using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurbidCurrent
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public abstract class PostEffectBase : MonoBehaviour
    {
        private Material m_ShaderMaterial;
        public Shader m_Shader;
        public abstract string ShaderName { get; }
        protected Material Mat
        {
            get
            {
                if (m_ShaderMaterial == null)
                {
                    m_Shader = Shader.Find(ShaderName);
                    if (m_Shader && m_Shader.isSupported)
                    {
                        m_ShaderMaterial = new Material(m_Shader);
                        m_ShaderMaterial.hideFlags = HideFlags.HideAndDontSave;
                    }
                    else
                    {
                        MDebug.LogError($"{ShaderName} is not Supported");
                        NotSupported();
                    }
                }
                return m_ShaderMaterial;
            }
        }

        protected void NotSupported()
        {
            enabled = false;
        }
        public virtual bool SetActive
        {
            get => enabled;
            set
            {
                enabled = value;
            }
        }


    }
}

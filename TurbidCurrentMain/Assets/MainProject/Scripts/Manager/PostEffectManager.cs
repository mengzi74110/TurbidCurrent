using System;
using System.Collections.Generic;
using UnityEngine;
namespace TurbidCurrent
{
    /// <summary>
    /// 后效脚本跟随相机，相机不销毁；
    /// </summary>
    public class PostEffectManager : SingleWithMon<PostEffectManager>
    {
        private Dictionary<ShaderEnum, PostEffectBase> m_PostEffectDic = new Dictionary<ShaderEnum, PostEffectBase>();

        public PostEffectBase GetFromDic(ShaderEnum shaderEnum)
        {
            if (m_PostEffectDic.ContainsKey(shaderEnum))
                return m_PostEffectDic[shaderEnum];
            else
                MDebug.LogError($"{shaderEnum.ToString()} is not exist！！！");
            return null;
        }
        public void RegisterPEDic(string peName, PostEffectBase pe)
        {
            MDebug.Log(peName);
            ShaderEnum shaderEnum = (ShaderEnum)Enum.Parse(typeof(ShaderEnum), peName);
            //要么在PEBase中直接注册到字典中；
            //PostEffectBase 脚本中增加枚举属性，直接调用；或者直接字符串转枚举；
            if (!m_PostEffectDic.ContainsKey(shaderEnum))
            {
                m_PostEffectDic.Add(shaderEnum, pe);
            }
        }
    }
}
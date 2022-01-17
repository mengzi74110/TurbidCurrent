using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TurbidCurrent
{
    //枚举需要和脚本名字保持一致；
    public enum ShaderEnum
    {
        None = 0,
        Interference_PE = 1,
        Snow_PE = 2,
        Fog_PE = 3,
        OutLine,
        OutLine_PE,

    }

    public class ShaderHelper
    {
        public static string SwitchShaderEnum(ShaderEnum shaderEnum)
        {
            string shaderPath = shaderEnum switch
            {
                ShaderEnum.None => string.Empty,
                ShaderEnum.Interference_PE => "MyShader/PostEffect/Interference",
                _ => throw new System.ArgumentException(shaderEnum.ToString())
            };
            return shaderPath;
        }
    }
}
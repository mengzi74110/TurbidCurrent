using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DebugEnum
{
    Normal=1,
    NetWork=2,
}
//配置文件增加控制是否显示Debug变量，用于真机调试；
public static class MDebug 
{

    public static bool isShowDebug = true;

    public static bool isShowNetDebug = true;

    public static void Log(object content, DebugEnum debugEnum = DebugEnum.Normal)
    {
        if (isShowDebug)
        {
            if (isShowNetDebug)
            {
                string colHtmlString = SwitchDebugEnum(debugEnum);
                string finalContent = $"<color=#{colHtmlString}>{content}</color>";
                Debug.Log(finalContent);
            }
        }
    }

    public static void LogError(object content, DebugEnum debugEnum = DebugEnum.Normal)
    {
        if (isShowDebug)
        {
            if (isShowNetDebug)
            {
                string colHtmlString = SwitchDebugEnum(debugEnum);
                string finalContent = $"<color=#{colHtmlString}>{content}</color>";
                Debug.Log(finalContent);
            }
        }
    }
    public static void LogWarning(object content, DebugEnum debugEnum = DebugEnum.Normal)
    {
        if (isShowDebug)
        {
            if (isShowNetDebug)
            {
                string colHtmlString = SwitchDebugEnum(debugEnum);
                string finalContent = $"<color=#{colHtmlString}>{content}</color>";
                Debug.Log(finalContent);
            }
        }
    }


    private static string SwitchDebugEnum(DebugEnum d)
    {
        switch (d)
        {
            case DebugEnum.NetWork:
                return ColorUtility.ToHtmlStringRGB(Color.green);
            case DebugEnum.Normal:
            default:
                return ColorUtility.ToHtmlStringRGB(Color.white);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//配置文件增加控制是否显示Debug变量，用于真机调试；
public static class MDebug 
{
    public static bool isShowDebug = true;

    public static void Log(object content)
    {
        if (isShowDebug)
        {
            Debug.Log(content);
        }
    }

    public static void LogError(object content)
    {
        if (isShowDebug)
        {
            Debug.LogError(content);
        }
    }
    public static void LogWarning(object content)
    {
        if (isShowDebug)
        {
            Debug.LogWarning(content);
        }
    }
}

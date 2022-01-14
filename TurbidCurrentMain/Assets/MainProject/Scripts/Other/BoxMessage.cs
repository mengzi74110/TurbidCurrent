using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurbidCurrent
{
    internal static class MessagerInternal
    {
        public static Dictionary<string, Delegate> m_DicMessage=new Dictionary<string, Delegate>();
    }

    public class BoxMessage
    {
        private static Dictionary<string, Delegate> m_DicMessage=MessagerInternal.m_DicMessage;

        //禁止注册同名消息；
        public static void RegisterMessage(string messageName, CallBack callBack)
        {
            if (m_DicMessage.ContainsKey(messageName))
            {
                MDebug.LogError($"{messageName}  消息注册的两次！");
                return;
            }
            m_DicMessage[messageName] = callBack;
        }

        public static void DispenseMessage(string messageName)
        {
            if (m_DicMessage.ContainsKey(messageName))
            {
                CallBack callBack = m_DicMessage[messageName] as CallBack;
                if (callBack != null)
                    callBack();
                m_DicMessage.Remove(messageName);
            }
         
        }

        public static void RemoveMessage(string messageName)
        {
            if (m_DicMessage.ContainsKey(messageName))
            {
                m_DicMessage.Remove(messageName);
            }
        }
    }

    public class BoxMessage<T>
    {
        private static Dictionary<string, Delegate> m_DicMessage=MessagerInternal.m_DicMessage;

        //禁止注册同名消息；
        public static void RegisterMessage(string messageName, Callback<T> callBack)
        {
            if (m_DicMessage.ContainsKey(messageName))
            {
                MDebug.LogError($"{messageName}  消息注册的两次！");
                return;
            }
            m_DicMessage[messageName] = callBack;
        }

        public static void DispenseMessage(string messageName,T parameter)
        {
            Delegate d ;
            if (m_DicMessage.TryGetValue(messageName,out d))
            {       
                Callback<T> callBack =d as Callback<T>;
                if (callBack != null)
                    callBack(parameter);
                m_DicMessage.Remove(messageName);
            }
            else
            {
                MDebug.LogError($" MessageName:{messageName} don't Regist In Dictionary!");
            }
        }

        public static void RemoveMessage(string messageName)
        {
            if (m_DicMessage.ContainsKey(messageName))
            {
                m_DicMessage.Remove(messageName);
            }
        }
    }
}

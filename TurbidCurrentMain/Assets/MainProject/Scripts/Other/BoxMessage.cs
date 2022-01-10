using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurbidCurrent
{
    public class BoxMessage
    {
        private static Dictionary<string, Delegate> m_DicMessage;

        //禁止注册同名消息；
        public static void RegisterMessage(string messageName, CallBack callBack)
        {
            if (m_DicMessage == null)
                m_DicMessage = new Dictionary<string, Delegate>();
            if (m_DicMessage.ContainsKey(messageName))
            {
                Debug.LogError($"{messageName}  消息注册的两次！");
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
    }
}

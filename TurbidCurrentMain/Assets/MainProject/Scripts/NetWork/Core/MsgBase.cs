using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace TurbidCurrent.NetWork
{
    /// <summary>
    /// 协议规则：
    /// 1：协议名 与类名相同
    /// 2:
    /// </summary>
    public class MsgBase 
    {

        /// <summary>
        /// 解码协议名（2字节长度+字符串）
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;
            //必须大于2字节
            if (offset + 2 > bytes.Length)
                return "";
            //读取长度
            Int16 length = (Int16)((bytes[offset + 1]) << 8 | bytes[offset]);
            //长度必须足够
            if (offset + 2 + length > bytes.Length)
                return "";
            //解析
            count = 2 + length;
            string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, length);
            return name;
        }

        //解码
        public static MsgBase Decode(string protoName,byte[] bytes,int offset,int count)
        {
            string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
            MDebug.Log("Debug Decode:" + s, DebugEnum.NetWork);
            MsgBase msgBase =(MsgBase)JsonUtility.FromJson(s,Type.GetType(protoName));
            return msgBase;
        }
    }
}
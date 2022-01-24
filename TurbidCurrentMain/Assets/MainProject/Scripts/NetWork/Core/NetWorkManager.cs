using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;

namespace TurbidCurrent.NetWork
{
    public static class NetWorkManager 
    {
        //定义套接字
        static Socket socket;
        //接收缓冲区
        static ByteArray readBuffer;
        //写入队列
        static Queue<ByteArray> writeQueue;
        //是否正在连接
        static bool isConnecting = false;
        //是否正在关闭
        static bool isClosing = false;

        //消息列表
        static List<MsgBase> msgList = new List<MsgBase>();
        //消息列表数量；
        static int msgCount = 0;
        //每一次Update处理的消息数量；
        readonly static int Max_MESSAGE_FIRE = 10;

        //是否启用心跳包
        public static bool isUsePing = true;
        //心跳间隔时间
        public static int pingInterval = 30;
        //上一次发送Ping 的时间；
        static float lastPingTime = 0;
        //上一次收到Ping 的时间；
        static float lastPongTime = 0;


        //事件枚举
        public enum NetEvent
        {
            ConnectSuccess =1,
            ConnectFail = 2,
            Close = 3,
        }
        #region 事件委托
        //事件委托类型
        public delegate void EventListener(string err);
        //事件监听列表
        private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

        public static void AddEventListener(NetEvent netEvent,EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] += listener;
            }
            else
            {
                eventListeners[netEvent] = listener;
                    
            }
        }
        public static void RemoveEventListener(NetEvent netEvent,EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
                eventListeners[netEvent] -= listener;
        }
        //分发事件
        private static void DispenseEvent(NetEvent netEvent,string err)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent](err);
            }
        }
        #endregion

        #region 消息委托
        //消息委托类型
        public delegate void MsgListener(MsgBase msgBase);
        private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();
        //添加消息监听
        public static void AddMsgListener(string msgName, MsgListener listener)
        {
            //添加
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] += listener;
            }
            //新增
            else
            {
                msgListeners[msgName] = listener;
            }
        }
        //删除消息监听
        public static void RemoveMsgListener(string msgName, MsgListener listener)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] -= listener;
            }
        }
        //分发消息
        private static void DispenseMsg(string msgName, MsgBase msgBase)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName](msgBase);
            }
        }
        #endregion

        public static void Connect(string ip,int port)
        {
            if(socket!=null && socket.Connected)
            {
                MDebug.Log("连接失败，已经连接过了！",DebugEnum.NetWork);
                return;
            }
            if (isConnecting)
            {
                MDebug.Log("连接失败，正在连接中……", DebugEnum.NetWork);
                return;
            }
            //初始化
            InitState();
            socket.NoDelay = true;
            isConnecting = true;
            socket.BeginConnect(ip, port, ConnectCallBack,socket);
        }
        //Connect 回调函数
        private static void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                MDebug.Log("Socket 连接成功！");
                DispenseEvent(NetEvent.ConnectSuccess, "");
                isConnecting = false;
                socket.BeginReceive(readBuffer.bytes, readBuffer.writeIndex, readBuffer.remain, 0, ReceiveCallback, socket);
            }
            catch(SocketException ex)
            {
                MDebug.LogError("Socket 连接失败" + ex.ToString());
                DispenseEvent(NetEvent.ConnectFail, ex.ToString());
                isConnecting = false;
            }
        }
        // Receive 回调函数
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket so = (Socket)ar.AsyncState;
                int count = so.EndReceive(ar);
                readBuffer.writeIndex += count;
                //处理二进制消息；
                OnReceiveData();
                //继续接受数据
                if (readBuffer.remain < 8)
                {
                    readBuffer.MoveBytes();
                    readBuffer.ResetSize(readBuffer.length * 2);
                }
                so.BeginReceive(readBuffer.bytes, readBuffer.writeIndex, readBuffer.remain, 0, ReceiveCallback, so);
            }
            catch(SocketException ex)
            {
                MDebug.Log("Socket Receive 失败！" + ex.ToString());
            }
        }
        //处理接收数据；
        public static void OnReceiveData()
        {
            if (readBuffer.length <= 2)
                return;
            int readIdx = readBuffer.readIndex;
            byte[] bytes = readBuffer.bytes;
            Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
            if (readBuffer.length < bodyLength)
                return;
            readBuffer.readIndex += 2;
            //解析协议相关代码
            int nameCount = 0;
            //数据长度；
            string protoName =MsgBase.DecodeName(readBuffer.bytes,readBuffer.readIndex,out nameCount); 
            if(protoName == "")
            {
                MDebug.Log("解析数据为空字符串", DebugEnum.NetWork);
                return;
            }
            readBuffer.readIndex += nameCount;
            //解析协议体
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = MsgBase.Decode(protoName, readBuffer.bytes, readBuffer.readIndex, bodyCount);
            readBuffer.readIndex += bodyCount;
            readBuffer.CheckAndMoveBytes();
            //添加到消息队列
            lock (msgList)
            {
                msgList.Add(msgBase);
                msgCount++;
            }
            if (readBuffer.length > 2)
            {
                OnReceiveData();
            }

        }

        private static void InitState()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            readBuffer = new ByteArray();
            writeQueue = new Queue<ByteArray>();
            isConnecting = false;
            isClosing = false;
            msgList = new List<MsgBase>();
            msgCount = 0;
            lastPingTime = Time.time;
            lastPongTime = Time.time;
            //监听心跳协议
            if (!msgListeners.ContainsKey("MsgPon"))
            {
                AddMsgListener("MsgPong", OnMsgPong);
            }
        }
        //监听PONG协议
        private static void OnMsgPong(MsgBase msgBase)
        {
            lastPongTime = Time.time;
        }
    }
}
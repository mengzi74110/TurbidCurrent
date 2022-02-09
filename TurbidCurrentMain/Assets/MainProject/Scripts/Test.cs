using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.UI;
using UnityEngine.U2D;
using TurbidCurrent;
using System.Net.Sockets;
using System;
using System.Linq;

public class Test : MonoBehaviour
{
    public Image m_image;
    public Canvas canvasRoot;

    //发送完整数据简单案例：
    byte[] sendBytes = new byte[1024]; //定义发送缓冲区
    int readIdx = 0; //缓冲区偏移值；
    int length = 0;  //缓冲区剩余长度；
    //发送消息
    public void Send()
    {
        sendBytes = new byte[10];
        length = sendBytes.Length; //数据长度；
        readIdx = 0;
        socket.BeginSend(sendBytes, 0, length, 0, SendCallback, socket);
    }
    public void SendCallback(IAsyncResult ar)
    {
        Socket socket = (Socket)ar.AsyncState;
        int count = socket.EndSend(ar);
        readIdx += count;
        length -= count;
        if(length>0)
        {
            socket.BeginSend(sendBytes, readIdx, length, 0, SendCallback, socket);
        }
    }

    ////网络测试；
    Socket socket = null;
    //List<Socket> checkRead=new List<Socket>();
    ////修改为异步或者多线程进行处理
    //public void TestUpdate()
    //{
    //    if (socket == null)
    //        return;
    //    checkRead.Clear();
    //    checkRead.Add(socket);
    //    Socket.Select(checkRead, null, null, 0);
    //    foreach (Socket item in checkRead)
    //    {
    //        byte[] readBuff = new byte[1024];
    //        int count = item.Receive(readBuff);
    //        string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
    //    }
    //    //比如发送
    //    //string str="1111";
    //    //byte[] bodyBytes = System.Text.Encoding.Default.GetBytes(str);
    //    //Int16 length =(Int16)bodyBytes.Length;
    //    //byte[] lenBytes = BitConverter.GetBytes(length);
    //    //if (!BitConverter.IsLittleEndian)
    //    //{
    //    //    lenBytes.Reverse();
    //    //}
    //    //byte[] sendBytes = lenBytes.Concat(bodyBytes).ToArray();
    //    //socket.Send(sendBytes);
    //}

    ////转换数据的大小端
    //int buffCount = 3;
    //byte[] readBuff = new byte[2];
    //public void OnReceiveData()
    //{
    //    if (buffCount <= 2)
    //        return;
    //    Int16 bodyLength = (short)((readBuff[1] << 8) | readBuff[0]);
    //    //消息体，更新缓冲区；
    //    //消息处理/继续读取消息；
    //    //……
    //}
    private void Awake()
    {
        
        MDebug.Log("Hello Green", DebugEnum.NetWork);
        //BoxMessage.RegisterMessage("Debug", () => { Debug.Log("11111"); });
        //BoxMessage<string>.RegisterMessage("MDebug", (string mes) => { Debug.Log(mes); });
        //SpriteAtlas atlas= Resources.Load<SpriteAtlas>("atlas_common");
        //m_image.sprite = atlas.GetSprite("littleRed");
        //UIManager.Instance.ShowUIAsync("Logo", UIFlag.UI_LogoWnd, null);
    }

    void Start()
    {
        //BoxMessage.DispenseMessage("Debug");
        //BoxMessage<string>.DispenseMessage("MDebug", "Hello World");
        //Debug.Log("Encoding GB1312:              "+Encoding.GetEncoding("GB2312").ToString());
        //Debug.Log("Encoding Default:                "+Encoding.Default.ToString());

        //string testPath = "Assets/Editor/Config/ConfigConvert/ConfigConverterSettings_Config.csv";
        //string dirPath = Path.GetDirectoryName(testPath);
        //if (!Directory.Exists(dirPath))
        //    Directory.CreateDirectory(dirPath);
        //if(!Directory.Exists("Assets/Editor/Config/ConfigConvert/Test/11.txt"))
        //    Directory.CreateDirectory("Assets/Editor/Config/ConfigConvert/Test/11.txt");
        //File.WriteAllText("Assets/Editor/Config/ConfigConvert/11.txt", "国人没有发展前途认命吧", Encoding.UTF8);
        //File.WriteAllText("Assets/Editor/Config/ConfigConvert/11.txt", "中国人没有发展前途认命吧", Encoding.UTF8);

    }


    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TurbidCurrent.NetWork
{
    public class PingMsg : MsgBase
    {
        public override string ProtoName => "PingMsg";
    }
    public class PongMsg : MsgBase
    {
        public override string ProtoName => "PongMsg";
    }
}
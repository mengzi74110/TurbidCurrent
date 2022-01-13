using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurbidCurrent
{
    public class UICoroutine : SingleWithMon<UICoroutine>
    {
        private void OnDestroy()
        {
            instacne = null;
        }
        //用于关闭协同程序；
        public bool SetEnable
        {
            get
            {
                return gameObject.activeSelf;
            }
            set
            {
                gameObject.SetActive(value);
            }
        }
    }
}

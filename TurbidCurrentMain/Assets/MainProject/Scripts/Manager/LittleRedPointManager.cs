using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TurbidCurrent
{
    public class LittleRedPointManager : SingleWithMon<LittleRedPointManager>
    {
        bool isRuningRedPoint = true;
        float refreshTimeInterval = 0.5f;
        float timer = 0;

        List<LittleRedPoint> listPoints = new List<LittleRedPoint>();
        List<int> removeList = new List<int>();

        public void RegisterDic(ILittleRedPoint IRedPoint)
        {
            if (IRedPoint == null)
                return;
            var points = IRedPoint.GetLittleRedPoint();
            if (points == null)
                return;
            foreach (LittleRedPoint item in points)
            {
                if (item != null)
                {
                    item.RefreshRedPoint();
                    listPoints.Add(item);
                }
            }
        }
        public void RefreshRedPoints()
        {
            for (int i = 0; i < listPoints.Count; i++)
            {
                LittleRedPoint item = listPoints[i];
                if (item.Exist())
                {
                    item.RefreshRedPoint();
                }
                else
                {
                    removeList.Add(i);
                }
            }

            //移除已经不存在的红点；
            if (removeList.Count > 0)
            {
                for (int i = 0; i < removeList.Count; i++)
                {
                    int index = removeList[i];
                    listPoints.RemoveAt(index);
                }
                removeList.Clear();
            }
        }

        public void Update()
        {
            if (isRuningRedPoint)
            {
                if (timer <=0)
                {
                    timer = refreshTimeInterval;
                    RefreshRedPoints();
                }
                else
                {
                    timer -= Time.deltaTime;
                }

            }
        }

        public void Clear()
        {
            removeList.Clear();
            listPoints.Clear();
        }
    }
}

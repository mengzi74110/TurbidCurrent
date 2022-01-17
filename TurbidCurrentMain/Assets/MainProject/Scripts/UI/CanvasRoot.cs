using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurbidCurrent
{
    public class CanvasRoot 
    {
        private static Canvas m_Canvas;
        private static Camera m_UICamera;
        public static Camera UICamera
        {
            get
            {
                if (m_UICamera==null)
                {
                    GameObject go = new GameObject("UICamera");
                    m_UICamera= go.AddComponent<Camera>();                   
                    InitUICamera(m_UICamera);
                }
                return m_UICamera;
            }
        }
        private static void InitUICamera(Camera cam)
        {
            cam.clearFlags = CameraClearFlags.Depth;
            //设置：裁剪层级；
            cam.cullingMask = 1 << 5;//只渲染UI层级；
            //设置：深度；
            cam.depth = 1;
            cam.farClipPlane = 100;
            //后效相机放在一个总的后效相机上，使用的时候直接调整对应的cullingmask
            Interference_PE pe = cam.gameObject.AddComponent<Interference_PE>();
            PostEffectManager.Instance.RegisterPEDic(typeof(Interference_PE).Name.ToString(), pe);
            pe.enabled = true;
        }
        public static Transform UICanvasRoot
        {
            get
            {
                if (m_Canvas == null)
                {
                    GameObject go = Resources.Load<GameObject>("Prefabs/CanvasRoot");
                    GameObject temp = GameObject.Instantiate<GameObject>(go);
                    temp.name = "Canvas";
                    UnityEngine.GameObject.DontDestroyOnLoad(temp);

                    m_Canvas = temp.GetComponent<Canvas>();
                    m_Canvas.renderMode= RenderMode.ScreenSpaceCamera;
                    m_Canvas.worldCamera = UICamera;
                    UICamera.transform.SetParent(temp.transform);
                }
                return m_Canvas.transform;
            }
        }
    }
}
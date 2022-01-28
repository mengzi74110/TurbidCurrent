using System.Collections.Generic;
using UnityEngine;
namespace TurbidCurrent
{
    public static class ScreenShot
    {
        public static Texture2D ScreenShotTexture2D(Camera m_camera,Rect m_rect)
        {
            RenderTexture rt = new RenderTexture((int)m_rect.width, (int)m_rect.height, 0);
            RenderTexture originRT = m_camera.targetTexture;
            m_camera.targetTexture = rt;
            m_camera.RenderDontRestore();//手动渲染；
            m_camera.targetTexture = originRT;

            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)m_rect.width, (int)m_rect.height);
            screenShot.ReadPixels(m_rect, 0, 0);
            screenShot.Apply();
            GameObject.Destroy(rt);
            RenderTexture.active = null;
            return screenShot;
        }
    }
}

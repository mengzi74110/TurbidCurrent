using UnityEngine;

namespace TurbidCurrent
{
    public class SingleWithMon<T>:MonoBehaviour  where T :MonoBehaviour,new()
    {
        protected static T instacne;
        public static T Instance
        {
            get
            {
                if (instacne == null)
                {
                    GameObject go = new GameObject(typeof(T).ToString());
                    instacne = go.AddComponent<T>();
                    GameObject.DontDestroyOnLoad(go);
                }
                return instacne;
            }
        }
        protected SingleWithMon() { }
    }
}

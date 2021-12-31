using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class Test : MonoBehaviour
{
    
    void Start()
    {
        Debug.Log("Encoding GB1312:              "+Encoding.GetEncoding("GB2312").ToString());
        Debug.Log("Encoding Default:                "+Encoding.Default.ToString());
    }

 
    void Update()
    {
       
    }
}

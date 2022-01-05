using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class Test : MonoBehaviour
{
    
    
    void Start()
    {
        //Debug.Log("Encoding GB1312:              "+Encoding.GetEncoding("GB2312").ToString());
        //Debug.Log("Encoding Default:                "+Encoding.Default.ToString());

        string testPath = "Assets/Editor/Config/ConfigConvert/ConfigConverterSettings_Config.csv";
        string dirPath = Path.GetDirectoryName(testPath);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
    }

 
    void Update()
    {
       //测试UTF-8 的文件转换功能；
    }
}

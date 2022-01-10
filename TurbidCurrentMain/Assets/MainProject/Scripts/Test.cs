using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using TurbidCurrent;
public class Test : MonoBehaviour
{
    private void Awake()
    {
        BoxMessage.RegisterMessage("Debug", () => { Debug.Log("11111"); });
        BoxMessage.RegisterMessage("Debug", () => { Debug.Log("11111"); });

    }

    void Start()
    {
        BoxMessage.DispenseMessage("Debug");
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ConfigConverterSettings : EditorWindow
{   
    string m_sourceConfigFolder;
    List<Item> m_items;
    Vector2 m_scrollPos;
    bool m_allToLua;
    bool m_allToAsset;
    bool m_allIsHorizontal;

    List<Item> m_showingItems;
    bool m_isOpenAllFunc;
    string m_strSearchWords;


    static string SourceConfigFolderKey
    {
        get { return "ConfigConverter_SourceConfigFolderKey_" + Application.dataPath; }
    }

    static string SourceConfigFolder
    {
        get { return PlayerPrefs.GetString(SourceConfigFolderKey); }
        set
        {
            PlayerPrefs.SetString(SourceConfigFolderKey, value);
            PlayerPrefs.Save();
        }
    }


    public class Item
    {
        public string Name;
        public bool IsToLua;
        public bool IsToTable;
        public bool IsHorizontal;
        public bool IsGenerateCSharp;
        public string FullPath;

        public string NameNoExt
        {
            get { return Path.GetFileNameWithoutExtension(Name); }
        }

        public Item(string folder, string name)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentException("folder");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            Name = name;
            FullPath = Path.Combine(folder, name);
        }

        public Item(string folder, string name, bool toLua, bool toTable, bool isHorizontal)
            : this(folder, name)
        {
            IsToLua = toLua;
            IsToTable = toTable;
            IsHorizontal = isHorizontal;
        }
    }


    public static List<Item> ReadSettings(out string configFolder)
    {
        configFolder = GetSourceConfigFolder();

        List<Item> list = new List<Item>();
        string path =AllEditorPathConfig.SettingFilePath;

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string[] words = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string name = words[0];
                    bool toLua = bool.Parse(words[1]);
                    bool toAsset = bool.Parse(words[2]);
                    bool isHor = bool.Parse(words[3]);
                    Item item = new Item(configFolder, name, toLua, toAsset, isHor);
                    list.Add(item);
                }
            }
        }

        return list;
    }

    [MenuItem("CustomToolbar/Config/Setting")]
    public static void CreateWindow()
    {
        var window = EditorWindow.GetWindow<ConfigConverterSettings>(false, "Config converter settings", true);
        window.Reset();
    }

    public static string GetSourceConfigFolder()
    {
        string folder = SourceConfigFolder;
        while (true)
        {
            bool isValid = !string.IsNullOrEmpty(folder) && Directory.Exists(folder);
            if (isValid)
            {
                SourceConfigFolder = folder;
                break;
            }
            folder = EditorUtility.SaveFolderPanel("请选择配置表所在目录...", folder, folder);
        }

        return folder;
    }

    void Reset()
    {
        var oldList = ReadSettings(out m_sourceConfigFolder);

        if (m_items == null)
            m_items = new List<Item>();
        else
            m_items.Clear();

        // get all
        DirectoryInfo dirInfo = new DirectoryInfo(m_sourceConfigFolder);
        FileInfo[] files = dirInfo.GetFiles();

        foreach (var f in files)
        {
            var item = oldList.FirstOrDefault(o => o.Name == f.Name) ?? new Item(m_sourceConfigFolder, f.Name);
            m_items.Add(item);
        }

        // [fb]
        if (m_showingItems == null)
            m_showingItems = new List<Item>();
        else
            m_showingItems.Clear();

        m_showingItems.AddRange(m_items);
    }

    void WriteList()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in m_items)
        {
            if (item.IsToLua || item.IsToTable)
            {
                sb.AppendFormat(",{0},{1},{2},{3}", item.Name, item.IsToLua, item.IsToTable, item.IsHorizontal);
                sb.AppendLine();
            }
            Debug.Log(item.Name);
        }
        PathHelper.CreateDirectory(AllEditorPathConfig.SettingFilePath);
      
        File.WriteAllText(AllEditorPathConfig.SettingFilePath, sb.ToString());
    }

    void OnGUI()
    {
        // 配置文件所在目录
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("设置配置所在目录路径", GUILayout.Width(150)))
        {
            m_sourceConfigFolder = EditorUtility.SaveFolderPanel("请选择配置表所在目录...", "D:/", "D:/");
            SourceConfigFolder = m_sourceConfigFolder;
            Reset();
        }
        GUILayout.TextField(m_sourceConfigFolder, GUILayout.Width(500));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        m_isOpenAllFunc = GUILayout.Toggle(m_isOpenAllFunc, "打开隐藏功能", GUILayout.Width(200));
        if (m_isOpenAllFunc && GUILayout.Button("保存更改", GUILayout.Width(100)))
        {
            WriteList();
            Debug.Log("Save csv settings succeed!");
            EditorUtility.DisplayDialog("csv settings", "Save csv settings succeed!", "ok");
        }
        GUILayout.EndHorizontal();

        DragItems();

        DrawButtons();
    }

    void DragItems()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(208);

        bool oldAllToLua = m_allToLua;
        m_allToLua = GUILayout.Toggle(m_allToLua, "to lua", GUILayout.Width(60));
        if (m_allToLua != oldAllToLua)
        {
            for (int i = 0; i < m_showingItems.Count; i++)
                m_showingItems[i].IsToLua = m_allToLua;
        }

        bool oldAllToAsset = m_allToAsset;
        m_allToAsset = GUILayout.Toggle(m_allToAsset, "to txt", GUILayout.Width(80));
        if (m_allToAsset != oldAllToAsset)
        {
            for (int i = 0; i < m_showingItems.Count; i++)
                m_showingItems[i].IsToTable = m_allToAsset;
        }

        if (m_isOpenAllFunc)
        {
            bool oldAllIsHorizontal = m_allIsHorizontal;
            m_allIsHorizontal = GUILayout.Toggle(m_allIsHorizontal, "is horizontal", GUILayout.Width(100));
            if (m_allIsHorizontal != oldAllIsHorizontal)
            {
                for (int i = 0; i < m_showingItems.Count; i++)
                    m_showingItems[i].IsHorizontal = m_allIsHorizontal;
            }

            GUILayout.Label("is generate c#", GUILayout.Width(100));
        }

        GUILayout.EndHorizontal();

        // 所有的配置文件
        m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
        {
            for (int i = 0; i < m_showingItems.Count; i++)
            {
                var item = m_showingItems[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label(item.Name, GUILayout.Width(200));
                item.IsToLua = GUILayout.Toggle(item.IsToLua, "", GUILayout.Width(60));
                item.IsToTable = GUILayout.Toggle(item.IsToTable, "", GUILayout.Width(80));
                if (m_isOpenAllFunc)
                {
                    item.IsHorizontal = GUILayout.Toggle(item.IsHorizontal, "", GUILayout.Width(100));
                    item.IsGenerateCSharp = GUILayout.Toggle(item.IsGenerateCSharp, "", GUILayout.Width(100));
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
    }

    // [fb]
    void DrawButtons()
    {
        GUILayout.Space(10);
        GUILayout.Space(10);
        GUILayoutOption buttonOption = GUILayout.Width(100);

        GUILayout.BeginHorizontal();
        m_strSearchWords = GUILayout.TextField(m_strSearchWords, GUILayout.Width(200));
        if (GUILayout.Button("搜索", buttonOption))
        {
            Search();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("转化", buttonOption))
        {
            ConfigConverter.ConvertConfig(m_showingItems);
            Debug.Log("Convert config finished");
        }

        if (GUILayout.Button("清空", buttonOption))
        {
            for (int i = 0; i < m_showingItems.Count; i++)
            {
                var item = m_showingItems[i];
                item.IsToLua = false;
                item.IsToTable = false;
            }
        }

        if (GUILayout.Button("反转", buttonOption))
        {
            for (int i = 0; i < m_showingItems.Count; i++)
            {
                var item = m_showingItems[i];
                item.IsToLua = !item.IsToLua;
                item.IsToTable = !item.IsToTable;
            }
        }

        if (GUILayout.Button("还原", buttonOption))
        {
            Reset();
        }

        if (m_isOpenAllFunc && GUILayout.Button("转化c#", buttonOption))
        {
            ConfigConverter.GenerateCSharp(m_showingItems);
        }

        GUILayout.EndHorizontal();
    }

    void Search()
    {
        if (string.IsNullOrEmpty(m_strSearchWords))
        {
            Reset();
            return;
        }

        m_showingItems.Clear();
        m_strSearchWords = m_strSearchWords.ToLower();
        foreach (var item in m_items)
        {
            if (item.Name.ToLower().Contains(m_strSearchWords))
            {
                m_showingItems.Add(item);
            }
        }

    }


}

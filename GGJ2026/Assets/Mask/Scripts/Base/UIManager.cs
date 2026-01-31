using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Test;

namespace Test
{
    public class UIManager
    {
        private static UIManager  _instance;

        private Transform _uiRoot;
    
        private Dictionary<string, string> pathDict;
    
        private Dictionary<string, GameObject> prefabDict;
    
        public Dictionary<string, BasePanel> panelDict;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIManager();
                }
                return _instance;
            }
        }

        public Transform UIRoot
        {
            get
            {
                if (_uiRoot == null)
                {
                    _uiRoot = GameObject.Find("Canvas").transform;
                }
                return _uiRoot;
            }
        }
        private UIManager()
        {
            InitDicts();
        }

        private void InitDicts()
        {
            panelDict = new Dictionary<string, BasePanel>();
            prefabDict = new Dictionary<string, GameObject>();
            pathDict = new Dictionary<string, string>()
            {
                {UIConst.TestPanel, "TestPanel" },
                {UIConst.MainPanel, "MainPanel" },
            };
        }

        public BasePanel OpenPanel(string name)
        {
            BasePanel panel = null;
            if (panelDict.TryGetValue(name, out panel))
            {
                Debug.LogError("界面已打开：" + name);
                return null;
            }

            string path = "";
            if (!pathDict.TryGetValue(name, out path))
            {
                Debug.LogError("界面名称错误：" + name);
                return null;
            }

            GameObject panelPerfab = null;
            if (!prefabDict.TryGetValue(name, out panelPerfab))
            {
                string realPath = "Prefab/Panel/" + path;
                panelPerfab = Resources.Load<GameObject>(realPath) as GameObject;
                prefabDict.Add(name, panelPerfab);
            }
            
            GameObject panelObject = GameObject.Instantiate(panelPerfab, UIRoot, false);
            panel = panelObject.GetComponent<BasePanel>();
            panelDict.Add(name, panel);
            return panel;
        }
    
        public bool ClosePanel(string name)
        {
            BasePanel panel = null;
            if (!panelDict.TryGetValue(name, out panel))
            {
                Debug.LogError("界面未打开：" + name);
                return false;
            }

            panel.ClosePanel();
            return true;
        }
        
    }
    public class UIConst
    {
        public const string TestPanel = "TestPanel";
        public const string MainPanel = "MainPanel";
    }
}


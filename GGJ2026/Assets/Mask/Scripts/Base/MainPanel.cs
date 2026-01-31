using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Test
{
    public class MainPanel : BasePanel
    {
        public Button startButton;
        // Start is called before the first frame update
        void Start()
        {
            //UIManager.Instance.panelDict.Add("MainPanel", this);
            // 获取按钮组件
            Button btn = startButton.GetComponent<Button>();
        
            // 添加点击事件监听
            btn.onClick.AddListener(TaskOnClick);
            Cursor.visible = true;
        }

        void TaskOnClick()
        {
            Debug.Log("You have clicked the button!");
            UIManager.Instance.ClosePanel(UIConst.MainPanel);
        }
        

    }
    
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class Canvas : BasePanel
    {
        // Start is called before the first frame update
        void Start()
        {
            UIManager.Instance.OpenPanel(UIConst.MainPanel);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}


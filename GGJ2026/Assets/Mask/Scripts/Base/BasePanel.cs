using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Test;

namespace Test
{
    public class BasePanel : MonoBehaviour
    {
        protected new string name;
        protected bool isRemove = false;
        public virtual void OpenPanel(string name)
        {
            this.name = name;
            gameObject.SetActive(true);
        }

        public virtual void ClosePanel()
        {
            isRemove =  true;
            gameObject.SetActive(false);
            Destroy(gameObject);
            /**if (UIManager.Instance.panelDict.ContainsKey(name))
            {
                UIManager.Instance.panelDict.Remove(name);
            }**/
        }
    }
}


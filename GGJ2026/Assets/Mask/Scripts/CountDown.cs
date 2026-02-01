using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    public string contextText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(float cd)
    {
        this.gameObject.SetActive(true);
        this.gameObject.GetComponent<TextMeshPro>().SetText(Mathf.RoundToInt(cd) + contextText);
    }

    public void ClearText()
    {
        this.gameObject.SetActive(false);
        this.gameObject.GetComponent<TextMeshPro>().SetText("");
    }
}

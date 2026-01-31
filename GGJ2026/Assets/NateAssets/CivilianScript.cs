using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianScript : MonoBehaviour
{
    [SerializeField] private GameObject outlookPool;
    // Start is called before the first frame update
    void Start()
    {
        int randomNumber = Random.Range(0,outlookPool.transform.childCount);
        outlookPool.transform.GetChild(0).gameObject.SetActive(false);
        outlookPool.transform.GetChild(randomNumber).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

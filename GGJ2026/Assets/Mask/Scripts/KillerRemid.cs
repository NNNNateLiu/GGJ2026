using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class KillerRemind : MonoBehaviour
{
    // Start is called before the first frame update
    private ParticleSystem ps;
    private ParticleSystemRenderer renderer;
    private Material material;
    private Color color;
    private float alpha = 0f;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        renderer = GetComponent<ParticleSystemRenderer>();

        material = renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            IncreaseEffect();
        }
    }

    public void IncreaseEffect()
    {
        alpha += 0.2f;
        material.SetColor("_TintColor",new Color(1,0,0,alpha));
    }

    public void ClearEffect()
    {
        material.SetColor("_TintColor",new Color(1,0,0,0));
    }
}

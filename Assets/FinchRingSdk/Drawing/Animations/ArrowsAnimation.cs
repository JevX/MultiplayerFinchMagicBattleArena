using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsAnimation : MonoBehaviour
{
    public Material Material;

    public string ColorField;

    public Color Color;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Material.SetColor(ColorField, Color.linear);
    }
}

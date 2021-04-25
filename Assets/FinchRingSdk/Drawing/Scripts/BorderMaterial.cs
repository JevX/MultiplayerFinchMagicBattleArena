using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BorderState
{
    ButtonOn,
    ButtonHover,
    ButtonOff
}

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class BorderMaterial : MonoBehaviour
{
    [Range(0, 0.5f)]
    public float BorderRadius = 0.5f;
    [Range(0, 1f)]
    public float BorderWidth = 0.1f;

    public Color ButtonColorOn = Color.white;
    public Color ButtonColorHover = new Color(140 / 255f, 140 / 255f, 140 / 255f, 200 / 255f);
    public Color ButtonColorOff = new Color(0, 0, 0, 0);

    public Shader BorderShader;

    public bool LeftUpCorner = true;
    public bool LeftDownCorner = true;
    public bool RightUpCorner = true;
    public bool RightDownCorner = true;

    public BorderState State = BorderState.ButtonOff;
    private Image sprite;
    private RectTransform rect;

    void Start()
    {
        sprite = GetComponent<Image>();
        sprite.material = new Material(BorderShader);
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        float max = Mathf.Max(rect.sizeDelta.x, rect.sizeDelta.y);

        if (max > 0)
        {
            float x = rect.sizeDelta.x / max;
            float y = rect.sizeDelta.y / max;

            sprite.material.SetFloat("_RatioX", x);
            sprite.material.SetFloat("_RatioY", y);
        }

        sprite.material.SetFloat("_RadiusAlpha", BorderRadius);
        sprite.material.SetFloat("_BorderWidth", BorderWidth);

        sprite.material.SetFloat("_LeftUpCorner", LeftUpCorner ? 1 : 0);
        sprite.material.SetFloat("_LeftDownCorner", LeftDownCorner ? 1 : 0);
        sprite.material.SetFloat("_RightUpCorner", RightUpCorner ? 1 : 0);
        sprite.material.SetFloat("_RightDownCorner", RightDownCorner ? 1 : 0);

        switch (State)
        {
            case BorderState.ButtonOff:
                sprite.material.SetColor("_ButtonColor", ButtonColorOff);
                break;

            case BorderState.ButtonOn:
                sprite.material.SetColor("_ButtonColor", ButtonColorOn);
                break;

            case BorderState.ButtonHover:
                sprite.material.SetColor("_ButtonColor", ButtonColorHover);
                break;
        }
    }
}

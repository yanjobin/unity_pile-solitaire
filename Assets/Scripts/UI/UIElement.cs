using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviour
{
    private float alphaThreshold = 1.0f;

    public GameObject MainImage;
    public GameObject GlowImage;

    private Vector3 hoveredScale = new Vector3(1.07f, 1.07f, 1f);
    private Vector3 glowImageScale = new Vector3(1.2f, 1.2f, 1f);

    private bool hovered = false;

    private bool selected = false;
    public bool Selected
    {
        set
        {
            selected = value;
        }
    }

    void Start()
    {
        GlowImage.transform.localScale = glowImageScale;
    }

    void Update()
    {
        GetComponent<Glow>().CancelGlow();
        if (MainImage.GetComponent<Image>().color.a >= alphaThreshold)
        {
            if (selected)
                GetComponent<Glow>().ShowGlow(Glow.COLOR_UI_SELECTED, 0);
            else if (hovered)
                GetComponent<Glow>().ShowGlow(Glow.COLOR_UI_HOVER, 0);
        }
    }

    public void OnHoverEnter()
    {
        if (MainImage.GetComponent<Image>().color.a >= alphaThreshold)
        {
            transform.localScale = hoveredScale;
            hovered = true;
        }        
    }

    public void OnHoverExit()
    {
        transform.localScale = Vector3.one;
        hovered = false;
    }

    public void OnClick()
    {
        if (MainImage.GetComponent<Image>().color.a >= alphaThreshold)
        {
            transform.parent.transform.parent.transform.parent.GetComponent<StylesScreen>().setSelected(this);
            transform.parent.transform.parent.transform.parent.GetComponent<StylesScreen>().updateStyle(MainImage.GetComponent<Image>().sprite);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultButton : MonoBehaviour
{
    //private Color hoveredColor = new Color(0 / 255f, 34 / 255f, 120 / 255f, 80 / 255f);
    private Color hoveredColor = new Color(65 / 255f, 109 / 255f, 212 / 255f, 78 / 255f);
    private Color disabledColor = new Color(10 / 255f, 10 / 255f, 10 / 255f, 50 / 255f);
    public Color defaultColor;

    private bool hovered = false;

    private bool disabled = false;
    public bool Disabled
    {
        set
        {
            disabled = value;

            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<DefaultButton>() != null)
                    child.gameObject.GetComponent<DefaultButton>().Disabled = value;
            }
        }
    }

    public Color CurrentColor
    {
        get
        {
            if (disabled)
            {
                return disabledColor;
            }
            else if (hovered)
            {
                return hoveredColor;
            }
            else
            {
                return defaultColor;
            }
        }
    }

    void Start()
    {
        if (transform.GetComponent<Image>() != null)
            defaultColor = transform.GetComponent<Image>().color;
    }

    private void Update()
    {
        if (transform.GetComponent<Image>() != null)
        {
            transform.GetComponent<Image>().color = CurrentColor;
        }        
    }

    public void OnHoverEnter()
    {
        hovered = true;
    }

    public void OnHoverExit()
    {
        hovered = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoveredButton : MonoBehaviour
{
    public Sprite HoveredSprite;
    private Sprite OriginalSprite;

    private bool hovered = false;
    public bool Hovered
    {
        set
        {
            hovered = value;

            if (GetComponent<Image>() != null)
            {
                if (hovered)
                {
                    GetComponent<Image>().sprite = HoveredSprite;
                }
                else
                {
                    GetComponent<Image>().sprite = OriginalSprite;
                }
            }
        }
    }

    void Start()
    {
        if (GetComponent<Image>() != null)
            OriginalSprite = GetComponent<Image>().sprite;
    }

    public void OnHoverEnter()
    {
        Hovered = true;
    }

    public void OnHoverExit()
    {
        Hovered = false;
    }
}

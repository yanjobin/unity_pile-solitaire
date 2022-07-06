using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameBackground : MonoBehaviour, IPointerClickHandler
{
    public GameObject SideBar;

    public void Start()
    {
        GetComponent<Image>().sprite = GameStyles.CURRENT_BACKGROUND;
    }

    public void Update()
    {
        GetComponent<Image>().sprite = GameStyles.CURRENT_BACKGROUND;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            SideBar.GetComponent<SideBar>().Toggle();
    }
}

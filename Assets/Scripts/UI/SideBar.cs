using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBar : MonoBehaviour
{
    private static float TIME_TO_OPEN = 0.2f;

    public GameObject ToggleButton;

    private bool closed = true;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Toggle()
    {
        closed = !closed;
#if UNITY_ANDROID
        float dx = Screen.width / 1280f;
#else
        float dx = Screen.width / 1920f;
#endif

        Vector3 width = new Vector3(transform.GetComponent<RectTransform>().rect.width * dx, 0, 0);
        GetComponent<SmoothMovement>().Move((closed ? width : -width), TIME_TO_OPEN);

        if (closed)
        {
            ToggleButton.GetComponent<SmoothRotate>().Rotate(180, new Vector3(0, 0, 1), TIME_TO_OPEN);
        }
        else
        {
            ToggleButton.GetComponent<SmoothRotate>().Rotate(-180, new Vector3(0, 0, 1), TIME_TO_OPEN);
        }
    }
}

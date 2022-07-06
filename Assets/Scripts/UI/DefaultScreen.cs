using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultScreen : MonoBehaviour
{
    public GameObject Background;
    public GameManager gameManager;

    public void Show()
    {
        StartCoroutine(DoShow());
    }

    private IEnumerator DoShow()
    {
        GetComponent<FadeInFadeOut>().FadeIn(0.5f);
        yield return new WaitForSeconds(0.5f);
        Background.GetComponent<FadeInFadeOut>().FadeIn(0.2f);
    }

    public void OnClickClose()
    {
        gameManager.GetComponent<GameManager>().CloseScreen(transform.gameObject, true);
    }
}

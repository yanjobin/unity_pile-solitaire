using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinGameScreen : MonoBehaviour
{
    public GameObject Background;
    public GameManager gameManager;

    public GameObject txtTimer;
    public GameObject txtMoves;

    public void Show(string time, int moves)
    {
        txtTimer.GetComponent<Text>().text = time;
        txtMoves.GetComponent<Text>().text = moves.ToString();

        StartCoroutine(DoShow());
    }

    private IEnumerator DoShow()
    {
        GetComponent<FadeInFadeOut>().FadeIn(1);
        yield return new WaitForSeconds(0.5f);
        Background.GetComponent<FadeInFadeOut>().FadeIn(0.2f);
    }

    public void OnClickClose()
    {
        gameManager.StopParticles();
        gameManager.CloseScreen(transform.gameObject, false);
    }
}

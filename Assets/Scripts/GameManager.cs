using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject MainCanvas;
    public GameObject TxtTimer;
    public GameObject TxtMoveCounter;
    public GameObject BtnToggleFullscreen;

    public Sprite GoFullscreen;
    public Sprite GoWindowed;

    public GameObject WinScreenPrefab;
    public GameObject BackgroundStylesPrefab;
    public GameObject CardStylesPrefab;
    public GameObject RulesScreenPrefab;

    public GameObject HintManager;
    public GameObject CardManager;

    private float startTime;
    private float currentTime;

    private int moveCounter;

    private bool winGame;
    private bool gameRunning;
    public bool IsGameRunning
    {
        get
        {
            return gameRunning;
        }
    }
    public bool IsGameWin
    {
        get
        {
            return winGame;
        }
    }

    private void Awake()
    {
#if UNITY_ANDROID
        MainCanvas = GameObject.Find("Main Canvas Android Landscape");
#else
        MainCanvas = GameObject.Find("Main Canvas PC");
#endif
    }

    void Start()
    {
        resetGame();
    }

    void Update()
    {
        if (gameRunning)
        {
            currentTime += Time.deltaTime;
            updateUITimer();
        }
    }

    public void addMove()
    {
        moveCounter++;
        updateUIMoveCounter();
    }

    public void startGame()
    {
        startTime = Time.time;
        currentTime = 0;
        moveCounter = 0;
        gameRunning = true;
        updateUITimer();
    }

    public void resetGame()
    {
        currentTime = 0;
        moveCounter = 0;
        gameRunning = false;
        winGame = false;
        updateUITimer();
        updateUIMoveCounter();

        HintManager.GetComponent<HintManager>().CancelCurrentHint();
    }

    private void updateUITimer()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        TxtTimer.GetComponent<Text>().text = niceTime;
    }

    private void updateUIMoveCounter()
    {
        TxtMoveCounter.GetComponent<Text>().text = "Moves  " + moveCounter;
    }

    public void StopParticles()
    {
        GameObject eRight = GameObject.Find("RightEmitter");
        GameObject eLeft = GameObject.Find("LeftEmitter");

        eRight.GetComponent("UIParticleSystem").SendMessage("Stop");
        eLeft.GetComponent("UIParticleSystem").SendMessage("Stop");
    }

    public IEnumerator DoGameCompleted()
    {
        gameRunning = false;
        winGame = true;
        yield return new WaitForSeconds(0.5f);

        GameObject eRight = GameObject.Find("RightEmitter");
        GameObject eLeft = GameObject.Find("LeftEmitter");

        eRight.transform.SetAsLastSibling();
        eLeft.transform.SetAsLastSibling();

        eRight.GetComponent("UIParticleSystem").SendMessage("Play");
        eLeft.GetComponent("UIParticleSystem").SendMessage("Play");

        yield return new WaitForSeconds(1);

        CreateWinScreen();
    }

    public void CloseScreen(GameObject screen, bool resumeGame)
    {
        screen.GetComponent<FadeInFadeOut>().FadeChildren = true;
        screen.GetComponent<FadeInFadeOut>().FadeOut(0.2f);

        GameObject.Destroy(screen, 5);

        if (resumeGame)
            gameRunning = true;
    }

    private void CreateWinScreen()
    {
        GameObject WinScreen = Instantiate(WinScreenPrefab, MainCanvas.transform);
        WinScreen.transform.SetAsLastSibling();

        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        WinScreen.GetComponent<WinGameScreen>().gameManager = this;
        WinScreen.GetComponent<WinGameScreen>().Show(niceTime, moveCounter);
    }

    public void OnClickShowStylesScreen(int style)
    {
        HintManager.GetComponent<HintManager>().CancelCurrentHint();

        GameObject prefab = null;
        if (style == (int)EStyles.Background)
        {
            prefab = BackgroundStylesPrefab;
        }
        else if (style == (int)EStyles.Card)
        {
            prefab = CardStylesPrefab;
        }

        if (prefab != null)
        {
            GameObject Screen = Instantiate(prefab, MainCanvas.transform);
            Screen.transform.SetAsLastSibling();

            Screen.GetComponent<DefaultScreen>().gameManager = this;
            Screen.GetComponent<DefaultScreen>().Show();
            gameRunning = false;
        }        
    }

    public void OnClickShowRulesScreen()
    {
        HintManager.GetComponent<HintManager>().CancelCurrentHint();

        GameObject Screen = Instantiate(RulesScreenPrefab, MainCanvas.transform);
        Screen.transform.SetAsLastSibling();

        Screen.GetComponent<DefaultScreen>().gameManager = this;
        Screen.GetComponent<DefaultScreen>().Show();
        gameRunning = false;
    }

    public void OnClickButtonCancelCurrentHint()
    {
        HintManager.GetComponent<HintManager>().CancelCurrentHint();
    }

    public void OnClickToggleFullscreen()
    {
        if (!CardManager.GetComponent<CardManager>().HasOneCardMoving())
        {
            Screen.fullScreen = !Screen.fullScreen;
            updateWindow();
        }        
    }

    private void updateWindow()
    {
        if (Screen.fullScreen)
        {
            Screen.SetResolution(1440, 810, false);
        }
        else
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        updateToggleFullscreenIcon();
    }

    private void updateToggleFullscreenIcon()
    {
        if (Screen.fullScreen)
        {
            BtnToggleFullscreen.GetComponent<Image>().sprite = GoFullscreen;
        }
        else
        {
            BtnToggleFullscreen.GetComponent<Image>().sprite = GoWindowed;
        }
    }
}


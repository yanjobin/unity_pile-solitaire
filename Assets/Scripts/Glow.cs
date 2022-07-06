using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Glow : MonoBehaviour
{
    public static Color COLOR_UI_HOVER = Color.white;
    public static Color COLOR_UI_SELECTED = Color.yellow;

    public static Color COLOR_HINT = Color.yellow;
    public static Color COLOR_DRAGGING = Color.cyan;

    private Color currentColor;
    private float currentTime;
    private float glowTime;

    private readonly float DEFAULT_FADE_TIME = 0.1f;

    private GameObject glowImage;

    private bool glowing;
    public bool IsGlowing
    {
        get
        {
            return glowing;
        }
    }

    private bool started = false;

    void Start()
    {
        glowImage = transform.Find("GlowImage").gameObject;
        Color c = glowImage.GetComponent<Image>().color;
        glowImage.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0);
        started = true;
    }

    public void ShowGlow(Color glowColor, float time = -1, float fadeInTime = -1, float fadeOutTime = -1)
    {
        if (!started)
            Start();

        currentColor = glowColor;
        currentTime = 0;
        glowTime = time;
        if (!glowing)
        {            
            StartCoroutine(DoGlow(fadeInTime >= 0 ? fadeInTime : DEFAULT_FADE_TIME, fadeOutTime >= 0 ? fadeOutTime : DEFAULT_FADE_TIME));
        }
    }

    public void CancelGlow()
    {
        glowing = false;
    }

    private IEnumerator DoGlow(float fadeInTime, float fadeOutTime)
    {
        currentTime = 0;
        glowing = true;
        glowImage.GetComponent<Image>().enabled = true;
        glowImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);

        yield return StartCoroutine(FadeInGlow(fadeInTime));

        while (glowing && (glowTime > 0 ? currentTime < glowTime : true))
        {
            currentTime += Time.deltaTime;
            glowImage.GetComponent<Image>().color = currentColor;
            yield return null;
        }
        
        yield return StartCoroutine(FadeOutGlow(fadeOutTime));

        glowImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
        glowImage.GetComponent<Image>().enabled = false;
    }

    private IEnumerator FadeInGlow(float fadeInTime)
    {
        float counter = 0;

        while (counter < fadeInTime)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, counter / fadeInTime);
            glowImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        glowImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
    }

    private IEnumerator FadeOutGlow(float fadeOutTime)
    {
        float counter = 0;

        while (counter < fadeOutTime)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / fadeOutTime);
            glowImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        glowImage.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
    }
}

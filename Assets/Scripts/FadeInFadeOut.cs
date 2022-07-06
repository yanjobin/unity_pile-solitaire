using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInFadeOut : MonoBehaviour
{
    public bool FadeChildren = true;
    public bool Ignore = false;
    public float maxAlpha = -1;

    private bool isFading = false;
    public bool IsFading
    {
        get
        {
            return isFading;
        }
    }

    public void FadeIn(float seconds)
    {
        StartCoroutine(fadeInAndOut(gameObject, true, seconds, FadeChildren));
    }

    public void FadeOut(float seconds)
    {
        StartCoroutine(fadeInAndOut(gameObject, false, seconds, FadeChildren));
    }

    private IEnumerator fadeInAndOut(GameObject objectToFade, bool fadeIn, float duration, bool fadeChildren)
    {
        if (objectToFade.GetComponent<FadeInFadeOut>() == null || !objectToFade.GetComponent<FadeInFadeOut>().Ignore)
        {
            isFading = true;
            if (fadeChildren)
            {
                if (objectToFade.GetComponent<FadeInFadeOut>() == null || objectToFade.GetComponent<FadeInFadeOut>().FadeChildren)
                {
                    for (int i = 0; i < objectToFade.transform.childCount; i++)
                    {
                        StartCoroutine(fadeInAndOut(objectToFade.transform.GetChild(i).gameObject, fadeIn, duration, fadeChildren));
                    }
                }
            }

            if (fadeIn)
                objectToFade.SetActive(true);

            float counter = 0f;

            int mode = 0;
            Color currentColor = Color.clear;

            //Set Values depending on if fadeIn or fadeOut
            float a, b;
            if (fadeIn)
            {
                a = 0;
                if (objectToFade.GetComponent<DefaultButton>() != null)
                    b = objectToFade.GetComponent<DefaultButton>().defaultColor.a;
                else if (objectToFade.GetComponent<FadeInFadeOut>() != null && objectToFade.GetComponent<FadeInFadeOut>().maxAlpha >= 0)
                    b = objectToFade.GetComponent<FadeInFadeOut>().maxAlpha;
                else
                    b = 1;
            }
            else
            {
                if (objectToFade.GetComponent<DefaultButton>() != null)
                    a = objectToFade.GetComponent<DefaultButton>().defaultColor.a;
                else if (objectToFade.GetComponent<FadeInFadeOut>() != null && objectToFade.GetComponent<FadeInFadeOut>().maxAlpha >= 0)
                    a = objectToFade.GetComponent<FadeInFadeOut>().maxAlpha;
                else
                    a = 1;
                b = 0;
            }

            SpriteRenderer tempSPRenderer = objectToFade.GetComponent<SpriteRenderer>();
            Image tempImage = objectToFade.GetComponent<Image>();
            RawImage tempRawImage = objectToFade.GetComponent<RawImage>();
            MeshRenderer tempRenderer = objectToFade.GetComponent<MeshRenderer>();
            Text tempText = objectToFade.GetComponent<Text>();

            //Check if this is a Sprite
            if (tempSPRenderer != null)
            {
                currentColor = tempSPRenderer.color;
                mode = 0;
            }
            //Check if Image
            else if (tempImage != null)
            {
                currentColor = tempImage.color;
                mode = 1;
            }
            //Check if RawImage
            else if (tempRawImage != null)
            {
                currentColor = tempRawImage.color;
                mode = 2;
            }
            //Check if Text 
            else if (tempText != null)
            {
                currentColor = tempText.color;
                mode = 3;
            }

            //Check if 3D Object
            else if (tempRenderer != null)
            {
                currentColor = tempRenderer.material.color;
                mode = 4;

                //ENABLE FADE Mode on the material if not done already
                tempRenderer.material.SetFloat("_Mode", 2);
                tempRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                tempRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                tempRenderer.material.SetInt("_ZWrite", 0);
                tempRenderer.material.DisableKeyword("_ALPHATEST_ON");
                tempRenderer.material.EnableKeyword("_ALPHABLEND_ON");
                tempRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                tempRenderer.material.renderQueue = 3000;
            }
            else
            {
                yield break;
            }

            if (objectToFade.GetComponent<DefaultButton>() != null)
            {
                currentColor = objectToFade.GetComponent<DefaultButton>().CurrentColor;
            }

            while (counter < duration)
            {
                if (objectToFade.GetComponent<DefaultButton>() != null)
                {
                    currentColor = objectToFade.GetComponent<DefaultButton>().CurrentColor;
                }

                counter += Time.deltaTime;
                float alpha = Mathf.Lerp(a, b, counter / duration);

                switch (mode)
                {
                    case 0:
                        tempSPRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                        break;
                    case 1:
                        tempImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                        break;
                    case 2:
                        tempRawImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                        break;
                    case 3:
                        tempText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                        break;
                    case 4:
                        tempRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                        break;
                }
                yield return null;
            }

            if (!fadeIn)
                objectToFade.SetActive(false);

            isFading = false;
        }        
    }
}

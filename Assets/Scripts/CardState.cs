using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardState : MonoBehaviour
{
    private static float TIME_TO_ROTATE = 0.2f;

    private bool flipped = true;
    public bool Flipped
    {
        set
        {
            if (value != flipped) {
                Flip();
            }
        }
        get
        {
            return flipped;
        }
    }

    public bool InstantFlipped
    {
        set
        {
            if (value != flipped)
            {
                Flip(0);
            }
        }
    }

    private bool isRotating = false;
    public bool IsRotating
    {
        get
        {
            return IsRotating;
        }
    }

    private void Flip(float time = -1)
    {
        StartCoroutine(DoFlip(time >= 0 ? time : TIME_TO_ROTATE));
    }

    private IEnumerator DoFlip(float time)
    {
        isRotating = true;
        bool spriteSwitched = false;
        float counter = 0f;
        float currentRotation = transform.rotation.y;

        while (counter < time)
        {
            counter += Time.deltaTime;
            float newRotation;
            if (!spriteSwitched)
                newRotation = Mathf.Lerp(currentRotation, 180, counter / (time / 2));
            else
                newRotation = Mathf.Lerp(currentRotation + 180, 360, counter / (time / 2));

            if (newRotation >= 90 && !spriteSwitched)
            {
                newRotation += 180;
                flipped = !flipped;
                GetComponent<CardValue>().updateSprite();
                spriteSwitched = true;
            }

            transform.rotation = Quaternion.Euler(new Vector3(0, newRotation, 0));
            yield return null;
        }

        transform.rotation = Quaternion.Euler(Vector3.zero);

        if (time == 0)
        {
            flipped = !flipped;
            GetComponent<CardValue>().updateSprite();
        }

        isRotating = false;
    }
}

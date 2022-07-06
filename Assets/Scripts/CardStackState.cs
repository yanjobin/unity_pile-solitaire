using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardStackState : MonoBehaviour
{
    public GameObject undoManager;

#if UNITY_ANDROID
    private Vector3 offset = new Vector3(30, -10f, 0f);
    private Vector3 Offset
    {
        get
        {
            float dx = Screen.width / 1280f;
            float dy = Screen.height / 800f;
            return new Vector3(offset.x * dx, offset.y * dy, offset.z);
        }
    }
#else
    private Vector3 offset = new Vector3(5f, -35f, 0f);
    private Vector3 Offset
    {
        get
        {
            float dx = Screen.width / 1920f;
            float dy = Screen.height / 1080f;
            return new Vector3(offset.x * dx, offset.y * dy, offset.z);
        }
    }
#endif


    private bool stackCompleted = false;

    public static int MaxStack = 4;

    public int CurrentStack
    {
        get
        {
            return transform.childCount;
        }
    }

    public bool IsStackFull
    {
        get
        {
            return transform.childCount >= MaxStack;
        }
    }

    public bool IsStackCompleted
    {
        get
        {
            return stackCompleted;
        }
    }

    public Vector3 GetNextCardPosition
    {
        get
        {
            return transform.position + ((Offset) * ((float)CurrentStack));
        }
    }

    public (int number, ECardValue value, List<GameObject> cards) GetTopMostCombo
    {
        get
        {
            (int number, ECardValue value, List<GameObject> cards) result = (0, ECardValue.A, new List<GameObject>());

            if (IsStackCompleted || CurrentStack == 0)
                return result;

            result.number = 1;
            result.value = transform.GetChild(CurrentStack - 1).GetComponent<CardValue>().Value;
            result.cards.Add(transform.GetChild(CurrentStack - 1).gameObject);
            for (int i = CurrentStack - 2; i >= 0; i--)
            {
                if (transform.GetChild(i).GetComponent<CardValue>().Value == result.value)
                {
                    result.number++;
                    result.cards.Add(transform.GetChild(i).gameObject);
                }
                else
                    break;
            }

            return result;
        }
    }

    void Update()
    {
        if (IsStackFull)
        {
            if (!this.stackCompleted)
            {
                bool stackCompleted = true;
                ECardValue val = transform.GetChild(0).GetComponent<CardValue>().Value;

                for (int i = 1; i < transform.childCount; i++)
                {
                    if (val != transform.GetChild(i).GetComponent<CardValue>().Value)
                    {
                        stackCompleted = false;
                        break;
                    }
                }

                if (stackCompleted)
                {
                    onStackCompleted();
                }
            }            
        }
        else
        {
            stackCompleted = false;
        }
    }

    private void onStackCompleted()
    {
        undoManager.GetComponent<UndoRedoManager>().Clear();
        this.stackCompleted = true;
        StartCoroutine(flipStack());
    }

    private IEnumerator flipStack()
    {
        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<CardState>().Flipped = true;
            transform.GetChild(i).GetComponent<SmoothScale>().Pulse(SmoothScale.FLIP_SMALL_PULSE_SCALE, SmoothScale.FLIP_SMALL_PULSE_TIME, 1);
        }
    }

    public Vector3 getPositionAtIndex(int i)
    {
        return transform.position + (Offset * (Mathf.Max(0, i)));
    }
}

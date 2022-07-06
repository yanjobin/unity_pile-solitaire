using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    private static float DEFAULT_HINT_TIME = -1f;
    private static float TIME_SHOW_AUTOMATIC_HINT = 60;
    private static float TIME_FADE_ANIMATING_HINT = 0.25f / 2;
    private static float TIME_WAIT = 0.75f;

    public GameObject Card_Manager;
    public GameObject MainCanvas;
    private List<GameObject> CardStacks;

    private float idleTimer;
    private bool animatingHint = false;
    private bool forceStopAnimatingHint = false;

    void Start()
    {
        CardStacks = Card_Manager.GetComponent<CardManager>().CardStacks;
        idleTimer = 0;
    }

    void Update()
    {
        if (Card_Manager.GetComponent<CardManager>().GameManager.GetComponent<GameManager>().IsGameRunning)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= TIME_SHOW_AUTOMATIC_HINT)
            {
                StartCoroutine(DoShowHint());
            }
        }
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown /*&& e.control */&& e.keyCode == KeyCode.H)
        {
            OnClickShowHint();
        }
    }

    public void resetIdleTimer()
    {
        idleTimer = 0;
    }

    public void CancelCurrentHint()
    {
        resetIdleTimer();
        forceStopAnimatingHint = true;
    }

    public void OnClickShowHint()
    {
        Card_Manager.GetComponent<CardManager>().cancelAllGlow();
        StartCoroutine(DoShowHint());
    }

    private IEnumerator DoShowHint()
    {
        var randomHint = getRandomHintFromPossibleHints();
        if (randomHint.stack1 != null && randomHint.stack2 != null)
        {
            var top1 = randomHint.stack1.GetComponent<CardStackState>().GetTopMostCombo;
            var top2 = randomHint.stack2.GetComponent<CardStackState>().GetTopMostCombo;

            if (randomHint.stack1.GetComponent<CardStackState>().CurrentStack + top2.cards.Count <= CardStackState.MaxStack &&
                randomHint.stack2.GetComponent<CardStackState>().CurrentStack + top1.cards.Count <= CardStackState.MaxStack)
            {
                //Cards from either stack can be moved to the other stack
                top1.cards.ForEach(x => {
                    x.GetComponent<Glow>().ShowGlow(Glow.COLOR_HINT, DEFAULT_HINT_TIME);
                    x.GetComponent<SmoothScale>().Pulse(SmoothScale.HINT_SMALL_PULSE_SCALE, SmoothScale.HINT_SMALL_PULSE_TIME, 1);
                });
                top2.cards.ForEach(x => {
                    x.GetComponent<Glow>().ShowGlow(Glow.COLOR_HINT, DEFAULT_HINT_TIME);
                    x.GetComponent<SmoothScale>().Pulse(SmoothScale.HINT_SMALL_PULSE_SCALE, SmoothScale.HINT_SMALL_PULSE_TIME, 1);
                });
            }
            else
            {
                GameObject stackFrom;
                GameObject stackTo;

                if (randomHint.stack1.GetComponent<CardStackState>().CurrentStack + top2.cards.Count <= CardStackState.MaxStack)
                {
                    //Only cards from the stack 2 can be moved to stack 1
                    stackFrom = randomHint.stack2;
                    stackTo = randomHint.stack1;
                }
                else
                {
                    //Only cards from the stack 1 can be moved to stack 2
                    stackFrom = randomHint.stack1;
                    stackTo = randomHint.stack2;
                }

                var topFrom = stackFrom.GetComponent<CardStackState>().GetTopMostCombo;
                var topTo = stackTo.GetComponent<CardStackState>().GetTopMostCombo;

                topTo.cards.ForEach(x => {
                    x.GetComponent<Glow>().ShowGlow(Glow.COLOR_HINT, DEFAULT_HINT_TIME, fadeOutTime: TIME_FADE_ANIMATING_HINT * 4);
                    x.GetComponent<SmoothScale>().Pulse(SmoothScale.HINT_SMALL_PULSE_SCALE, SmoothScale.HINT_SMALL_PULSE_TIME, 1);
                });

                List<GameObject> tempMovingCards = new List<GameObject>();

                for (int i = topFrom.cards.Count - 1; i >= 0; i--)
                {
                    GameObject card = topFrom.cards[i];

                    GameObject newCard = Instantiate(card, card.transform);
                    newCard.GetComponent<CardState>().InstantFlipped = false;
                    newCard.GetComponent<CardValue>().Value = card.GetComponent<CardValue>().Value;
                    newCard.GetComponent<CardValue>().Suit = card.GetComponent<CardValue>().Suit;
                    newCard.GetComponent<CardValue>().updateSprite();
                    newCard.GetComponent<Glow>().ShowGlow(Glow.COLOR_HINT, DEFAULT_HINT_TIME);
                    newCard.GetComponent<SmoothScale>().Pulse(SmoothScale.HINT_SMALL_PULSE_SCALE, SmoothScale.HINT_SMALL_PULSE_TIME, 1);
                    newCard.AddComponent<FadeInFadeOut>();
                    newCard.transform.Find("CardImage").GetComponent<Image>().raycastTarget = false;
                    GameObject.Destroy(newCard.GetComponent<DragAndDrop>());
                    GameObject.Destroy(newCard.GetComponent<Rigidbody2D>());
                    GameObject.Destroy(newCard.GetComponent<BoxCollider2D>());

                    newCard.transform.position = card.transform.position;
                    newCard.transform.SetParent(MainCanvas.transform);
                    newCard.transform.SetAsLastSibling();

                    tempMovingCards.Add(newCard);
                }

                forceStopAnimatingHint = true;
                yield return new WaitUntil(() => !animatingHint);
                yield return StartCoroutine(DoAnimationHint(tempMovingCards, stackTo));
            }
        }

        yield return null;
    }

    private IEnumerator DoAnimationHint(List<GameObject> cards, GameObject stackTo)
    {
        animatingHint = true;
        forceStopAnimatingHint = false;

        //Save initial position of cards being animated
        Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();
        for (int i = 0; i < cards.Count; i++)
        {
            positions[i] = new Vector3(cards[i].transform.position.x, cards[i].transform.position.y, cards[i].transform.position.z);
        }

        int j = 0;
        while (!forceStopAnimatingHint && animatingHint)
        {
            resetIdleTimer();

            cards.ForEach(x => x.SetActive(true));

            if (j > 0)
            {
                //On rounds > 0, reset position to initial position and fade cards in
                for (int i = 0; i < cards.Count; i++)
                {
                    cards[i].transform.position = positions[i];
                    cards[i].GetComponent<FadeInFadeOut>().FadeIn(0.001f);
                }

                //Tait until cards are faded in
                yield return new WaitUntil(() => (!animatingHint || forceStopAnimatingHint) || cards.Where(x => x.GetComponent<FadeInFadeOut>().IsFading).ToList().Count == 0);
            }

            yield return StartCoroutine(WaitForSecondsOrNotAnimating(TIME_WAIT));

            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].GetComponent<SmoothMovement>().MoveTo(stackTo.GetComponent<CardStackState>().getPositionAtIndex(stackTo.GetComponent<CardStackState>().CurrentStack + i),
                                                                    CardManager.HINT_TIME_MOVE_CARD);
            }

            //Wait until cards have moved to destination
            yield return new WaitUntil(() => (!animatingHint || forceStopAnimatingHint) || cards.Where(x => x.GetComponent<SmoothMovement>().IsMoving).ToList().Count == 0);
            yield return StartCoroutine(WaitForSecondsOrNotAnimating(TIME_WAIT));

            cards.ForEach(x => {
                x.GetComponent<FadeInFadeOut>().FadeOut(TIME_FADE_ANIMATING_HINT);
            });  
            
            //Wait until cards are faded out
            yield return new WaitUntil(() => (!animatingHint || forceStopAnimatingHint) || cards.Where(x => x.GetComponent<FadeInFadeOut>().IsFading).ToList().Count == 0);
            yield return StartCoroutine(WaitForSecondsOrNotAnimating(TIME_WAIT));

            j++;
        }

        //Destroy animated cards when hint it cancelled
        cards.ForEach(x => GameObject.Destroy(x.gameObject));

        animatingHint = false;
        forceStopAnimatingHint = false;

        yield return null;
    }

    private IEnumerator WaitForSecondsOrNotAnimating(float time)
    {
        float counter = 0;
        while (animatingHint && !forceStopAnimatingHint && counter < time)
        {
            counter += Time.deltaTime;
            yield return null;
        }
    }

    private (GameObject stack1, GameObject stack2) getRandomHintFromPossibleHints()
    {
        List<(GameObject stack1, GameObject stack2)> hints = getPossibleHints();
        return hints.OrderBy(x => Random.value).FirstOrDefault();
    }

    private List<(GameObject stack1, GameObject stack2)> getPossibleHints()
    {
        List<(GameObject stack1, GameObject stack2)> result = new List<(GameObject stack1, GameObject stack2)>();

        foreach (GameObject stack in CardStacks)
        {
            var topMost = stack.GetComponent<CardStackState>().GetTopMostCombo;
            if (topMost.number > 0)
            {
                foreach (GameObject s in CardStacks)
                {
                    if (!s.Equals(stack) && !s.GetComponent<CardStackState>().IsStackFull && s.GetComponent<CardStackState>().CurrentStack > 0)
                    {
                        var sTopMost = s.GetComponent<CardStackState>().GetTopMostCombo;
                        if (sTopMost.number > 0)
                        {
                            if (topMost.value == sTopMost.value)
                            {
                                if (s.GetComponent<CardStackState>().CurrentStack + topMost.number <= CardStackState.MaxStack)
                                {
                                    //Stack 1 topmost can be moved to stack 2
                                    result.Add((stack, s));
                                }
                            }
                        }
                    }
                }
            }            
        }

        return result;
    }
}

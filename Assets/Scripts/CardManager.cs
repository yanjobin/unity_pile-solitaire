using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static float TIME_MOVE_CARD = 0.20f;
    public static float HINT_TIME_MOVE_CARD = 1.2f;

    public GameObject CardPrefab;
    public GameObject UndoRedoManagerPrefab;
    public List<GameObject> CardStacks;

    public GameObject DrawCardsButton;
    public GameObject BtnUndo;
    public GameObject BtnRedo;
    public GameObject GameManager;

    private Color defaultStackColor;

    private List<GameObject> deck;
    private GameObject undoManager;

    private void Awake()
    {
        defaultStackColor = CardStacks[0].GetComponent<Image>().color;
    }

    void Start()
    {
        undoManager = Instantiate(UndoRedoManagerPrefab, transform);
        undoManager.GetComponent<UndoRedoManager>().BtnUndo = BtnUndo;
        undoManager.GetComponent<UndoRedoManager>().BtnRedo = BtnRedo;
        foreach (GameObject cs in CardStacks)
        {
            cs.GetComponent<CardStackState>().undoManager = undoManager;
        }

        createCards();
        shuffleCards();
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Z)
        {
            GameManager.GetComponent<GameManager>().OnClickButtonCancelCurrentHint();
            OnClickUndo();
        }
        else if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Y)
        {
            GameManager.GetComponent<GameManager>().OnClickButtonCancelCurrentHint();
            OnClickRedo();
        }
    }

    void Update()
    {
        foreach (GameObject stack in CardStacks)
        {
            stack.GetComponent<Image>().color = defaultStackColor;
        }

        if (GameManager.GetComponent<GameManager>().IsGameRunning)
        {
            int stackCompleted = CardStacks.Count(x => x.GetComponent<CardStackState>().IsStackCompleted);

            if (stackCompleted >= 13)
            {
                StartCoroutine(GameManager.GetComponent<GameManager>().DoGameCompleted());
            }
        }
        
    }

    private void createCards()
    {
        List<(ECardSuit suit, ECardValue value)> deckValues = new List<(ECardSuit, ECardValue)>();
        for (int i = 0; i < System.Enum.GetNames(typeof(ECardSuit)).Length; i++)
        {
            for (int j = 0; j < System.Enum.GetNames(typeof(ECardValue)).Length; j++)
            {
                (ECardSuit suit, ECardValue value) combo = ((ECardSuit)System.Enum.GetValues(typeof(ECardSuit)).GetValue(i), (ECardValue)System.Enum.GetValues(typeof(ECardValue)).GetValue(j));
                deckValues.Add(combo);
            }
        }
        
        deck = new List<GameObject>();

        foreach (var c in deckValues)
        {
            GameObject newCard = Instantiate(CardPrefab, transform);
            newCard.GetComponent<CardValue>().Value = c.value;
            newCard.GetComponent<CardValue>().Suit = c.suit;
            newCard.GetComponent<CardValue>().updateSprite();
            newCard.GetComponent<DragAndDrop>().undoManager = undoManager.GetComponent<UndoRedoManager>();
            newCard.GetComponent<DragAndDrop>().cardManager = this;

            newCard.GetComponent<SmoothMovement>().MoveTo(transform.position, 0);

            deck.Add(newCard);
        }
    }

    private void shuffleCards()
    {
        deck = deck.OrderBy(x => Random.value).ToList();
        deck.ForEach(x => x.transform.SetAsLastSibling());
    }

    private IEnumerator distributeCards()
    {
        DrawCardsButton.transform.Find("Image").GetComponent<DefaultButton>().OnHoverExit();
        DrawCardsButton.GetComponent<FadeInFadeOut>().FadeOut(0.8f);

        int currentStack = 0;
        foreach (var c in deck)
        {
            CardStacks[currentStack].transform.SetAsLastSibling();

            c.GetComponent<SmoothMovement>().MoveTo(CardStacks[currentStack].GetComponent<CardStackState>().GetNextCardPosition, TIME_MOVE_CARD);
            c.GetComponent<CardState>().Flipped = false;
            c.transform.SetParent(CardStacks[currentStack].transform);

            if (CardStacks[currentStack].transform.childCount >= 4)
                currentStack++;

            yield return new WaitForSeconds(0.015f);
        }

        GameManager.GetComponent<GameManager>().startGame();
    }

    private IEnumerator resetCardsBackToDeck()
    {
        for (int i = CardStacks.Count - 1; i >= 0; i--)
        {
            CardStacks[i].transform.SetAsLastSibling();
            for (int y = CardStacks[i].transform.childCount - 1; y >= 0; y--)
            {
                GameObject card = CardStacks[i].transform.GetChild(y).gameObject;
                card.transform.SetAsLastSibling();
                card.GetComponent<CardState>().Flipped = true;
                yield return new WaitForSeconds(0.02f);
                card.GetComponent<SmoothMovement>().MoveTo(transform.position, TIME_MOVE_CARD);
            }
        }

        yield return new WaitUntil(() => (deck.Where(x => x.GetComponent<SmoothMovement>().IsMoving).ToList().Count == 0));

        deck.ForEach(x => x.transform.SetParent(transform));
        shuffleCards();

        DrawCardsButton.GetComponent<FadeInFadeOut>().FadeIn(0.8f);
    }

    public bool HasOneCardMoving()
    {
        return deck.Exists(x => x.GetComponent<SmoothMovement>().IsMoving);
    }

    public void cancelAllGlow()
    {
        deck.ForEach(x => x.GetComponent<Glow>().CancelGlow());
    }

    public void updateAllCardSprite()
    {
        deck.ForEach(x => x.GetComponent<CardValue>().updateSprite());
    }

    public void OnClickDrawCards()
    {
        bool exists = deck.Where(x => x.GetComponent<CardState>().Flipped).Count() > 0;

        StartCoroutine(distributeCards());
    }

    public void OnClickNewGame()
    {
        if (GameManager.GetComponent<GameManager>().IsGameRunning || GameManager.GetComponent<GameManager>().IsGameWin)
        {
            cancelAllGlow();
            undoManager.GetComponent<UndoRedoManager>().Clear();
            GameManager.GetComponent<GameManager>().resetGame();

            StartCoroutine(resetCardsBackToDeck());
        }        
    }

    public void OnClickUndo()
    {
        undoManager.GetComponent<UndoRedoManager>().Undo();
    }

    public void OnClickRedo()
    {
        undoManager.GetComponent<UndoRedoManager>().Redo();
    }
}

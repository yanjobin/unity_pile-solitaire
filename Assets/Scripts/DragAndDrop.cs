using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour
{
    public CardManager cardManager;
    public UndoRedoManager undoManager;

    private GameObject overlappedStack;
    private List<GameObject> draggedCards;

    private bool isDragging;
    private bool isDragged;

    private Vector3 startPosition;
    private Vector3 offset;

    void Start()
    {
        isDragging = false;
        isDragged = false;
        overlappedStack = null;
        draggedCards = new List<GameObject>();
    }

    void Update()
    {
        if (isDragging)
        {
            cardManager.GameManager.GetComponent<GameManager>().HintManager.GetComponent<HintManager>().CancelCurrentHint();
            //Update card position, following mouse with initial offset
            transform.position = (Input.mousePosition - offset);
        }

        if (!isDragged && overlappedStack != null && !overlappedStack.Equals(transform.parent.gameObject))
        {
            //Show overlapped stack in red
            overlappedStack.GetComponent<Image>().color = Color.cyan;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDragging)
            return;

        if (collision.gameObject.Equals(transform.parent.gameObject))
            //Colliding stack is same as current card's stack, do nothing
            return;

        if (overlappedStack == null)
        {
            //Set currently overlapped stack
            overlappedStack = collision.gameObject;
        }
        else
        {
            //Check for distance to new overlapped stack, to pick the closer one
            float distanceToCurrentStack = (transform.position - overlappedStack.transform.position).magnitude;
            float distanceToNewStack = (transform.position - collision.gameObject.transform.position).magnitude;

            if (distanceToNewStack < distanceToCurrentStack)
            {
                overlappedStack = collision.gameObject;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Same as on enter collision, to ensure nearest stack is updated every frame
        if (!isDragging)
            return;

        if (collision.gameObject.Equals(transform.parent.gameObject))
            //Colliding stack is same as current card's stack, do nothing
            return;

        if (overlappedStack == null)
        {
            overlappedStack = collision.gameObject;
        }
        else
        {
            float distanceToCurrentStack = (transform.position - overlappedStack.transform.position).magnitude;
            float distanceToNewStack = (transform.position - collision.gameObject.transform.position).magnitude;

            if (distanceToNewStack < distanceToCurrentStack || overlappedStack.Equals(transform.parent.gameObject))
            {
                overlappedStack = collision.gameObject;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.Equals(transform.parent.gameObject))
            //Colliding stack is same as current card's stack, do nothing
            return;

        overlappedStack = null;
    }

    public void OnHoverEnter()
    {
        //GetComponent<SmoothScale>().Pulse(2f, 5);
    }

    private bool canDrag()
    {
        int currentIndex = transform.GetSiblingIndex();
        int totalCardNumber = transform.parent.childCount;
        ECardValue cardValue = transform.GetComponent<CardValue>().Value;

        if (!cardManager.GetComponent<CardManager>().GameManager.GetComponent<GameManager>().IsGameRunning)
            return false;

        if (GetComponent<CardState>().Flipped)
            return false;

        if (GetComponent<SmoothMovement>().IsMoving)
            return false;

        if (transform.parent.GetComponent<CardStackState>().IsStackCompleted)
            return false;

        if (currentIndex == totalCardNumber - 1)
            //Last card of the stack
            return true;

        for (int i = currentIndex + 1; i < totalCardNumber; i++)
        {
            //Make sure all the cards have the same value. Otherwise cannot grab
            CardValue c = transform.parent.GetChild(i).GetComponent<CardValue>();
            if (c.Value != cardValue)
                return false;
        }

        //All good
        return true;
    }

    public void BeginDrag()
    {
        if (canDrag())
        {
            //Cancel all glowing (Most likely hints)
            cardManager.cancelAllGlow();
            cardManager.GameManager.GetComponent<GameManager>().HintManager.GetComponent<HintManager>().CancelCurrentHint();

            draggedCards = new List<GameObject>();
            //Add all the cards of the same number from the one dragged. Already checked to make sure Value matches in canDrag()
            for (int i = transform.GetSiblingIndex() + 1; i < transform.parent.childCount; i++)
            {
                draggedCards.Add(transform.parent.GetChild(i).gameObject);
            }

            isDragging = true;
            startPosition = transform.position;

            offset = Input.mousePosition - transform.position;
            overlappedStack = transform.parent.gameObject;

            transform.parent.SetAsLastSibling();
            transform.SetAsLastSibling();
            GetComponent<Glow>().ShowGlow(Glow.COLOR_DRAGGING);
            foreach (GameObject c in draggedCards)
            {
                DragAndDrop dad = c.GetComponent<DragAndDrop>();
                dad.isDragging = true;
                dad.isDragged = true;
                dad.startPosition = c.transform.position;
                dad.offset = Input.mousePosition - c.transform.position;
                dad.overlappedStack = transform.parent.gameObject;

                //Make sure dragged cards are all in order to render properly
                c.transform.SetAsLastSibling();
                c.GetComponent<Glow>().ShowGlow(Glow.COLOR_DRAGGING);
            }
        }        
    }

    public void EndDrag()
    {
        if (isDragging)
        {
            isDragging = false;

            GetComponent<Glow>().CancelGlow();
            cardManager.GameManager.GetComponent<GameManager>().HintManager.GetComponent<HintManager>().CancelCurrentHint();

            foreach (GameObject c in draggedCards)
            {
                c.GetComponent<DragAndDrop>().isDragging = false;
                c.GetComponent<DragAndDrop>().isDragged = false;
                c.GetComponent<Glow>().CancelGlow();
            }

            bool canDrop = true;

            if (overlappedStack == null ||
                overlappedStack.Equals(transform.parent.gameObject) ||
                overlappedStack.GetComponent<CardStackState>().CurrentStack + (draggedCards == null ? 0 : draggedCards.Count) >= CardStackState.MaxStack)
                //If stack cannot accept all the cards dragged
                canDrop = false;
            else if (overlappedStack.transform.childCount > 0)
            {
                GameObject lastchild = overlappedStack.transform.GetChild(overlappedStack.transform.childCount - 1).gameObject;
                ECardValue lastchildValue = lastchild.GetComponent<CardValue>().Value;
                ECardValue cardValue = transform.GetComponent<CardValue>().Value;
                if (lastchildValue != cardValue)
                    //Last card in stack does not have same Card Value, cannot drop
                    canDrop = false;
            }

            if (!canDrop) {
                GetComponent<SmoothMovement>().MoveTo(startPosition, CardManager.TIME_MOVE_CARD);

                foreach (GameObject c in draggedCards)
                {
                    c.GetComponent<SmoothMovement>().MoveTo(c.GetComponent<DragAndDrop>().startPosition, CardManager.TIME_MOVE_CARD);
                }
            }
            else
            {
                //Add move to undo redo manager
                undoManager.AddMove(transform.gameObject, draggedCards, transform.parent.gameObject, overlappedStack.gameObject);

                GetComponent<SmoothMovement>().MoveTo(overlappedStack.GetComponent<CardStackState>().GetNextCardPosition, CardManager.TIME_MOVE_CARD);
                transform.SetParent(overlappedStack.transform);
                foreach (GameObject c in draggedCards)
                {
                    c.GetComponent<SmoothMovement>().MoveTo(overlappedStack.GetComponent<CardStackState>().GetNextCardPosition, CardManager.TIME_MOVE_CARD);
                    c.transform.SetParent(overlappedStack.transform);
                    c.GetComponent<DragAndDrop>().overlappedStack = null;
                }
                overlappedStack = null;
            }
        }        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoRedoManager : MonoBehaviour
{
    public GameObject BtnUndo;
    public GameObject BtnRedo;
    public GameObject Card_Manager;

    private Stack<GameMove> moves = new Stack<GameMove>();
    private Stack<GameMove> redoMoves = new Stack<GameMove>();

    void Start()
    {

    }

    void Update()
    {
        BtnUndo.GetComponent<DefaultButton>().Disabled = moves.Count <= 0;
        BtnRedo.GetComponent<DefaultButton>().Disabled = redoMoves.Count <= 0;
    }    

    public void Clear()
    {
        moves.Clear();
        redoMoves.Clear();
    }

    public void AddMove(GameObject mainCard, List<GameObject> cards, GameObject from, GameObject to)
    {
        List<GameObject> newList = new List<GameObject>();
        newList.Add(mainCard);
        newList.AddRange(cards);

        GameMove newMove = new GameMove();
        newMove.cards = newList;
        newMove.stackFrom = from;
        newMove.stackTo = to;

        moves.Push(newMove);

        //Clear redo when you make a new move
        redoMoves.Clear();

        transform.parent.gameObject.GetComponent<CardManager>().GameManager.GetComponent<GameManager>().addMove();
    }

    public void Undo()
    {
        if (moves.Count > 0)
        {
            transform.parent.gameObject.GetComponent<CardManager>().cancelAllGlow();
            transform.parent.gameObject.GetComponent<CardManager>().GameManager.GetComponent<GameManager>().HintManager.GetComponent<HintManager>().resetIdleTimer();

            //Remove the move from the stack to undo
            GameMove move = moves.Pop();

            foreach (GameObject c in move.cards)
            {
                move.stackFrom.transform.SetAsLastSibling();
                c.GetComponent<SmoothMovement>().MoveTo(move.stackFrom.GetComponent<CardStackState>().GetNextCardPosition, CardManager.TIME_MOVE_CARD);
                c.transform.SetParent(move.stackFrom.transform);
                c.transform.SetAsLastSibling();
            }

            //Add move into redo moves
            redoMoves.Push(move);

            transform.parent.gameObject.GetComponent<CardManager>().GameManager.GetComponent<GameManager>().addMove();
        }
    }

    public void Redo()
    {
        if (redoMoves.Count > 0)
        {
            transform.parent.gameObject.GetComponent<CardManager>().cancelAllGlow();
            transform.parent.gameObject.GetComponent<CardManager>().GameManager.GetComponent<GameManager>().HintManager.GetComponent<HintManager>().resetIdleTimer();

            GameMove move = redoMoves.Pop();

            foreach (GameObject c in move.cards)
            {
                move.stackTo.transform.SetAsLastSibling();
                c.GetComponent<SmoothMovement>().MoveTo(move.stackTo.GetComponent<CardStackState>().GetNextCardPosition, CardManager.TIME_MOVE_CARD);
                c.transform.SetParent(move.stackTo.transform);
                c.transform.SetAsLastSibling();
            }

            moves.Push(move);

            transform.parent.gameObject.GetComponent<CardManager>().GameManager.GetComponent<GameManager>().addMove();
        }
    }

    private class GameMove
    {
        public List<GameObject> cards;
        public GameObject stackFrom;
        public GameObject stackTo;
    }
}

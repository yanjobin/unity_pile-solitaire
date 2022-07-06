using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMovement : MonoBehaviour
{
    private bool isMoving = false;
    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
    }
    public bool forceStop = false;

    public void Move(Vector3 move, float duration)
    {
        if (IsMoving)
            forceStop = true;
        StartCoroutine(DoMove(move, duration));
    }

    private IEnumerator DoMove(Vector3 move, float duration)
    {
        yield return new WaitUntil(() => !isMoving);
        Vector3 destination = transform.position + move;
        yield return StartCoroutine(DoMoveTo(destination, duration));
    }

    public void MoveTo(Vector3 destination, float duration)
    {
        StartCoroutine(DoMoveTo(destination, duration));
    }

    private IEnumerator DoMoveTo(Vector3 destination, float duration)
    {
        isMoving = true;
        float counter = 0f;
        Vector3 startPosition = transform.position;

        while (!forceStop && counter < duration)
        {
            counter += Time.deltaTime;

            float t = counter / duration;
            t = t * t * (3f - 2f * t);

            Vector3 newPosition = Vector3.Lerp(startPosition, destination, t);

            transform.position = newPosition;
            yield return null;
        }

        transform.position = destination;
        isMoving = false;
        forceStop = false;
    }
}

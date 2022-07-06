using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRotate : MonoBehaviour
{
    private bool isRotating = false;
    private bool forceFinish = false;

    private float degrees;
    private Vector3 axis;

    public void Rotate(float degrees, Vector3 axis, float time)
    {
        this.degrees = degrees;
        this.axis = axis;

        StartCoroutine(DoRotate(this.gameObject, time));
    }

    private IEnumerator DoRotate(GameObject obj, float time)
    {
        if (isRotating)
        {
            forceFinish = true;
            yield return new WaitWhile(() => isRotating);
        }            

        isRotating = true;
        float counter = 0;

        Vector3 initialRotation = obj.transform.eulerAngles;
        Vector3 targetRotation = initialRotation + (axis * degrees);

        while (!forceFinish && counter < time)
        {
            counter += Time.deltaTime;
            Vector3 newRotation = Vector3.Lerp(initialRotation, targetRotation, counter / time);

            obj.transform.rotation = Quaternion.Euler(newRotation);
            yield return null;
        }

        obj.transform.rotation = Quaternion.Euler(targetRotation);
        isRotating = false;
        forceFinish = false;
    }
}

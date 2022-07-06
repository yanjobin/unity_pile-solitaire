using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothScale : MonoBehaviour
{
    public static float HINT_SMALL_PULSE_SCALE = 1.035f;
    public static float HINT_SMALL_PULSE_TIME = 0.2f;

    public static float FLIP_SMALL_PULSE_SCALE = 1.1f;
    public static float FLIP_SMALL_PULSE_TIME = 0.25f;

    private bool isScaling = false;
    public bool IsScaling
    {
        get
        {
            return isScaling;
        }
    }

    private Vector3 initialScale;
    private float currentScaleMultiply = -1;
    private float currentPulseTime = 1;
    private float currentPulseAmount = -1;

    private bool started = false;

    public void Start()
    {
        initialScale = transform.localScale;
        started = false;
    }

    public void resetScale()
    {
        isScaling = false;
    }

    public void Pulse(float scaleMultiply, float pulseTime, float pulseAmount = -1)
    {
        if (!started)
            Start();

        currentScaleMultiply = scaleMultiply;
        currentPulseTime = pulseTime;
        currentPulseAmount = pulseAmount;

        if (!isScaling)
        {
            StartCoroutine(DoPulse());
        }
    }

    private IEnumerator DoPulse()
    {
        isScaling = true;
        float counter = 0f;

        while (isScaling && (currentPulseAmount <= 0 ? true : counter < (currentPulseTime * currentPulseAmount)))
        {
            counter += Time.deltaTime;

            float calculatedScale = Mathf.Cos((counter / currentPulseTime) * (Mathf.PI * 2) + Mathf.PI) * (currentScaleMultiply - 1);
            calculatedScale = (calculatedScale + (currentScaleMultiply - 1)) / 2;
            Vector3 newScale = initialScale + initialScale * calculatedScale;
            transform.localScale = newScale;

            yield return null;
        }

        transform.localScale = initialScale;
        isScaling = false;
    }
}

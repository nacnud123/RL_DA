using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float shakeDuration = 0f;
    // Desired duration of the shake effect
    private float currShakeDuration = 0f;
    // A measure of magnitude for the shake
    public float shakeMagnitude = .7f;
    // A measure of how quickly the shake effect should evaporate
    public float dampingSpeed = 1.0f;
    // The initial position of the gameobject
    Vector3 initPosition;

    private void OnEnable()
    {
        initPosition = transform.localPosition;
    }

    private void Update()
    {
        if(currShakeDuration > 0)
        {
            transform.localPosition = initPosition + Random.insideUnitSphere * shakeMagnitude;

            currShakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            currShakeDuration = 0f;
            transform.localPosition = initPosition;
        }
    }

    public void TriggerShake()
    {
        currShakeDuration = shakeDuration;
    }
}

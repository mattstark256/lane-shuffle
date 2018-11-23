using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousPulse : MonoBehaviour
{
    [SerializeField]
    private float pulseDuration = 2;
    [SerializeField]
    private float pulseAmount = 0.2f;

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        transform.localScale = initialScale * (1 + pulseAmount * Mathf.Sin(Time.time * 2 * Mathf.PI / pulseDuration));
    }
}

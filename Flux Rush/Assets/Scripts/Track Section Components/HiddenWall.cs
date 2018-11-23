using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenWall : MonoBehaviour
{
    [SerializeField]
    private float xOffset = 0.3f;
    [SerializeField]
    private float zOffset = 3f;
    [SerializeField]
    private float slideZPosition = 6f;
    [SerializeField]
    private float slideDuration = 0.5f;
    [SerializeField]
    private int zDirection;

    private int xDirection;
    private bool hasSlid = false;

    private void Awake()
    {
        xDirection = (Random.value < 0.5f) ? -1 : 1;
    }

    void Start()
    {
        transform.localPosition += Vector3.right * xDirection * xOffset;
        transform.localPosition += Vector3.forward * zDirection * zOffset;
    }

    void Update()
    {
        if (!hasSlid && transform.localPosition.z - zOffset * zDirection < slideZPosition)
        {
            hasSlid = true;
            StartCoroutine(SlideOut());
        }
    }

    private IEnumerator SlideOut()
    {
        float f = 0;
        float oldSmoothedF = 0;
        while (f < 1)
        {
            f += Time.deltaTime / slideDuration;
            f = Mathf.Clamp01(f);
            float smoothedF = (1 - Mathf.Cos(f * Mathf.PI)) / 2;
            float deltaSmoothedF = smoothedF - oldSmoothedF;
            oldSmoothedF = smoothedF;

            transform.localPosition += Vector3.forward * deltaSmoothedF * zOffset * (-zDirection);

            yield return null;
        }
    }
}

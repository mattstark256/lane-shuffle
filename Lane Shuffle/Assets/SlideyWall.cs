using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideyWall : MonoBehaviour
{
    [SerializeField]
    private float distanceFromCentre = 0.3f;
    [SerializeField]
    private float slideZDistance = 6f;
    [SerializeField]
    private float slideDuration = 0.5f;

    private int direction;
    private bool hasSlid = false;

    private void Awake()
    {
        direction = (Random.value < 0.5f) ? -1 : 1;
    }

    void Start()
    {
        SetXPosition(direction * distanceFromCentre);
    }

    void Update()
    {
        if (!hasSlid && transform.localPosition.z < slideZDistance)
        {
            hasSlid = true;
            StartCoroutine(SlideAcross());
        }
    }

    private IEnumerator SlideAcross()
    {
        float f = 0;
        while (f < 1)
        {
            f += Time.deltaTime / slideDuration;
            f = Mathf.Clamp01(f);

            float offset = Mathf.Cos(f * Mathf.PI);
            SetXPosition(direction * distanceFromCentre * offset);

            yield return null;
        }
    }

    private void SetXPosition(float xPosition)
    {
        transform.localPosition = new Vector3(xPosition, transform.localPosition.y, transform.localPosition.z);
    }


}

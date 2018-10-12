using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterSwitcher : MonoBehaviour {

    [SerializeField]
    private RectTransform letter1;
    [SerializeField]
    private RectTransform letter2;
    [SerializeField]
    private float switchDuration = 1;
    [SerializeField]
    private float verticalRadius = 100;

    private Vector3 centerPosition;
    private float horizontalRadius;

    // Use this for initialization
    void Start () {
        centerPosition = (letter1.localPosition + letter2.localPosition)/2;
        horizontalRadius = (centerPosition - letter1.localPosition).x;
        //InvokeRepeating("SwitchLetters", 1, 2);
        StartCoroutine(SwitchLettersCoroutine());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SwitchLetters()
    {
        StartCoroutine(SwitchLettersCoroutine());
    }

    private IEnumerator SwitchLettersCoroutine()
    {
        yield return new WaitForSeconds(1);

        float f = 0;
        while (f < 1)
        {
            f += Time.deltaTime / switchDuration;
            f = Mathf.Clamp01(f);
            float smoothedF = Mathf.SmoothStep(0, 1, f);

            Vector3 offsetX = Vector3.right * horizontalRadius * Mathf.Cos(smoothedF * Mathf.PI);
            Vector3 offsetY = Vector3.up * verticalRadius * Mathf.Sin(smoothedF * Mathf.PI);
            letter1.localPosition = centerPosition + offsetX;
            letter2.localPosition = centerPosition - offsetX + offsetY;

            yield return null;
        }

        StartCoroutine(SwitchLettersCoroutine());
    }
}

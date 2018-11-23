using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleShaderController : MonoBehaviour
{
    [SerializeField]
    private Image imageA;
    private Material matA;
    [SerializeField]
    private Image imageB;
    private Material matB;
    // Weird note: If you declare two variables after a SerializeField (eg [SerializeField] private int a, b;), it works fine, but Unity shows a warning in the console.

    [SerializeField]
    private float animationDuration = 1;
    [SerializeField]
    private float animationInterval = 1;

    private float nextAnimationTime;

    // Use this for initialization
    void Start()
    {
        matA = imageA.material;
        matB = imageB.material;

        matA.SetFloat("_LowerCutoff", 0);
        matB.SetFloat("_LowerCutoff", 0);
        matA.SetFloat("_UpperCutoff", 0.5f);
        matB.SetFloat("_UpperCutoff", 0.5f);

        nextAnimationTime = animationInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextAnimationTime)
        {
            StartCoroutine(DoTitleAnimation());
            nextAnimationTime += animationDuration;
            nextAnimationTime += animationInterval;
        }
    }

    private IEnumerator DoTitleAnimation()
    {
        float f = 0;
        while (f < 1)
        {
            f += Time.deltaTime / animationDuration;
            f = Mathf.Clamp01(f);

            float smoothedF = Mathf.SmoothStep(0, 1, f);

            float lower = smoothedF;
            float upper = (smoothedF + 0.5f) % 1f;

            matA.SetFloat("_LowerCutoff", lower);
            matB.SetFloat("_LowerCutoff", lower);
            matA.SetFloat("_UpperCutoff", upper);
            matB.SetFloat("_UpperCutoff", upper);

            yield return null;
        }
    }
}

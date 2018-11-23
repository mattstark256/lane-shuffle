using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewTopScoreEffect : MonoBehaviour
{
    [SerializeField]
    private SoundEffectManager soundEffectManager;

    [SerializeField]
    private Transform message;
    [SerializeField]
    private Image fadeImage;
    [SerializeField]
    private float inflateDuration = 0.5f;

    [SerializeField]
    private float pulseDuration = 0.5f;
    [SerializeField]
    private float pulseAmount = 0.2f;

    private Vector3 finalMessageScale;


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    DoNewTopScoreEffect();
        //}
    }


    void Start()
    {
        finalMessageScale = message.localScale;
        message.localScale = Vector3.zero;
    }


    public void DoNewTopScoreEffect()
    {
        soundEffectManager.PlayEffect("Fanfare");

        StartCoroutine(InflateMessage());
    }


    private IEnumerator InflateMessage()
    {
        float f = 0;
        while (f < 1)
        {
            f += Time.deltaTime / inflateDuration;
            f = Mathf.Clamp01(f);

            message.transform.localScale = finalMessageScale * Mathf.Sin(f * Mathf.PI / 2);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1-f);

            yield return null;
        }

        float p = 0;
        while (true)
        {
            p += Time.deltaTime / pulseDuration;
            float c = (1 - Mathf.Cos(p * Mathf.PI * 2)) / 2;
            message.transform.localScale = finalMessageScale * (1 - pulseAmount * c);

            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour
{
    [SerializeField]
    private string messageBeforeScore = "Score: ";

    [SerializeField]
    private float bounceAmount = 2;
    [SerializeField]
    private float bounceSpeed = 10;

    Text text;
    Vector3 defaultScale;

    private void Awake()
    {
        text = GetComponent<Text>();
        text.text = "";

        defaultScale = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, Time.deltaTime * bounceSpeed);
    }

    public void UpdateScore(int newScore)
    {
        text.text = messageBeforeScore + newScore;
    }

    public void Bounce()
    {
        transform.localScale = defaultScale * bounceAmount;
    }
}

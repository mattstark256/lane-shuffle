using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NewTopScoreEffect))]
public class ScoreCounter : MonoBehaviour
{
    private NewTopScoreEffect topScoreEffect;

    [SerializeField]
    private ScoreText inGameScoreText;

    public int Score { get; private set; }
    private bool hasNewTopScore = false;


    private void Awake()
    {
        topScoreEffect = GetComponent<NewTopScoreEffect>();
    }


    private void Start()
    {
        inGameScoreText.UpdateScore(0);
    }


    public void AddToScore(int amount)
    {
        Score += amount;
        inGameScoreText.UpdateScore(Score);
        inGameScoreText.Bounce();

        int topScore = PlayerPrefs.GetInt("Top Score");
        if (!hasNewTopScore &&
            topScore > 0 &&
            Score > topScore)
        {
            hasNewTopScore = true;
            topScoreEffect.DoNewTopScoreEffect();
        }
    }
}

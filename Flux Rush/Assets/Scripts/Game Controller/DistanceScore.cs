using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameOver))]
[RequireComponent(typeof(TrackObjectManager))]
[RequireComponent(typeof(ScoreCounter))]
public class DistanceScore : MonoBehaviour
{
    private GameOver gameOver;
    private TrackObjectManager trackObjectManager;
    private ScoreCounter scoreCounter;

    [SerializeField]
    private float firstScoreDistance = 10f;
    [SerializeField]
    private float sectionLength = 3f;
    [SerializeField]
    private int scorePerSection = 1;

    private float distanceUntilNextScoreIncrease;

    private void Awake()
    {
        gameOver = GetComponent<GameOver>();
        trackObjectManager = GetComponent<TrackObjectManager>();
        scoreCounter = GetComponent<ScoreCounter>();

        distanceUntilNextScoreIncrease = firstScoreDistance;
    }

	void Update ()
    {
        distanceUntilNextScoreIncrease -= trackObjectManager.MoveDelta;
        if (distanceUntilNextScoreIncrease < 0 && !gameOver.GameIsOver)
        {
            scoreCounter.AddToScore(scorePerSection);
            distanceUntilNextScoreIncrease += sectionLength;
        }
    }
}

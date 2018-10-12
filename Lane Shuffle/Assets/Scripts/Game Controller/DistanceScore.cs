using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController), typeof(TrackObjectManager))]
public class DistanceScore : MonoBehaviour
{
    private GameController gameController;
    private TrackObjectManager trackObjectManager;

    [SerializeField]
    private float firstScoreDistance = 10f;
    [SerializeField]
    private float sectionLength = 3f;
    [SerializeField]
    private int scorePerSection = 1;

    private float distanceUntilNextScoreIncrease;

    private void Awake()
    {
        gameController = GetComponent<GameController>();
        trackObjectManager = GetComponent<TrackObjectManager>();

        distanceUntilNextScoreIncrease = firstScoreDistance;
    }

	void Update ()
    {
        distanceUntilNextScoreIncrease -= trackObjectManager.MoveDelta;
        if (distanceUntilNextScoreIncrease < 0 && gameController.GameIsInProgress)
        {
            gameController.AddToScore(scorePerSection);
            distanceUntilNextScoreIncrease += sectionLength;
        }
    }
}

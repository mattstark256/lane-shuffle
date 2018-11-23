using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameOver))]
public class TrackSpeed : MonoBehaviour
{
    private GameOver gameOver;

    [SerializeField]
    private float initialTrackSpeed = 1.25f;
    [SerializeField]
    private float finalTrackSpeed = 4.5f;
    [SerializeField]
    private float initialTrackGradient = 0.005f;

    [SerializeField]
    private float accelerationRate = 3f;

    private float gameTime = 0;
    private float targetSpeed = 0;
    public float Speed { get; private set; }


    private void Awake()
    {
        gameOver = GetComponent<GameOver>();
    }


    void Update ()
    {
        UpdateTargetSpeed();
        UpdateSpeed();
    }


    private void UpdateTargetSpeed()
    {
        if (!gameOver.GameIsOver)
        {
            gameTime += Time.deltaTime;
            targetSpeed = Mathf.Lerp(initialTrackSpeed, finalTrackSpeed, Mathf.Atan(gameTime * initialTrackGradient));
        }
        else
        {
            targetSpeed = 0;
        }
    }


    private void UpdateSpeed()
    {
        if (Speed < targetSpeed)
        {
            Speed += accelerationRate * Time.deltaTime;
            if (Speed > targetSpeed) { Speed = targetSpeed; }
        }

        if (Speed > targetSpeed)
        {
            Speed -= accelerationRate * Time.deltaTime;
            if (Speed < targetSpeed) { Speed = targetSpeed; }
        }
    }
}

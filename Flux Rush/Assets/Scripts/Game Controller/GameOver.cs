using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MouseAndTouchManager))]
[RequireComponent(typeof(ScoreCounter))]
public class GameOver : MonoBehaviour
{
    private MouseAndTouchManager mouseAndTouchManager;
    private ScoreCounter scoreCounter;

    [SerializeField]
    private SoundEffectManager soundEffectManager;
    [SerializeField]
    private MusicManager musicManager;

    [SerializeField]
    private float gameOverDelay = 1f;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private Text finalScoreText;
    [SerializeField]
    private GameObject movementButtons;

    public bool GameIsOver { get; private set; }


    private void Awake()
    {
        mouseAndTouchManager = GetComponent<MouseAndTouchManager>();
        scoreCounter = GetComponent<ScoreCounter>();
    }


    private void Start()
    {
        gameOverPanel.SetActive(false);
    }


    public void DoGameOver() { StartCoroutine(GameOverCoroutine()); }
    private IEnumerator GameOverCoroutine()
    {
        GameIsOver = true;
        mouseAndTouchManager.SetInputEnabled(false);
        movementButtons.SetActive(false);
        musicManager.EndMusic();
        soundEffectManager.PlayEffect("Die");

        yield return new WaitForSeconds(gameOverDelay);

        gameOverPanel.SetActive(true);
        int topScore = PlayerPrefs.GetInt("Top Score");
        int finalScore = scoreCounter.Score;
        // 0 can't be a top score because they haven't scored anything.
        if (topScore > 0)
        {
            if (finalScore > topScore)
            {
                finalScoreText.text = "New record!\nYour score: " + finalScore + "\nPrevious top score: " + topScore;
                PlayerPrefs.SetInt("Top Score", finalScore);
            }
            else
            {
                finalScoreText.text = "Your score: " + finalScore + "\nTop score: " + topScore;
            }
        }
        else
        {
            finalScoreText.text = "Your score: " + finalScore;
            PlayerPrefs.SetInt("Top Score", finalScore);
        }
    }
}

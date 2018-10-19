using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TrackObjectManager), typeof(MouseAndTouchManager))]
public class GameController : MonoBehaviour
{
    private TrackObjectManager trackObjectManager;
    private MouseAndTouchManager mouseAndTouchManager;

    [SerializeField]
    private SoundEffectManager soundEffectManager;
    [SerializeField]
    private MusicManager musicManager;

    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private ScoreText inGameScoreText;
    [SerializeField]
    private Text finalScoreText;
    [SerializeField]
    private GameObject movementButtons;

    [SerializeField]
    private float initialTrackSpeed = 1f;
    [SerializeField]
    private float finalTrackSpeed = 2f;
    [SerializeField]
    private float initialTrackGradient = 0.01f;
    private float trackSpeed = 0f;

    [SerializeField]
    private float gameOverDelay = 1f;

    private int score = 0;
    private bool gameIsInProgress = true;
    public bool GameIsInProgress { get { return gameIsInProgress; } }
    private float gameTime = 0;


    private void Awake()
    {
        trackObjectManager = GetComponent<TrackObjectManager>();
        mouseAndTouchManager = GetComponent<MouseAndTouchManager>();

        trackSpeed = initialTrackSpeed;
    }


    private void Start()
    {
        gameOverPanel.SetActive(false);
        inGameScoreText.UpdateScore(0);
    }


    private void Update()
    {
        // accelerate the track
        if (gameIsInProgress)
        {
            gameTime += Time.deltaTime;
            trackSpeed = Mathf.Lerp(initialTrackSpeed, finalTrackSpeed, Mathf.Atan(gameTime * initialTrackGradient));
            trackObjectManager.TargetMoveSpeed = trackSpeed;
        }
    }


    public void AddToScore(int amount)
    {
        score += amount;
        inGameScoreText.UpdateScore(score);
        inGameScoreText.Bounce();
    }


    public void GameOver()
    {
        trackObjectManager.TargetMoveSpeed = 0f;
        gameIsInProgress = false;
        mouseAndTouchManager.SetInputEnabled(false);
        movementButtons.SetActive(false);
        musicManager.EndMusic();
        soundEffectManager.PlayEffect("Die");

        StartCoroutine(GameOverCoroutine());
    }


    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(gameOverDelay);
        gameOverPanel.SetActive(true);

        int topScore = PlayerPrefs.GetInt("Top Score");
        // 0 can't be a top score because they haven't scored anything.
        if (topScore > 0)
        {
            if (score > topScore)
            {
                finalScoreText.text = "New record!\nYour score: " + score + "\nPrevious top score: " + topScore;
                PlayerPrefs.SetInt("Top Score", score);
            }
            else
            {
                finalScoreText.text = "Your score: " + score + "\nTop score: " + topScore;
            }
        }
        else
        {
            finalScoreText.text = "Your score: " + score;
            PlayerPrefs.SetInt("Top Score", score);
        }
    }


    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}

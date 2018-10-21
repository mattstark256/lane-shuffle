using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController))]
public class PauseMenu : MonoBehaviour
{
    private GameController gameController;

    [SerializeField, Tooltip("This is the parent to the menu pages and includes all generic menu elements")]
    private GameObject pauseMenuBase;
    [SerializeField]
    private GameObject mainPage;
    [SerializeField]
    private GameObject settingsPage;
    [SerializeField]
    private GameObject creditsPage;

    [SerializeField]
    private MusicManager musicManager;


    private void Awake()
    {
        gameController = GetComponent<GameController>();

        pauseMenuBase.SetActive(false);
    }


    public void OpenPauseMenu()
    {
        pauseMenuBase.SetActive(true);
        Time.timeScale = 0;
        musicManager.PauseMusic();

        CloseAllPages();
        mainPage.SetActive(true);
    }


    public void CloseMenu()
    {
        pauseMenuBase.SetActive(false);
        Time.timeScale = 1;
        musicManager.PlayMusic();
    }


    public void SwitchToSettings()
    {
        CloseAllPages();
        settingsPage.SetActive(true);
    }


    public void SwitchToCredits()
    {
        CloseAllPages();
        creditsPage.SetActive(true);
    }


    public void RestartGame()
    {
        Time.timeScale = 1;
        gameController.ReloadScene();
    }


    private void CloseAllPages()
    {
        mainPage.SetActive(false);
        settingsPage.SetActive(false);
        creditsPage.SetActive(false);
    }
}

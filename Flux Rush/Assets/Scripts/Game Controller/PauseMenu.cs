using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController))]
public class PauseMenu : MonoBehaviour
{
    private GameController gameController;

    [SerializeField, Tooltip("This is the parent to the menu pages and includes all generic menu elements")]
    private GameObject pauseMenuBase;
    [SerializeField, Tooltip("This the button used to return to the main page")]
    private GameObject backButton;
    [SerializeField]
    private GameObject mainPage;
    [SerializeField]
    private GameObject settingsPage;
    [SerializeField]
    private GameObject creditsPage;
    [SerializeField]
    private GameObject instructionsPage;

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
        SwitchToMainPage();
    }


    public void ClosePauseMenu()
    {
        pauseMenuBase.SetActive(false);
        Time.timeScale = 1;
        musicManager.PlayMusic();
    }


    public void SwitchToMainPage()
    {
        CloseAllPages();
        mainPage.SetActive(true);
    }


    public void SwitchToSettingsPage()
    {
        CloseAllPages();
        settingsPage.SetActive(true);
        backButton.SetActive(true);
    }


    public void SwitchToCreditsPage()
    {
        CloseAllPages();
        creditsPage.SetActive(true);
        backButton.SetActive(true);
    }


    public void SwitchToInstructionsPage()
    {
        CloseAllPages();
        instructionsPage.SetActive(true);
        backButton.SetActive(true);
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
        instructionsPage.SetActive(false);
        backButton.SetActive(false);
    }
}

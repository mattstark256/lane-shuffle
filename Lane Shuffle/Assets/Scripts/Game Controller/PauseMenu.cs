using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController))]
public class PauseMenu : MonoBehaviour
{
    private GameController gameController;

    [SerializeField]
    private GameObject inGameMenu;
    [SerializeField]
    private MusicManager musicManager;


    private void Awake()
    {
        gameController = GetComponent<GameController>();

        inGameMenu.SetActive(false);
    }


    public void OpenMenu()
    {
        inGameMenu.SetActive(true);
        Time.timeScale = 0;
        musicManager.PauseMusic();
    }


    public void CloseMenu()
    {
        inGameMenu.SetActive(false);
        Time.timeScale = 1;
        musicManager.PlayMusic();
    }


    public void RestartGame()
    {
        Time.timeScale = 1;
        gameController.ReloadScene();
    }
}

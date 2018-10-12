using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController))]
public class PauseMenu : MonoBehaviour
{
    private GameController gameController;

    [SerializeField]
    private GameObject inGameMenu;


    private void Awake()
    {
        gameController = GetComponent<GameController>();

        inGameMenu.SetActive(false);
    }


    public void OpenMenu()
    {
        inGameMenu.SetActive(true);
        Time.timeScale = 0;
    }


    public void CloseMenu()
    {
        inGameMenu.SetActive(false);
        Time.timeScale = 1;
    }


    public void RestartGame()
    {
        Time.timeScale = 1;
        gameController.ReloadScene();
    }
}

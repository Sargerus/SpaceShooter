using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private GameObject _pauseMenu;

    enum GameStatus
    {
        Paused = 0,
        Active = 1
    }

    private void Start()
    {
        StartCoroutine(GetPauseMenuGM());        
    }

    private IEnumerator GetPauseMenuGM()
    {
        var canvasGM = GameObject.Find("Canvas");
        if (canvasGM == null)
        {
            Debug.LogWarning("PauseMenu: _canvas is null");
        }
    
        while (true)
        {
            foreach (Transform transform in canvasGM.transform)
            {
                if (transform.CompareTag("PauseMenu"))
                {
                    _pauseMenu = transform.gameObject;
                }
            }
    
            if(_pauseMenu != null)
            {
                break;
            }
            else yield return new WaitForSeconds(0.1f);
        }
    
        GetComponent<PlayerInput>().currentActionMap.FindAction("PauseMenu").performed += Game_Manager_performed_main_menu;
        yield break;
    }

    private void Game_Manager_performed_main_menu(InputAction.CallbackContext obj)
    {
        //already in pause
        if (Time.timeScale == 0) { ChangeGameStatus(GameStatus.Active); return; }

        ChangeGameStatus(GameStatus.Paused);
    }

    public void Resume()
    {
        ChangeGameStatus(GameStatus.Active);
    }

    public void BackToMainMenu()
    {
        ChangeGameStatus(GameStatus.Active);
        SceneManager.LoadScene(0);
    }

    private void ChangeGameStatus(GameStatus status)
    {
        switch (status)
        {
            case GameStatus.Paused:
                {
                    if (_pauseMenu != null)
                    {
                        Time.timeScale = 0;
                        _pauseMenu.SetActive(true);
                    }
                }; break;
            case GameStatus.Active:
                {
                    if (_pauseMenu != null)
                    {
                        Time.timeScale = 1;
                        _pauseMenu.SetActive(false);
                    }
                }; break;
        }
    }
}

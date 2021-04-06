using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    private static GlobalVariables.GameplayMode _gameplayMode;

    [SerializeField]
    private GameObject _spawnManager;

    [SerializeField]
    private Image _wasdImg;
    private Vector3 _wasdImgDefaultPosition;

    [SerializeField]
    private Image _spaceImg;
    private Vector3 _spaceImgDefaultPosition;

    private bool _isGameOver;
    private bool _isGameStarted;

    private GameObject _mainPlayer;
    public void GameOver()
    {
        _isGameOver = true;
        _isGameStarted = false;
    }

    public static void SetGameplayMode(GlobalVariables.GameplayMode mode)
    {
        _gameplayMode = mode;
    }
    public static GlobalVariables.GameplayMode GetGameplayMode()
    {
        return _gameplayMode;
    }

    public void Start()
    {
        GetComponent<PlayerInput>().currentActionMap.FindAction("Restart").performed += Game_Manager_performed_restart;
        //GetComponent<PlayerInput>().currentActionMap.FindAction("Quit").performed += Game_Manager_performed_quit;

        if (_gameplayMode.Equals(GlobalVariables.GameplayMode.SinglePlayer))
        {
            _wasdImgDefaultPosition = _wasdImg.transform.position;
            _spaceImgDefaultPosition = _spaceImg.transform.position;

            _wasdImg.gameObject.SetActive(false);
            _spaceImg.gameObject.SetActive(false);

            _isGameStarted = false;

            if (_isGameOver == true)
            {
                _isGameOver = false;
            }
            else
            {
                StartCoroutine(Tutorial());
            }
        } else { StartGame(); }
    }

    private void Game_Manager_performed_quit(InputAction.CallbackContext obj)
    {
        Application.Quit();
    }

    private IEnumerator Tutorial()
    {
        GameObject playerPrefab = (GameObject)Resources.Load("Prefabs/Player/Player", typeof(GameObject));
        _mainPlayer = GameObject.Instantiate(playerPrefab, new Vector3(0, -6.0f, 0), Quaternion.identity);
        
        if(_mainPlayer == null)
        {
            Debug.LogError("Game_Manger: _mainPlayer is null");
            yield break;
        }

        _mainPlayer.GetComponent<PlayerInput>().DeactivateInput();

        while (_mainPlayer.transform.position != new Vector3(0,0,0))
        {
            _mainPlayer.transform.Translate(new Vector3(0, GlobalVariables.playerSpeed * Time.deltaTime, 0));
            yield return new WaitForSeconds(0.016f);
        }

        _mainPlayer.GetComponent<PlayerInput>().ActivateInput();

        _wasdImg.gameObject.SetActive(true);
        _spaceImg.gameObject.SetActive(true);

        StartCoroutine(WaitForPlayerSpaceBeforeStart());
    }

    private IEnumerator WaitForPlayerSpaceBeforeStart()
    {
        GetComponent<PlayerInput>().currentActionMap.FindAction("SpaceToStart").performed += Game_Manager_performed_space_to_start;

        while (true)
        {
            if (_isGameStarted)
            {
                GetComponent<PlayerInput>().currentActionMap.FindAction("SpaceToStart").performed -= Game_Manager_performed_space_to_start;
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void Game_Manager_performed_restart(InputAction.CallbackContext obj)
    {
        if (_isGameOver)
        {
            _isGameOver = false;

            if (_gameplayMode.Equals(GlobalVariables.GameplayMode.SinglePlayer))
            {
                SceneManager.LoadScene(1);
            }
            else { SceneManager.LoadScene(2); }
        }
    }

    private void Game_Manager_performed_space_to_start(InputAction.CallbackContext obj)
    {
        StartGame();
    }

    private void StartGame()
    {
        _isGameStarted = true;
        _spawnManager.GetComponent<SpawnManager>().StartDefaultSpawningLogic();
        _wasdImg.gameObject.SetActive(false);
        _spaceImg.gameObject.SetActive(false);
    }
}

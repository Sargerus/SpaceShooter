using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    private const string _bestScorePlayerPrefsString = "BestScore";
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _bestScoreText;

    [SerializeField]
    private Sprite[] _liveSprites;

    [SerializeField]
    private Image _LiveImg;

    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Text _restartText;

    private int _score;
    private int _bestScore;

    // Start is called before the first frame update
    void Start()
    {
        _LiveImg.sprite = _liveSprites[3];
        _gameOverText.gameObject.SetActive(false);
        _scoreText.text = "Score: " + _score;
        _restartText.gameObject.SetActive(false);

        _bestScore = PlayerPrefs.GetInt(_bestScorePlayerPrefsString, 0);
        StartCoroutine(SaveBestScore());
    }

    private IEnumerator SaveBestScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            if (_score >= _bestScore)
            {
                PlayerPrefs.SetInt(_bestScorePlayerPrefsString, _bestScore);
                PlayerPrefs.Save();
            }
                
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_score > _bestScore)
        {
            _bestScore = _score;
        }

        _scoreText.text = "Score: " + _score;
        _bestScoreText.text = "Best: " + _bestScore;
    }

    public void UpdateScore(int points) 
    {
        _score += points;
    }

    public void UpdateLives(int liveIndex)
    {
        if (liveIndex > 3) {
            _LiveImg.sprite = _liveSprites[3];
        }
        else if (liveIndex < 0)
        {
            _LiveImg.sprite = _liveSprites[3];
        }
        else
        {
            _LiveImg.sprite = _liveSprites[liveIndex];
        }
    }

    public void UIGameOver()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(gameOverFlickerRoutine());
    }

    private IEnumerator gameOverFlickerRoutine()
    {
        float flickerTime = 0.7f;
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(flickerTime);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(flickerTime);
        }
    }
}

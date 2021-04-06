using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Text _preesText;

    public void Start()
    {
        StartCoroutine(flickerText());
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    private IEnumerator flickerText()
    {
        float flickerTime = 0.7f;
        while (true)
        {
            _preesText.gameObject.SetActive(true);
            yield return new WaitForSeconds(flickerTime);
            _preesText.gameObject.SetActive(false);
            yield return new WaitForSeconds(flickerTime);
        }
    }
    
}

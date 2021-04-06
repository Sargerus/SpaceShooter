using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoopMenu : MonoBehaviour
{

    public void LoadSinglePlayer()
    {
        Game_Manager.SetGameplayMode(GlobalVariables.GameplayMode.SinglePlayer);
        SceneManager.LoadScene(1);        
    }

    public void LoadMultiplayer()
    {
        Game_Manager.SetGameplayMode(GlobalVariables.GameplayMode.MultiPlayers);
        SceneManager.LoadScene(2);
    }

}

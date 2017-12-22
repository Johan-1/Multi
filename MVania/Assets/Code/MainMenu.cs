using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour 
{

    [SerializeField] GameObject _player;

    public void NewGame()
    {
        SaveLoadManager.GetInstance.DeleteGame("Game.data");
        Instantiate(_player);
        SceneManager.LoadScene("Screen1");

    }

    public void LoadGame()
    {

        SaveLoadManager.GetInstance.LoadGame("Game.data");
        
    }

}

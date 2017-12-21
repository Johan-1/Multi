using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour 
{


    public void NewGame()
    {
        SceneManager.LoadScene("Screen1");

    }

    public void LoadGame()
    {

        print("Add load function");
    }

}

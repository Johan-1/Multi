using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;


public class MainMenu : MonoBehaviour 
{

    [SerializeField] GameObject _player;
    [SerializeField] GameObject _startScreen;
    [SerializeField] GameObject _LoadScreen;

    [SerializeField] Button[] _savefileButtons;

    GameObject _previousSelectedButton;

    SavefileInfoData[] _savefileInfoData;
   

    void Awake()
    {
        LoadSaveFileInfoData();
       
    }

    public void ToLoadFiles()
    {

        _startScreen.SetActive(false);
        _LoadScreen.SetActive(true);
        _previousSelectedButton = EventSystem.current.gameObject;
        EventSystem.current.SetSelectedGameObject(_LoadScreen.transform.GetChild(0).gameObject);
                        
    }

    public void QuitGame()
    {
        Application.Quit();        
    }

    public void FileSelected(int buttonId)
    {        
        StartGameWithFile("Game" + buttonId + ".data", buttonId);        
    }
    
    void StartGameWithFile(string file, int fileID)
    {

        if (File.Exists(Application.persistentDataPath + "/" + file))
        {            
            SaveLoadManager.GetInstance.LoadGame(file, _savefileInfoData[fileID].playername);
        }
        else
            StartNewGame(file,fileID);
    }

    void StartNewGame(string file, int fileID)
    {   
        //TODO: make players being able to type in a name
        
        SaveLoadManager.GetInstance.saveFile = file;
        GameObject player = Instantiate(_player);
        player.GetComponent<PlayerAbilitys>().playerName = "Player" + (fileID + 1); // hardcode name to "Player 1-3" for now
        SceneManager.LoadScene("Screen1");
    }


    void LoadSaveFileInfoData()
    {        
         _savefileInfoData = new SavefileInfoData[3];
                  
         for (int i = 0; i < 3; i++)
         {
             if (File.Exists(Application.persistentDataPath + "/InfoGame" + i + ".data"))
             {
                 BinaryFormatter bf = new BinaryFormatter();
                 FileStream fs = new FileStream(Application.persistentDataPath + "/InfoGame" + i + ".data", FileMode.Open);

                 _savefileInfoData[i] = (SavefileInfoData)bf.Deserialize(fs);

                 _savefileButtons[i].GetComponentInChildren<Text>().text = _savefileInfoData[i].playername;

                 fs.Close();
             }
             else
             {
                 _savefileButtons[i].GetComponentInChildren<Text>().text = "Empty";
             }

         }
        

    }

    

    
}

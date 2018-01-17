﻿using System.Collections; 
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

    [SerializeField] SaveFileButton[] _savefileButtons;
    
    [Serializable]
    struct SaveFileButton
    {
        public Button button;
        public Text name;
        public Text time;
        public Text healthPickup;
    }

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
        StartGameWithFile("Game" + buttonId + ".data");        
    }

    public void DeleteFileSelected(int buttonId)
    {
        DeleteGame("Game" + buttonId + ".data", buttonId);
    }
    
    void StartGameWithFile(string file)
    {

        if (File.Exists(Application.persistentDataPath + "/" + file))
        {            
            SaveLoadManager.GetInstance.LoadGame(file);
        }
        else
            StartNewGame(file);
    }

    void StartNewGame(string file)
    {   
                
        // create player     
        GameObject player = Instantiate(_player);

        // tell saveloadmanager witch file to save data to
        SaveLoadManager.GetInstance.saveFilename = file;

        // save base data to file, (if we die without having saved in game, this data will be loaded)
        SaveLoadManager.GetInstance.saveData.sceneData = new Scenedata("Screen1", new float[3] { 0, 3, 0 });
        SaveLoadManager.GetInstance.SaveGame(); 

        SceneManager.LoadScene("Screen1");
    }

    public void DeleteGame(string file, int fileId)
    {
        // try to find a specific file and delete it
        if (File.Exists(Application.persistentDataPath + "/" + file))
        {
            File.Delete(Application.persistentDataPath + "/" + file);
            File.Delete(Application.persistentDataPath + "/" + "Info" + file);

            _savefileButtons[fileId].name.text = "Empty";
            _savefileButtons[fileId].time.gameObject.SetActive(false);
            _savefileButtons[fileId].healthPickup.gameObject.SetActive(false);

        }
           
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

                // set texts on savefile to saved data 
                _savefileButtons[i].name.text = "Game " + (i + 1);
                _savefileButtons[i].time.text = "Time : " + _savefileInfoData[i].hours.ToString("00") + "." + _savefileInfoData[i].minutes.ToString("00");
                _savefileButtons[i].healthPickup.text = "HealthUpgrades : " + _savefileInfoData[i].numHealthUpgrades + "/10";

                 fs.Close();
             }
             else
             {
                // if file is not found we display it as an empty game
                 _savefileButtons[i].name.text = "Empty";
                _savefileButtons[i].time.gameObject.SetActive(false);
                _savefileButtons[i].healthPickup.gameObject.SetActive(false);
             }

         }
        

    }

    

    
}

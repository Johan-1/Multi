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

    [SerializeField] GameObject _startButton;

    [SerializeField] SaveFileButton[] _savefileButtons;

    // scales if button is selected or not
    [SerializeField] Vector2 _standardScale;
    [SerializeField] Vector2 _highlightScale;

    // the selected button colors depending on witch mode we are in
    [SerializeField] Color _standardColor;
    [SerializeField] Color _deleteColor;
    [SerializeField] Color _copyColor;

    // all properties of a button in loadgamescreen   
    [Serializable]
    struct SaveFileButton
    {
        public Button button;
        public Text name;
        public Text time;
        public Text healthPickup;
    }

    enum MENUSTATE
    {
        SELECTING,
        DELETING,
        COPYING,
    }

    Color _usingColor;
    string _fileToCopy = "";
    GameObject _buttonOfFileToCopy;
    MENUSTATE _currentState;
    GameObject _lastSelectedButton;

    void Awake()
    {
        // load info of our games
        LoadSaveFileInfoData();

        // set state to defualt selectingstate
        _currentState = MENUSTATE.SELECTING;
        
        // give the eventsystem a reference to first button and highlight this button
        EventSystem.current.SetSelectedGameObject(_startButton);       
        _startButton.transform.localScale = _highlightScale;

        _lastSelectedButton = _startButton;
        _usingColor = _standardColor;
    }

    void Update()
    {
        // check if the selected button has changed
        if ( _lastSelectedButton != EventSystem.current.currentSelectedGameObject)
        {

            // if mouse has been clicked refocus on last selected button
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
                return;
            }

            // if we are not copying a file we always set the last button back to original values
            if (_currentState != MENUSTATE.COPYING)
            {
                _lastSelectedButton.transform.localScale = _standardScale;
                _lastSelectedButton.GetComponent<Image>().color = _standardColor;
            }
            else if (_currentState == MENUSTATE.COPYING)
            {
                // if we are copying but we still havent chosen witch file to copy or if the chosen file is not == to the last button we also set back original values
                // this makes the selected file to copy stay blue untill we chose witch file to copy it too
                if (_buttonOfFileToCopy == null || _buttonOfFileToCopy != _lastSelectedButton)
                {
                    _lastSelectedButton.transform.localScale = _standardScale;
                    _lastSelectedButton.GetComponent<Image>().color = _standardColor;
                }              
            }
           
            // highlight the new button to the color of the mode we are in(regular, deleting,copying)
            EventSystem.current.currentSelectedGameObject.transform.localScale = _highlightScale;            
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = _usingColor;
            _lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }

        // dash is B button, setup own input for Cancel to avoid confusion
        // if in delete or copymode press B to cancel and go back to regular selectmode
        if (Input.GetButtonDown("Dash") && _currentState != MENUSTATE.SELECTING)
        {
            // set to standard mode
            _currentState = MENUSTATE.SELECTING;
            _usingColor = _standardColor;

            // set back current object to standardcolor
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = _standardColor;
           
            // if we are canceling in copy mode and alredy have selected a file to copy set that file back to original values 
            if (_buttonOfFileToCopy != null)
            {
                // only set back scale if we dont have the selectedbutton to copy as our current selected button
                if (_buttonOfFileToCopy != EventSystem.current.currentSelectedGameObject)
                    _buttonOfFileToCopy.transform.localScale = _standardScale;

                // always set color back
                _buttonOfFileToCopy.GetComponent<Image>().color = _standardColor;
                
                // null copy values
                _buttonOfFileToCopy = null;
                _fileToCopy = "";
            }            
        }
            
    }

    public void ToLoadFiles()
    {
        // startgame button, will take us to loadGame screen
        _startScreen.SetActive(false);
        _LoadScreen.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(_LoadScreen.transform.GetChild(0).gameObject);        
    }

    public void QuitGame()
    {
        Application.Quit();        
    }

    public void FileSelected(int buttonId)
    {
        // check witch state we are in when a file have been selected and call respective function
        if (_currentState == MENUSTATE.SELECTING)
            StartGameWithFile("Game" + buttonId + ".data");
        else if (_currentState == MENUSTATE.DELETING)
            DeleteFileSelected(buttonId);
        else if (_currentState == MENUSTATE.COPYING)
            CopyFileSelected(buttonId);

    }      

    public void DeleteButton()
    {
        // if transitioning from copystate to delete state make sure we reset values if a file to copy alredy have been selected
        if (_currentState == MENUSTATE.COPYING)
        {
            if (_buttonOfFileToCopy != null)
            {
                _buttonOfFileToCopy.transform.localScale = _standardScale;
                _buttonOfFileToCopy.GetComponent<Image>().color = _standardColor;

                _buttonOfFileToCopy = null;
                _fileToCopy = "";
            }
        }
        // change state to deleting and change color of current button
        _currentState = MENUSTATE.DELETING;
        _usingColor = _deleteColor;       
        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = _usingColor;

    }

    public void CopyButton()
    {
        // change state to copying and chnage color of currentbutton
        _currentState = MENUSTATE.COPYING;
        _usingColor = _copyColor;       
        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = _copyColor;

    }

    void DeleteFileSelected(int buttonId)
    {

        string file = "Game" + buttonId + ".data";  

        // try to find a specific file and delete it
        if (File.Exists(Application.persistentDataPath + "/" + file))
        {
            File.Delete(Application.persistentDataPath + "/" + file);
            File.Delete(Application.persistentDataPath + "/" + "Info" + file);

            _savefileButtons[buttonId].name.text = "Empty";
            _savefileButtons[buttonId].time.gameObject.SetActive(false);
            _savefileButtons[buttonId].healthPickup.gameObject.SetActive(false);

        }

        _currentState = MENUSTATE.SELECTING;
        _usingColor = _standardColor;
        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = _usingColor;
    }
    
    void CopyFileSelected(int buttonId)
    {
        // if file not been selected set the file to be copied to the current one
        if (_fileToCopy == "")
        {
            _fileToCopy = "Game" + buttonId + ".data";
            _buttonOfFileToCopy = EventSystem.current.currentSelectedGameObject;
        }
        // if file to copy have been selected, copy the file into the destination of the current selected 
        // also check so the file to copy and the current selected file is not the same
        else if (_buttonOfFileToCopy != EventSystem.current.currentSelectedGameObject) 
        {
            if (File.Exists(Application.persistentDataPath + "/" + _fileToCopy))
            {                
                File.Copy(Application.persistentDataPath + "/" + _fileToCopy, Application.persistentDataPath + "/" + "Game" + buttonId + ".data", true);
                File.Copy(Application.persistentDataPath + "/" + "Info" + _fileToCopy, Application.persistentDataPath + "/" + "InfoGame" + buttonId + ".data", true);              
            }

            // reset values after copying is done
            _usingColor = _standardColor;
            _currentState = MENUSTATE.SELECTING;

            _buttonOfFileToCopy.transform.localScale = _standardScale;
            _buttonOfFileToCopy.GetComponent<Image>().color = _standardColor;
            _fileToCopy = "";
            _buttonOfFileToCopy = null;

            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = _usingColor;
           
            // reload the info of the savefiles tho they have changed
            LoadSaveFileInfoData();
        }
    }
    
    void StartGameWithFile(string file)
    {
        // check if file exist and load it, else start new game
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
        Instantiate(_player);

        // tell saveloadmanager witch file to save data to
        SaveLoadManager.GetInstance.saveFilename = file;

        // save base data to file, (if we die without having saved in game, this data will be loaded)
        // basicly the sceneID and spawnpos of first room will be saved and all other data to startvalues
        SaveLoadManager.GetInstance.saveData.sceneData = new Scenedata("Screen1", new float[3] { 0, 3, 0 });
        SaveLoadManager.GetInstance.SaveGame();

        // set that we are in a game and pausmenu now can be used
        UIManager.GetInstance.pausMenu.canOpenMenu = true;

        SceneManager.LoadScene("Screen1");
    }  

    void LoadSaveFileInfoData()
    {        
        SavefileInfoData[] savefileInfoData = new SavefileInfoData[3];

        // load the info of each of our 3 savegames
        // we have 2 files witch we save data into per game
        // one for everything in the game
        // and one with selected data that we want acces to before we load a specific game, this is for UI in menu purposes
        for (int i = 0; i < 3; i++)
         {
             // if the file exist, load it and fill the ui properties to the data
             if (File.Exists(Application.persistentDataPath + "/InfoGame" + i + ".data"))
             {
                 BinaryFormatter bf = new BinaryFormatter();
                 FileStream fs = new FileStream(Application.persistentDataPath + "/InfoGame" + i + ".data", FileMode.Open);

                 savefileInfoData[i] = (SavefileInfoData)bf.Deserialize(fs);

                // set texts on savefile to saved data 
                _savefileButtons[i].name.text = "Game " + (i + 1);
                _savefileButtons[i].time.text = "Time : " + savefileInfoData[i].hours.ToString("00") + "." + savefileInfoData[i].minutes.ToString("00");
                _savefileButtons[i].healthPickup.text = "HealthUpgrades : " + savefileInfoData[i].numHealthUpgrades + "/10";

                _savefileButtons[i].time.gameObject.SetActive(true);
                _savefileButtons[i].healthPickup.gameObject.SetActive(true);

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

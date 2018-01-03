using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;


public class SaveLoadManager : MonoBehaviour
{

    [SerializeField] GameObject _player;

    // singleton pattern on this class
    static SaveLoadManager _instance;
    public static SaveLoadManager GetInstance {get { return _instance; } }

    // main savedata class that will hold other subdata classes
    SaveData _saveData = new SaveData();
    public SaveData saveData { get { return _saveData; } }

    // savedata class that will hold info about a savefile(name, time played ,progress etc)
    SavefileInfoData _savefileInfoData = new SavefileInfoData();
    public SavefileInfoData savefileInfoData { get { return _savefileInfoData; } }

    //list of all objects that will save thier data on save
    List<ISaveable> _saveableObjects = new List<ISaveable>();

    // witch file we are playing the game with
    string _saveFile; 
    public string saveFile { set { _saveFile = value; } }

    

    void Awake()
    {
        // initialize singleton
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else if (_instance != this)
        {
            DestroyImmediate(this);
        }
    }


    public void SaveGame()
    {
        // loop over list so all objects save thier current status to the main savedata class
        foreach (ISaveable obj in _saveableObjects)
            obj.SaveData();
            
        //save data to game savefile
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(Application.persistentDataPath + "/" + _saveFile ,FileMode.Create);        
        bf.Serialize(fs, _saveData);
        fs.Close();

        //save data to info savefile      
        fs = new FileStream(Application.persistentDataPath + "/Info" + _saveFile, FileMode.Create);
        bf.Serialize(fs, _savefileInfoData);
        fs.Close();

        print("saved to file " + _saveFile);


    }   

    public void LoadGame(string file, string name)
    {
        if (File.Exists(Application.persistentDataPath + "/" + file))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(Application.persistentDataPath + "/" + file, FileMode.Open);

            // load data into savefile
            _saveData = (SaveData)bf.Deserialize(fs);
            fs.Close();

            // load data into savefileinfodata
            fs = new FileStream(Application.persistentDataPath + "/Info" + file, FileMode.Open);
            _savefileInfoData = (SavefileInfoData)bf.Deserialize(fs);

            fs.Close();

            // set witch file we loaded so we know where to save new progress
            _saveFile = file;

            // tell the gameProgressManager to set its data that we just loaded
            // this have info of example defeated bosses, picked up items, locked doors etc 
            GameProgressManager.GetInstance.SetupSaveData();

            // create player and load the scene where we last saved
            GameObject player = Instantiate(_player);
            player.GetComponent<PlayerMovement>().transform.position = new Vector3( _saveData.sceneData.SavePointPosition[0], _saveData.sceneData.SavePointPosition[1],0);            
            SceneManager.LoadScene(_saveData.sceneData.sceneID);


        }
        else
            print("could not find savefile");
    }

    

    // adds an object to list of saveObjects
    public void AddSaveObject(ISaveable obj)
    {
        _saveableObjects.Add(obj);
    }

    //removes object from list
    public void RemoveSaveObject(ISaveable obj)
    {
        _saveableObjects.Remove(obj);
    }

}


//dataContainers 

//main savedata class that will save the state of the game
[Serializable]
public class SaveData
{
    public Scenedata sceneData;
    public PlayerAbilityData playerAbilityData;
    public GameProgressData gameProgressData;
}

// holds data that is neccesery to know about a savefile, will be used to see description of your savegame before you load it
[Serializable]
public class SavefileInfoData
{
    public string playername;
    public int hours;
    public int minutes;
}

[Serializable]
public class Scenedata
{
    public string sceneID;
    public float[] SavePointPosition;

    public Scenedata(string id, float[] pos)
    {
        sceneID = id;
        SavePointPosition = new float[3] { pos[0], pos[1], pos[2] };
    }
}

[Serializable]
public class PlayerAbilityData
{
    public bool airJumpsUnlocked;
    public bool wallJumpUnlocked;
    public bool dashUnlocked;

    public PlayerAbilityData(bool dubbleJump,bool wallJump, bool dash)
    {
        airJumpsUnlocked = dubbleJump;
        wallJumpUnlocked = wallJump;
        dashUnlocked = dash;
    }

}

[Serializable]
public class GameProgressData
{
    public bool[] _powerUpPickedUp;

    public GameProgressData(bool[] powerUpPickedUp)
    {
        _powerUpPickedUp = new bool[(int)PowerUp.POWERUPTYPE.SIZE] { powerUpPickedUp[0], powerUpPickedUp[1], powerUpPickedUp[2] };
    }

}

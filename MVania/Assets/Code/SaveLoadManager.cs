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

    //list of all objects that will save thier data on save
    List<ISaveable> _saveableObjects = new List<ISaveable>();

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
            
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(Application.persistentDataPath + "/" + _saveFile ,FileMode.Create);
        
        bf.Serialize(fs, _saveData);
        fs.Close();


    }   

    public void LoadGame(string file)
    {
        if (File.Exists(Application.persistentDataPath + "/" + file))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(Application.persistentDataPath + "/" + file, FileMode.Open);

            _saveData = (SaveData)bf.Deserialize(fs);

            fs.Close();

            _saveFile = file;

            // create player and load the scene where we last saved
            GameObject player = Instantiate(_player);
            player.GetComponent<PlayerMovement>().transform.position = new Vector3( _saveData.sceneData.SavePointPosition[0], _saveData.sceneData.SavePointPosition[1],0);
            SceneManager.LoadScene(_saveData.sceneData.sceneID);


        }
        else
            print("could not find savefile");
    }

    public void DeleteGame(string path)
    {
        // try to find a specific file and delete it
        if(File.Exists(Application.persistentDataPath + "/" + path))
            File.Delete(Application.persistentDataPath + "/" + path);
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

[Serializable]
public class SaveData
{
    public Scenedata sceneData;
    public PlayerAbilityData playerAbilityData;
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
    public bool dubbleJumpUnlocked;
    public bool wallJumpUnlocked;
    public bool dashUnlocked;

    public PlayerAbilityData(bool dubbleJump,bool wallJump, bool dash)
    {
        dubbleJumpUnlocked = dubbleJump;
        wallJumpUnlocked = wallJump;
        dashUnlocked = dash;
    }

}

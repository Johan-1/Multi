using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour, ISaveable
{

    static GameProgressManager _gameProgressManager;
    public static GameProgressManager GetInstance { get { return _gameProgressManager;  } }

    // will hold witch powerup pickups that have been picked up
    bool[] _powerUpPickedUp = new bool[(int)PowerUp.POWERUPTYPE.SIZE];
    bool[] _healthUpgradesPickedUp = new bool[10];
 

    void Awake()
    {

        if (_gameProgressManager == null)
        {
            _gameProgressManager = this;
            DontDestroyOnLoad(this);
        }
        else if (_gameProgressManager != this)
            DestroyImmediate(gameObject);               
    }

    void Start()
    {
        AddToSaveableObjects();
    }

    //powerups
    public void SetPowerUpPickupStatus(int type, bool status)
    {
        _powerUpPickedUp[type] = status;
    }
    public bool GetPowerUpPickupStatus(int type)
    {
        return _powerUpPickedUp[type];
    }

    //healthupgrades
    public void SetHealthPickupStatus(int id, bool status)
    {
        _healthUpgradesPickedUp[id] = status;
    }

    public bool GetHealthPickupStatus(int id)
    {
        return _healthUpgradesPickedUp[id]; 
    }


    public void AddToSaveableObjects()
    {
        SaveLoadManager.GetInstance.AddSaveObject(this);

    }

    public void SaveData()
    {
        SaveLoadManager.GetInstance.saveData.gameProgressData = new GameProgressData(_powerUpPickedUp, _healthUpgradesPickedUp);
    }


    public void SetupSaveData()
    {
        GameProgressData data = SaveLoadManager.GetInstance.saveData.gameProgressData;
        _powerUpPickedUp = data.powerUpPickedUp;
        _healthUpgradesPickedUp = data.healthUpgradePickedUp;


        //debug
        string[] names = new string[] { "doublejumpItem-", "dashitem-", "walljumpItem-"};
        string all = "PICKEDUP ITEMS : ";

        for (int i = 0; i < _powerUpPickedUp.Length; i++)
            all += names[i] + _powerUpPickedUp[i] + ", ";

            print(all);
        
    }

    public void ClearData()
    {
        //clear all data so we go in clean in new game        
        _powerUpPickedUp = new bool[(int)PowerUp.POWERUPTYPE.SIZE];
        _healthUpgradesPickedUp = new bool[10];

    }

}

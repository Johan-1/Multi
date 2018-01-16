using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour, ISaveable
{

    static GameProgressManager _gameProgressManager;
    public static GameProgressManager GetInstance { get { return _gameProgressManager;  } }

    // will hold witch powerup pickups that have been picked up
    bool[] _powerUpPickedUp = new bool[(int)PowerUp.POWERUPTYPE.SIZE];


    void Awake()
    {

        if (_gameProgressManager == null)
        {
            _gameProgressManager = this;
            DontDestroyOnLoad(this);
        }
        else if (_gameProgressManager != this)
            DestroyImmediate(this);               
    }

    void Start()
    {
        AddToSaveableObjects();
    }


    public void SetPowerUpPickupStatus(int type, bool status)
    {
        _powerUpPickedUp[type] = status;
    }

    public bool GetPowerUpPickupStatus(int type)
    {
        return _powerUpPickedUp[type];
    }


    public void AddToSaveableObjects()
    {
        SaveLoadManager.GetInstance.AddSaveObject(this);

    }

    public void SaveData()
    {
        SaveLoadManager.GetInstance.saveData.gameProgressData = new GameProgressData(_powerUpPickedUp);
    }


    public void SetupSaveData()
    {
        GameProgressData data = SaveLoadManager.GetInstance.saveData.gameProgressData;
        _powerUpPickedUp = data._powerUpPickedUp;
        
    }

    public void ClearData()
    {
        //clear all data so we go in clean in new game
        _powerUpPickedUp = new bool[(int)PowerUp.POWERUPTYPE.SIZE];

    }

}

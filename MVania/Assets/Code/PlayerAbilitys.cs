using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class PlayerAbilitys : MonoBehaviour, ISaveable
{
    
    bool[] _abilitys = new bool[(int)PowerUp.POWERUPTYPE.SIZE];
   
    //temp storage in this class for now
    string _playerName;
    public string playerName { set { _playerName = value; } }
  
    void Start()
    {
        AddToSaveableObjects();

        if (SaveLoadManager.GetInstance.saveData.playerAbilityData != null)
            LoadData();
    }

    void LoadData()
    {
       
        PlayerAbilityData data = SaveLoadManager.GetInstance.saveData.playerAbilityData;
       
        _abilitys = data.abilitys;

        _playerName = SaveLoadManager.GetInstance.savefileInfoData.playername;
        
        print("loading abilitydata");
        print("dubblejump " + _abilitys[(int)PowerUp.POWERUPTYPE.AIRJUMP]);
        print("walljump " + _abilitys[(int)PowerUp.POWERUPTYPE.WALLJUMP]);
        print("dash " + _abilitys[(int)PowerUp.POWERUPTYPE.DASH]);

    }

    public void AddToSaveableObjects()
    {
        SaveLoadManager.GetInstance.AddSaveObject(this);
    }

    public void SaveData()
    {
        print("saving abilitys");
        SaveLoadManager.GetInstance.saveData.playerAbilityData = new PlayerAbilityData(_abilitys);

        SaveLoadManager.GetInstance.savefileInfoData.playername = _playerName;
       
    }


    void Update()
    {
        // debug unlock abilitys
        if (Input.GetKeyDown(KeyCode.Alpha1))
            _abilitys[0] = true;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _abilitys[1] = true;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            _abilitys[2] = true;


    }

    public void UnlockAbility(bool status, PowerUp.POWERUPTYPE power)
    {
        _abilitys[(int)power] = status;
    }

    public bool AbilityUnlocked(PowerUp.POWERUPTYPE power)
    {
        return _abilitys[(int)power];
    }

}

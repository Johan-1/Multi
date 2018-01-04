using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class PlayerAbilitys : MonoBehaviour, ISaveable
{

    bool _airJumpsUnlocked = false; public bool airJumpsUnlocked { get { return _airJumpsUnlocked; } set { _airJumpsUnlocked = value; } }
    bool _wallJumpUnlocked = false; public bool wallJumpUnlocked { get { return _wallJumpUnlocked; } set { _wallJumpUnlocked = value; } }
    bool _dashUnlocked = false; public bool dashUnlocked { get { return _dashUnlocked; } set { _dashUnlocked = value; } }

   

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
        _airJumpsUnlocked = data.airJumpsUnlocked;
        _wallJumpUnlocked = data.wallJumpUnlocked;
        _dashUnlocked = data.dashUnlocked;

        _playerName = SaveLoadManager.GetInstance.savefileInfoData.playername;
        
        print("loading abilitydata");
        print("dubblejump " + _airJumpsUnlocked);
        print("walljump " + _wallJumpUnlocked);
        print("dash " + _dashUnlocked);

    }

    public void AddToSaveableObjects()
    {
        SaveLoadManager.GetInstance.AddSaveObject(this);
    }

    public void SaveData()
    {
        print("saving abilitys");
        SaveLoadManager.GetInstance.saveData.playerAbilityData = new PlayerAbilityData(_airJumpsUnlocked, _wallJumpUnlocked, _dashUnlocked);

        SaveLoadManager.GetInstance.savefileInfoData.playername = _playerName;
       
    }


    void Update()
    {
        // debug unlock abilitys
        if (Input.GetKeyDown(KeyCode.Alpha1))
            _airJumpsUnlocked = true;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _wallJumpUnlocked = true;


    }

}

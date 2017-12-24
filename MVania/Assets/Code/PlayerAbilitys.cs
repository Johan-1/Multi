using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class PlayerAbilitys : MonoBehaviour, ISaveable
{

    bool _dubbleJumpUnlocked = false; public bool dubbleJumpUnlocked { get { return _dubbleJumpUnlocked; } set { _dubbleJumpUnlocked = value; } }
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
        _dubbleJumpUnlocked = data.dubbleJumpUnlocked;
        _wallJumpUnlocked = data.wallJumpUnlocked;
        _dashUnlocked = data.dashUnlocked;

        print("loading abilitydata");
        print("dubblejump " + _dubbleJumpUnlocked);
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
        SaveLoadManager.GetInstance.saveData.playerAbilityData = new PlayerAbilityData(_dubbleJumpUnlocked, _wallJumpUnlocked, _dashUnlocked);

        SaveLoadManager.GetInstance.savefileInfoData.playername = _playerName;
    }

    void Update()
    {

        DebugAbilitys();
    }

    void DebugAbilitys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _dubbleJumpUnlocked = true;
            print("dubbleJumpUnlocked");
            
        }

    }



}

using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class PlayerAbilitys : MonoBehaviour, ISaveable
{
    
    bool[] _abilitys = new bool[(int)PowerUp.POWERUPTYPE.SIZE];
   
    
  
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
                
        //debug
        print("UNLOCKED ABILITYS :" + "dubblejump-" + _abilitys[0] + ", dash-" + _abilitys[1] + ", walljump-" + _abilitys[2]);
       
    }

    public void AddToSaveableObjects()
    {
        SaveLoadManager.GetInstance.AddSaveObject(this);
    }

    public void SaveData()
    {        
        SaveLoadManager.GetInstance.saveData.playerAbilityData = new PlayerAbilityData(_abilitys);               
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable, ISaveable
{

    [SerializeField] int _maxHealth = 5;
    [SerializeField] int _extraHealthPerUpgrade = 1;

    int _numUpgrades;
    int _currentHealth;
       

    PlayerRespawn _playerRespawn;

    void Awake()
    {
        // add so number of upgrades will be saved and load data
        AddToSaveableObjects();
        LoadData();

        // set max health to include number of uppgrades found
        _maxHealth += (_numUpgrades * _extraHealthPerUpgrade);
        _currentHealth = _maxHealth;

        // setup healthbar depending on uppgrades
        UIManager.GetInstance.healthBar.SetupHealthBar(_numUpgrades);

        _playerRespawn = GetComponent<PlayerRespawn>();
       
    }

        
    public void ModifyHealth(int health, bool instantRespawn = false)
    {
        // change health
        _currentHealth += health;

        // uppdate healthbar
        UIManager.GetInstance.healthBar.ModifyHealthbar(_currentHealth, _maxHealth);

        // if health is 0 player is dead, else check if instantrespawn is true and respawn player to last respawn point (spikes etc can do 1 damage but will return player to begining of room)
        if (_currentHealth <= 0)       
            Die();                   
        else if (instantRespawn)
            _playerRespawn.RespawnPlayer();
                   
    }
   

    public void Die()
    {
        // Reload last savefile
        _playerRespawn.ReloadLastSave();
        
    }

    public void UpgradeMaxHealth()
    {
        // add one to found uppgrades
        // add on to max health and reset currenthealth
        _numUpgrades++;
        _maxHealth += _extraHealthPerUpgrade;
        _currentHealth = _maxHealth;

        // upgrade healthbar(will get wider on moore maxhealth)
        UIManager.GetInstance.healthBar.UpgradeHealthBar(_numUpgrades);
    }

    public void AddToSaveableObjects()
    {
        SaveLoadManager.GetInstance.AddSaveObject(this);
    }

    void LoadData()
    {
        // check if we have any saved data
        PlayerHealthData data = SaveLoadManager.GetInstance.saveData.healthData;
        if (data != null)
        {
            _numUpgrades = data.numHealthUpgrades;
            print("NUM HEALTH UPPGRADES : " + _numUpgrades);
        }
    }

    public void SaveData()
    {
        // save both to gamefile and to savefileinfo file(num uppgrades found will be shown in ui for each game in loadgame screen)
        SaveLoadManager.GetInstance.saveData.healthData = new PlayerHealthData(_numUpgrades);
        SaveLoadManager.GetInstance.savefileInfoData.numHealthUpgrades = _numUpgrades;

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            ModifyHealth(-1);

        if (Input.GetKeyDown(KeyCode.M))
            ModifyHealth(1);

        // debug stuff just here for the moment
        // go back to main menu
        if (Input.GetKeyDown(KeyCode.H))
        {
            SaveLoadManager.GetInstance.ClearData();
            GameProgressManager.GetInstance.ClearData();
            UIManager.GetInstance.DisableUI();
            FindObjectOfType<CameraMovement>().enabled = false;
            Destroy(FindObjectOfType<PlayerAbilitys>().gameObject);

            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            

        }

        if (Input.GetKeyDown(KeyCode.U))
            UpgradeMaxHealth();
           
    }

}

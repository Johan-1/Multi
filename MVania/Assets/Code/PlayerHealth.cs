using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{

    [SerializeField] int _maxHealth = 5;
  
    int _currentHealth;
       

    PlayerRespawn _playerRespawn;

    void Awake()
    {
        _currentHealth = _maxHealth;
        _playerRespawn = GetComponent<PlayerRespawn>();
        UIManager.GetInstance.healthBar.SetupHealthBar(_maxHealth);
    }

    
    // modify health
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
           
    }

}

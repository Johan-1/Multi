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
    }

    void Start()
    {
        UIManager.GetInstance.healthBar.ModifyHealthbar(_currentHealth, _maxHealth);
    }

    public void ModifyHealth(int health)
    {
        _currentHealth += health;

        UIManager.GetInstance.healthBar.ModifyHealthbar(_currentHealth, _maxHealth);
        
        if (_currentHealth <= 0 )
            Die();
    }

    public void RespawnOnDamage(int health)
    {
        _currentHealth += health;

        UIManager.GetInstance.healthBar.ModifyHealthbar(_currentHealth, _maxHealth);

        if (_currentHealth > 0)
            _playerRespawn.RespawnPlayer();
        else
            _playerRespawn.ReloadLastSave();
       
    }

    public void Die()
    {
        // start respawn
        _playerRespawn.ReloadLastSave();
        
    }      
   

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            ModifyHealth(-1);

        if (Input.GetKeyDown(KeyCode.M))
            ModifyHealth(1);

        if (Input.GetKeyDown(KeyCode.H))
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable, ISaveable
{

    [SerializeField] int _maxHealth = 5;
    [SerializeField] int _extraHealthPerUpgrade = 1;

    [Space(5), Header("KNOCKBACK AND INVINSIBILITY"), Space(5)]
    [SerializeField] Vector2 _knockbackForce = new Vector2(0.5f, 2.0f);
    [SerializeField] float _knockbackTime = 0.2f;
    [SerializeField] float _invinsibilityTime = 0.5f;
    [SerializeField] float _flashTime = 0.1f;
    [SerializeField] Color _flashColor;

    // privates
    int _numUpgrades;
    int _currentHealth;
    Color _originalColor;
    bool _isinvinisible; public bool isInvinsible { get { return _isinvinisible; } set { _isinvinisible = value; } }

    // references
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

        // get the color of the sprite
        _originalColor = GetComponent<SpriteRenderer>().color;

        _playerRespawn = GetComponent<PlayerRespawn>();
       
    }

        
    public void ModifyHealth(int health, bool instantRespawn = false)
    {
        // only do damage if we are not invincible (can still respawn on instantRespawn damaging objects, just without taking damage)
        if (!_isinvinisible)
        {
            _currentHealth += health;

            // uppdate healthbar
            UIManager.GetInstance.healthBar.ModifyHealthbar(_currentHealth, _maxHealth);
        }
                   
        // if health is 0 player is dead, else check if instantrespawn is true and respawn player to last respawn point (spikes etc can do 1 damage but will return player to begining of room)
        if (_currentHealth <= 0)
            Die();
        else if (instantRespawn)
        {
            // dont want to be abble to take damage while respawning(will be set back when respawning is done)
            _isinvinisible = true;
            _playerRespawn.RespawnPlayer();
        }            
        else if(!_isinvinisible) // if we are not dead and instant respawn is false, do a knockback and start invinsible routines
        {
            Knockback();
            StartCoroutine(Invinsible());
            StartCoroutine(InvinisbleEffect());
        }                            
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

    public void Knockback()
    {
        // knockback is handled in playermovement
        GetComponent<PlayerMovement>().HandleKnockback(_knockbackForce, _knockbackTime);
    }

    IEnumerator Invinsible()
    {
        // set invinisble to true and set it back after invinsibletime
        _isinvinisible = true;
        float timer = 0.0f;
        while (timer < _invinsibilityTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _isinvinisible = false;
    }

    IEnumerator InvinisbleEffect()
    {        
        while (_isinvinisible)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            // switch between original spritecolor and flashcolor during invincibletime
            sr.color = _flashColor;
            yield return new WaitForSeconds(_flashTime);
            sr.color = _originalColor;
            yield return new WaitForSeconds(_flashTime);

        }
        
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
        
        if (Input.GetKeyDown(KeyCode.U))
            UpgradeMaxHealth();
           
    }

}

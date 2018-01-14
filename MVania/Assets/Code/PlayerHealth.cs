using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{

    [SerializeField] int _maxHealth = 5;

    int _currentHealth;
    bool _isRespawning;

    public event Action<int, int> OnHealthChange;


    void Awake()
    {
        _currentHealth = _maxHealth;    
    }


    public void ModifyHealth(int health)
    {
        _currentHealth += health;

        // call subscribers
        if (OnHealthChange != null)
            OnHealthChange(_currentHealth,_maxHealth);

        // kill if health is at 0 and we are not alredy in respan
        if (_currentHealth <= 0 && !_isRespawning)
            Die();
    }

    public void Die()
    {       
        // start respawn
        StartCoroutine(Respawn());
    }


    IEnumerator Respawn()
    {
        //disable player
        _isRespawning = true;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<SpriteRenderer>().enabled = false;

        // wait before we reload the game
        yield return new WaitForSeconds(2.0f);

        // check if we have any savedata
        if (SaveLoadManager.GetInstance.saveData.sceneData != null)
        {
            // if we have saved data, reset the state of the game to when we last saved
            SaveLoadManager.GetInstance.LoadGame("Respawn");
            Destroy(gameObject);
        }
        else
        {
            // if not set position to first room and load it
            // TODO : should have save data by just starting a new game with this info
            transform.position = new Vector3(0, 4, 0);
            SceneManager.LoadScene("Screen1");
            _currentHealth = _maxHealth;

            yield return new WaitForSeconds(1.0f);

            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<SpriteRenderer>().enabled = true;
            _isRespawning = false;

        }

        

        

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            ModifyHealth(-1);

        if (Input.GetKeyDown(KeyCode.M))
            ModifyHealth(1);
    }

}

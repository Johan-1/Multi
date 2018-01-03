using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUp : MonoBehaviour
{    
    enum POWERUPTYPE
    {
        AIRJUMP,
        DASH,
        WALLJUMP
    }

    [SerializeField] POWERUPTYPE _powerUp;

    bool _pickedUp = false;

    void Awake()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerAbilitys>().airJumpsUnlocked = true;            
            Destroy(gameObject);
        }
    }

    
}

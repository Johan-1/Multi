using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUp : MonoBehaviour
{    
    public enum POWERUPTYPE
    {
        AIRJUMP,
        DASH,
        WALLJUMP,
        SIZE
    }

    [SerializeField] POWERUPTYPE _powerUp;
    [SerializeField] GameObject _pickupTextImage;

    bool _pickedUp = false;

    void Awake()
    {
        CheckIfPickedUp();
    }

    void CheckIfPickedUp()
    {
        // if alredy been picked up, remove from scene
        if (GameProgressManager.GetInstance.GetPowerUpPickupStatus((int)_powerUp))
            DestroyImmediate(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // unlock ability so we can use it
            other.GetComponent<PlayerAbilitys>().airJumpsUnlocked = true; 

            // set that the pickup have been picked up otherwise it will be back when we reenter the room
            GameProgressManager.GetInstance.SetPowerUpPickupStatus((int)_powerUp, true);

            // spawn text,delete pickup and set text to be deleted
            _pickupTextImage = Instantiate(_pickupTextImage, transform.position + new Vector3(0, 3, 0), Quaternion.identity);

            Destroy(_pickupTextImage, 3.0f);
            Destroy(gameObject);
        }
    }

    
}

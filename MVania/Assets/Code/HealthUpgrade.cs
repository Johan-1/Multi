using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : MonoBehaviour
{
    [Range(0,9)]
    [SerializeField] int _id;
    [SerializeField] GameObject _pickupTextImage;

    void Awake()
    {
        CheckIfPickedUp();
    }

    void CheckIfPickedUp()
    {
        // if alredy been picked up, remove from scene
        if (GameProgressManager.GetInstance.GetHealthPickupStatus(_id))
            DestroyImmediate(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // unlock ability so we can use it
            other.GetComponent<PlayerHealth>().UpgradeMaxHealth();

            // set that the pickup have been picked up otherwise it will be back when we reenter the room
            GameProgressManager.GetInstance.SetHealthPickupStatus(_id, true);

            // spawn text,delete pickup and set text to be deleted
            _pickupTextImage = Instantiate(_pickupTextImage, transform.position + new Vector3(0, 3, 0), Quaternion.identity);

            Destroy(_pickupTextImage, 3.0f);
            Destroy(gameObject);
        }
    }

}

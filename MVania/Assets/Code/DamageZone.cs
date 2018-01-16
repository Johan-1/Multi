using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{

    [SerializeField] int _damage = 1;
    [SerializeField] bool _instantRespawn = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IDamageable>() != null)
        {
            // if player, respawn is set to _instantrespawn, else always false, enemys dont have respawn           
            other.GetComponent<IDamageable>().ModifyHealth(-_damage, other.tag == "Player" ? _instantRespawn : false);
            print("tookdamage");
        }                   
    }

}

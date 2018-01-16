using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{

    [SerializeField] int _damage = 1;
    [SerializeField] bool _instantRespawn = false;

    void OnTriggerEnter2D(Collider2D other)
    {

        if (_instantRespawn)
            if (other.tag == "Player")
            {
                other.GetComponent<PlayerHealth>().RespawnOnDamage(-_damage);
                return;
            }

        if (other.GetComponent<IDamageable>() != null)
            other.GetComponent<IDamageable>().ModifyHealth(-_damage);


        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{

    [SerializeField] int _damage = 1;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<IDamageable>() != null)
            other.GetComponent<IDamageable>().ModifyHealth(-_damage);


        print("everyframe?");
    }
}

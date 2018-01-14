using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{

    void ModifyHealth(int health);
    void Die();
	
}

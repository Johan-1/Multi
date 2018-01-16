using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    
    [SerializeField] Image _barFill;

    public void ModifyHealthbar(int newHealth, int maxhealth)
    {
               
        _barFill.fillAmount = (float)newHealth / maxhealth;
        
    }

    public void SetupHealthBar(int maxHealth)
    {
        gameObject.SetActive(true);
        _barFill.fillAmount = 1.0f;

    }
    

    
}

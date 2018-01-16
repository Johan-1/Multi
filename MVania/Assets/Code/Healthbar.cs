using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    
    Image _barFill;


    void Awake()
    {              
        _barFill = GetComponent<Image>();
    }

    public void ModifyHealthbar(int newHealth, int maxhealth)
    {
               
        _barFill.fillAmount = (float)newHealth / maxhealth;
        
    }
}

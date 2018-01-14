using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    [SerializeField] PlayerHealth _playerhealth;
    Image _barFill;


    void Awake()
    {
        //subscribe to when player takes damage
        _playerhealth.OnHealthChange += ModifyHealthbar;
        _barFill = GetComponent<Image>();
    }

    void ModifyHealthbar(int newHealth, int maxhealth)
    {
               
        _barFill.fillAmount = (float)newHealth / maxhealth;
        
    }
}

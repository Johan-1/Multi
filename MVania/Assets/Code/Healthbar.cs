using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    
    [SerializeField] Image _barFill;
    Image _bar;

    [SerializeField] float _standardWidth = 200.0f;
    [SerializeField] float _extraWidthAllUppgrades = 400.0f;

    int _maxUpgrades = 10; // temp storage and number

    void Awake()
    {
        _bar = GetComponent<Image>();    
    }

    public void ModifyHealthbar(int newHealth, int maxhealth)
    {              
        _barFill.fillAmount = (float)newHealth / maxhealth;     
        
    }

    public void UpgradeHealthBar(int numUpgrades)
    {                
        // no need to wory about devison by zero here, only called when a upgrade been picked up
        float width = _standardWidth + (_extraWidthAllUppgrades * ((float)numUpgrades / _maxUpgrades));
        
        _barFill.rectTransform.sizeDelta = new Vector2(width, 20.0f);
        _bar.rectTransform.sizeDelta = new Vector2(width, 20.0f);
        _barFill.fillAmount = 1.0f;
    }

    public void SetupHealthBar(int numUpgrades)
    {
        gameObject.SetActive(true);       

        // avoid devision by zero
        float width;
        if (numUpgrades != 0)
            width = _standardWidth + (_extraWidthAllUppgrades * ((float)numUpgrades / _maxUpgrades));
        else
            width = _standardWidth;

        _barFill.rectTransform.sizeDelta = new Vector2(width, 20.0f);
        _bar.rectTransform.sizeDelta = new Vector2(width, 20.0f);
        _barFill.fillAmount = 1.0f;
    }
    

    
}

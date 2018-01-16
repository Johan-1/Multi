using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static UIManager _UiManager;
    public static UIManager GetInstance { get { return _UiManager; } }


    [SerializeField] Healthbar _healthBar;
    public Healthbar healthBar { get { return _healthBar; } }

    [SerializeField] ScreenFade _screenFade;
    public ScreenFade screenfade { get { return _screenFade; } }

    void Awake()
    {
        if (_UiManager == null)
        {
            _UiManager = this;
            DontDestroyOnLoad(this);
        }
        else if (_UiManager != this)
            DestroyImmediate(this);
    }



    public void DisableUI()
    {

        _healthBar.gameObject.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PausMenu : MonoBehaviour
{

    [SerializeField] GameObject _objects; // parent object of evrything in pausmenu

    [SerializeField] GameObject _firstButton;
    [SerializeField] Vector2 _highlightScale;
    [SerializeField] Vector2 _standardScale = new Vector2(1, 1);

    bool _canOpenMenu; public bool canOpenMenu { get { return _canOpenMenu; } set { _canOpenMenu = value; }  }

    bool _active;

    GameObject _lastSelectedButton;

    void Update()
    {
        if (_canOpenMenu && Input.GetButtonDown("Pause"))
        {
            _active = !_active; // toggle menu

            if (_active)
                EnableMenu();
            else
            {
                DisableMenu();
                StartCoroutine(ResumeCo());
            }
                
        }

        // check if the selected button has changed
        if (_active && _lastSelectedButton != EventSystem.current.currentSelectedGameObject)
        {
            
            // if mouse has been clicked refocus on last selected button
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
                return;
            }

            // reset scale on last button
            // set scale on the new button
            // set the new button as last selected button
            _lastSelectedButton.transform.localScale = _standardScale;
            EventSystem.current.currentSelectedGameObject.transform.localScale = _highlightScale;
            _lastSelectedButton = EventSystem.current.currentSelectedGameObject;           
        }
        
    }   

    void EnableMenu()
    {
        // enable pausScreen and disable playerinput
        _objects.SetActive(true);
        FindObjectOfType<PlayerMovement>().DisableInput();
        Time.timeScale = 0.0f;

        // set the higlighted button to firstbutton, change scale and set last selected button to current button
        EventSystem.current.SetSelectedGameObject(_firstButton);
        _firstButton.transform.localScale = _highlightScale;
        _lastSelectedButton = _firstButton;
    }

    void DisableMenu()
    {
        // disable pausscreen and resume speed
        EventSystem.current.currentSelectedGameObject.transform.localScale = _standardScale;
        EventSystem.current.SetSelectedGameObject(null);
        _lastSelectedButton = null;
        _objects.SetActive(false);
        Time.timeScale = 1.0f;

        _active = false;
    }

    IEnumerator ResumeCo()
    {
        yield return null; // skip one frame before enable input of player (dont want player getting input on pressing Resumebutton)
        FindObjectOfType<PlayerMovement>().EnableInput();
                
    }

    public void ResumeGame()
    {
        DisableMenu();
        StartCoroutine(ResumeCo());
    }

    public void QuitGame()
    {
        _canOpenMenu = false;
        DisableMenu();

        // clear all data and disable game UI
        SaveLoadManager.GetInstance.ClearData();
        GameProgressManager.GetInstance.ClearData();
        UIManager.GetInstance.DisableUI();

        // disable cameramovement and remove player
        FindObjectOfType<CameraMovement>().enabled = false;
        Destroy(FindObjectOfType<PlayerAbilitys>().gameObject);

        // load mainMenu
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");

    }

}

using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class TimeManager : MonoBehaviour , ISaveable
{

    float _seconds;
    int _minutes;
    int _hours;

    void Start () 			
	{
        AddToSaveableObjects();
        LoadData();
	}
	
	void Update () 	
	{
        _seconds += Time.deltaTime;
        if (_seconds >= 60)
        {
            _minutes++;
            _seconds = 0.0f;
            if (_minutes == 60)
            {
                _hours++;
                _minutes = 0;
            }
        }

    }

    void LoadData()
    {
        SavefileInfoData fileData = SaveLoadManager.GetInstance.savefileInfoData;
        _minutes = fileData.minutes;
        _hours = fileData.hours;
    }

    public void AddToSaveableObjects()
    {
        SaveLoadManager.GetInstance.AddSaveObject(this);
    }

    public void SaveData()
    {      
        SaveLoadManager.GetInstance.savefileInfoData.hours = _hours;
        SaveLoadManager.GetInstance.savefileInfoData.minutes = _minutes;
    }

}

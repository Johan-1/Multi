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
        _seconds += Time.deltaTime * 1000.0f;
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
        TimeData fileData = SaveLoadManager.GetInstance.saveData.timeData;
        if (fileData == null)
            return;

        _hours = fileData.time[0];
        _minutes = fileData.time[1];

        print("Time Played : " + _hours.ToString("00") + " " + _minutes.ToString("00"));
             
    }

    public void AddToSaveableObjects()
    {
        SaveLoadManager.GetInstance.AddSaveObject(this);
    }

    public void SaveData()
    {      
        SaveLoadManager.GetInstance.savefileInfoData.hours = _hours;
        SaveLoadManager.GetInstance.savefileInfoData.minutes = _minutes;

        SaveLoadManager.GetInstance.saveData.timeData = new TimeData(new int[] { _hours, _minutes });
    }

}

using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class SavePoint : MonoBehaviour 
{

    [SerializeField] string _sceneID;
    [SerializeField] Transform _spawnPosition;
    bool _saved = false;


    void OnTriggerStay2D(Collider2D other)
    {
        if(!_saved)
            if (other.tag == "Player" && Input.GetAxisRaw("Vertical") > 0.8f)
            {
                SaveLoadManager.GetInstance.saveData.sceneData = new Scenedata(_sceneID, new float[3] { _spawnPosition.position.x, _spawnPosition.position.y, 0 });
                SaveLoadManager.GetInstance.SaveGame();
               
                _saved = true;
            }
    }

    

    

}

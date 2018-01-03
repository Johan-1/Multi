using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class SavePoint : MonoBehaviour 
{

    [SerializeField] string _sceneID;
    [SerializeField] Transform _spawnPosition;

    [SerializeField] GameObject _saveText;
    bool _saved = false;
     
    void OnTriggerStay2D(Collider2D other)
    {

        // if player and not alredy saved(delay between able to save of 2 sec)
        if (other.tag == "Player" && !_saved)
        {
            // show "press up to save" image
            _saveText.SetActive(true);
            
            // if up is pressed remove press to save image
            // set saved to true and start coroutine that set it back to false in 2 sec
            // set witch scene we saved in and the pos of savepoint
            if (Input.GetAxisRaw("Vertical") > 0.8f)
            {
                _saveText.SetActive(false);
                _saved = true;

                SaveLoadManager.GetInstance.saveData.sceneData = new Scenedata(_sceneID, new float[3] { _spawnPosition.position.x, _spawnPosition.position.y, 0 });
                SaveLoadManager.GetInstance.SaveGame(); // will loop over all objects that have data to save

                StartCoroutine(Wait());
               
            }

        }
            
    }

    void OnTriggerExit2D(Collider2D other)
    {
        _saveText.SetActive(false);
    }


    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2.0f);
        _saved = false;
    }


}

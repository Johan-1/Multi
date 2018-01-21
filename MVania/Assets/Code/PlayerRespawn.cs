using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    
    [SerializeField] float _waitBeforeRespawn = 1.0f;
    [SerializeField] float _fadingTime = 1.0f;
    [SerializeField] float _stayBlackTime = 0.5f;


    Vector3 _respawnPosition;
    public Vector3 respawnPosition { set { _respawnPosition = value; } }

    bool _isHandelingRespawn = false;

    public void RespawnPlayer()
    {
        if(!_isHandelingRespawn)
           StartCoroutine(RespawnCo());
    }

    public void ReloadLastSave()
    {
        if(!_isHandelingRespawn)
           StartCoroutine(ReloadLastSaveCo());
    }

    void DisablePlayer()
    {
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void EnablePlayer()
    {
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<SpriteRenderer>().enabled = true;

    }

    // respawn in room when taking damage on something that want you to start the room over(pits,spikes etc)
    IEnumerator RespawnCo()
    {        
        //disable player and set that we are handeling respawn
        _isHandelingRespawn = true;
        DisablePlayer();
        
        // fade out to black and wait untill done
        UIManager.GetInstance.screenfade.FadeOut(_fadingTime, _waitBeforeRespawn);
        yield return new WaitForSeconds(_fadingTime + _waitBeforeRespawn);

        // respawn player and enable renderer
        transform.position = _respawnPosition;
        GetComponent<SpriteRenderer>().enabled = true;
       
        // fade back in and wait untill done
        UIManager.GetInstance.screenfade.FadeIn(_fadingTime, _stayBlackTime);
        yield return new WaitForSeconds(_fadingTime + _stayBlackTime);

        // enable player and set that respawn is finished
        EnablePlayer();
        _isHandelingRespawn = false;

        // set that we now again can take damage
        GetComponent<PlayerHealth>().isInvinsible = false;
    }

    // when all health is lost, reload last save
    IEnumerator ReloadLastSaveCo()
    {
        _isHandelingRespawn = true;
        //disable player
        DisablePlayer();
        
        UIManager.GetInstance.screenfade.FadeOut(_fadingTime,_waitBeforeRespawn);

        yield return new WaitForSeconds(_fadingTime + _stayBlackTime + _waitBeforeRespawn);

        // check if we have any savedata
        if (SaveLoadManager.GetInstance.saveData.sceneData != null)
        {
            // if we have saved data, reset the state of the game to when we last saved
            SaveLoadManager.GetInstance.LoadGame("Respawn");
            UIManager.GetInstance.screenfade.FadeIn(1.0f, 1.0f);
            Destroy(gameObject);
        }
        else // if not something have gone horribly wrong
            print("Cant find savefile to reload last save");
        

    }

}

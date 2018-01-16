using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.SceneManagement;


public class RoomLoader : MonoBehaviour 
{


    [SerializeField] string _roomToLoad;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerRespawn>().respawnPosition = other.transform.position;
            SceneManager.LoadScene(_roomToLoad);
        }
            

    }



    void OnDrawGizmos()
    {

        BoxCollider2D bc = GetComponent<BoxCollider2D>();

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(bc.transform.position, new Vector3(bc.size.x, bc.size.y, 1));
        

    }


}

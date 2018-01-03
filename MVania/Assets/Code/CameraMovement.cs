using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class CameraMovement : MonoBehaviour 
{

    [SerializeField] Vector2 _XClampMinMax;
    [SerializeField] Vector2 _YClampMinMax;

    Transform _target;
    Vector3 _vel;

	void Start () 			
	{
        _target = FindObjectOfType<PlayerMovement>().transform;

        Vector3 desiredPosition = GetDesiredPosition();
        transform.position = new Vector3( desiredPosition.x, desiredPosition.y ,transform.position.z);

    }

    void LateUpdate()
    {

        Vector3 desiredPosition = GetDesiredPosition();
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(desiredPosition.x,desiredPosition.y,transform.position.z), ref _vel, 0.4f);
    }


    Vector3 GetDesiredPosition()
    {
        Vector3 desiredPosition = _target.transform.position + new Vector3(0, 0, 0);
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, _XClampMinMax.x, _XClampMinMax.y);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, _YClampMinMax.x, _YClampMinMax.y);

        return desiredPosition;

    }


}

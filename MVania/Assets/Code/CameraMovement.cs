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

    void OnDrawGizmos()
    {

        // draw the bounding box of camera clamps
        Camera camera = GetComponent<Camera>();
        
        float height = 2.0f * camera.orthographicSize;
        float width = height * camera.aspect;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(_XClampMinMax.y + (width * 0.5f), _YClampMinMax.y + (height * 0.5f), 0), new Vector3(_XClampMinMax.y + (width * 0.5f), _YClampMinMax.x - (height * 0.5f), 0));
        Gizmos.DrawLine(new Vector3(_XClampMinMax.x - (width * 0.5f), _YClampMinMax.y + (height * 0.5f), 0), new Vector3(_XClampMinMax.x - (width * 0.5f), _YClampMinMax.x - (height * 0.5f), 0));
        Gizmos.DrawLine(new Vector3(_XClampMinMax.x - (width * 0.5f), _YClampMinMax.y + (height * 0.5f), 0), new Vector3(_XClampMinMax.y + (width * 0.5f), _YClampMinMax.y + (height * 0.5f), 0));
        Gizmos.DrawLine(new Vector3(_XClampMinMax.x - (width * 0.5f), _YClampMinMax.x - (height * 0.5f), 0), new Vector3(_XClampMinMax.y + (width * 0.5f), _YClampMinMax.x - (height * 0.5f), 0));

         
    }

}

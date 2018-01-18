using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class CameraMovement : MonoBehaviour 
{

    [SerializeField] Vector2 _XClampMinMax;
    [SerializeField] Vector2 _YClampMinMax;

    [SerializeField] Vector2 _boundingboxXPosNeg;
    [SerializeField] Vector2 _boundingboxYPosNeg;

    // scriptable object that all cameras share
    // can overide the shared values and use the defined ones above instead
    [SerializeField] bool _overidePlayerBoundingbox = false;
    [SerializeField] PlayerBoundingboxIncamera _playerBoundingbox;

    Vector3 _desiredPosition;
    Transform _target;
    Vector3 _vel;

	void Start () 			
	{
        // if we dont want to overide bounding box set the values to the ones in our scriptable objects(will be the same for all cameras in all scenes)
        if (!_overidePlayerBoundingbox)
        {
            _boundingboxXPosNeg = _playerBoundingbox.boundingboxXPosNeg;
            _boundingboxYPosNeg = _playerBoundingbox.boundingboxYPosNeg;
        }
        
        // find target
        _target = FindObjectOfType<PlayerMovement>().transform;

        // set desiredpos to pos of player 
        _desiredPosition = new Vector3(_target.position.x, _target.position.y, transform.position.z);

        // clamp values and set the position
        ClampPosition();
        transform.position = _desiredPosition;

    }

    void LateUpdate()
    {

        GetDesiredPosition();
        ClampPosition();
        //transform.position = _desiredPosition;
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(_desiredPosition.x,_desiredPosition.y,transform.position.z), ref _vel, 0.1f);
    }

    void GetDesiredPosition()
    {
        
        // check if player is outside the boundingbox and move desiredpos accordingly
        // these values will be clamped later
        if (_target.transform.position.x > transform.position.x + (_boundingboxXPosNeg.x))
            _desiredPosition.x = _target.transform.position.x - (_boundingboxXPosNeg.x);
        else if (_target.transform.position.x < transform.position.x - (_boundingboxXPosNeg.y))
            _desiredPosition.x = _target.transform.position.x + (_boundingboxXPosNeg.y);

        if (_target.transform.position.y > transform.position.y + (_boundingboxYPosNeg.x))
            _desiredPosition.y = _target.transform.position.y - (_boundingboxYPosNeg.x);
        else if (_target.transform.position.y < transform.position.y - (_boundingboxYPosNeg.y))
            _desiredPosition.y = _target.transform.position.y + (_boundingboxYPosNeg.y);

    }

    void ClampPosition()
    {
        // adjust desiredpos to be insed clamps if neccesary
        _desiredPosition.x = Mathf.Clamp(_desiredPosition.x, _XClampMinMax.x, _XClampMinMax.y);
        _desiredPosition.y = Mathf.Clamp(_desiredPosition.y, _YClampMinMax.x, _YClampMinMax.y);
    }

    void OnDrawGizmos()
    {        
        Camera camera = GetComponent<Camera>();

        // draw the bounding box of camera clamps
        float height = 2.0f * camera.orthographicSize;
        float width = height * camera.aspect;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(_XClampMinMax.y + (width * 0.5f), _YClampMinMax.y + (height * 0.5f), 0), new Vector3(_XClampMinMax.y + (width * 0.5f), _YClampMinMax.x - (height * 0.5f), 0)); // top right to bottom right
        Gizmos.DrawLine(new Vector3(_XClampMinMax.x - (width * 0.5f), _YClampMinMax.y + (height * 0.5f), 0), new Vector3(_XClampMinMax.x - (width * 0.5f), _YClampMinMax.x - (height * 0.5f), 0)); // top left to bottom left
        Gizmos.DrawLine(new Vector3(_XClampMinMax.x - (width * 0.5f), _YClampMinMax.y + (height * 0.5f), 0), new Vector3(_XClampMinMax.y + (width * 0.5f), _YClampMinMax.y + (height * 0.5f), 0)); // top left to top right
        Gizmos.DrawLine(new Vector3(_XClampMinMax.x - (width * 0.5f), _YClampMinMax.x - (height * 0.5f), 0), new Vector3(_XClampMinMax.y + (width * 0.5f), _YClampMinMax.x - (height * 0.5f), 0)); // bottom left to bottom right

        // draw the boundingbox of player 
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(transform.position.x + _boundingboxXPosNeg.x, transform.position.y + _boundingboxYPosNeg.x, 0), new Vector3(transform.position.x + _boundingboxXPosNeg.x, transform.position.y - _boundingboxYPosNeg.y, 0)); // top right to bottom right
        Gizmos.DrawLine(new Vector3(transform.position.x - _boundingboxXPosNeg.y, transform.position.y + _boundingboxYPosNeg.x, 0), new Vector3(transform.position.x - _boundingboxXPosNeg.y, transform.position.y - _boundingboxYPosNeg.y, 0)); // top left to bottom left
        Gizmos.DrawLine(new Vector3(transform.position.x - _boundingboxXPosNeg.y, transform.position.y + _boundingboxYPosNeg.x, 0), new Vector3(transform.position.x + _boundingboxXPosNeg.x, transform.position.y + _boundingboxYPosNeg.x, 0)); // top left to top right
        Gizmos.DrawLine(new Vector3(transform.position.x - _boundingboxXPosNeg.y, transform.position.y - _boundingboxYPosNeg.y, 0), new Vector3(transform.position.x + _boundingboxXPosNeg.x, transform.position.y - _boundingboxYPosNeg.y, 0)); // bottom left to bottom right

    }

    // will write the playerboundingbox values on this object to our scriptableobject that all cameras share(called from custom editor)
    public void SetPlayerBoundingboxAllCameras()
    {
        _playerBoundingbox.boundingboxXPosNeg = _boundingboxXPosNeg;
        _playerBoundingbox.boundingboxYPosNeg = _boundingboxYPosNeg;
    }

    // will set the playerboundingbox values of this camera to the ones in the shared scriptable object(this happens automaticly in playmode, this is only to see it in editormode)
    public void GetPlayerBoundingbox()
    {
        _boundingboxXPosNeg = _playerBoundingbox.boundingboxXPosNeg;
        _boundingboxYPosNeg = _playerBoundingbox.boundingboxYPosNeg;
    }
    

}

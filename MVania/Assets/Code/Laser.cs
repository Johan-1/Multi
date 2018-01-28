using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{

    [Space(5), Header("BASIC SETTINGS"), Space(5)]
    [SerializeField] Transform _laserStartPosition; public Transform raycastPoint { get{ return _laserStartPosition; }}
    [SerializeField] Vector3 _startDirection;
    [SerializeField] bool _localSpace;
    [SerializeField] int _damage;
    
    [Space(5), Header("ACTIVE TOOGLE"), Space(5)]
    [SerializeField] bool _constant;
    [SerializeField] bool _startActive;
    // have array of toogletimes to be able to create moore diverse patterns
    [SerializeField] float[] toggleActiveTimes;

    [Space(5),Header("DYNAMIC WIDTH"), Space(5)]
    [SerializeField] bool _dynamicWidthOnToggle;
    [SerializeField] Vector2 _widthMinMax; public Vector2 widthMinMax { get { return _widthMinMax; } }
    [SerializeField] float _changeWidthTime; 
    

    // particles that will be positioned to laserhit or particles that just will toggle whit laser activestate
    [Space(5), Header("PARTICLES"), Space(5)]
    [SerializeField] ParticleSystem[] _hitParticleSystems;
    [SerializeField] ParticleSystem[] _otherParticleSystems;


    bool _OverideDirection = false;
    public bool overideDirection { set { _OverideDirection = value; } }

    bool _isLethal = true;
    public bool isLethal { set { _isLethal = value; } }

    Vector3 _direction;
    public Vector3 direction { set { _direction = value; } }

    string _targetTag = "";
    public string targetTag { set { _targetTag = value; } }

    LineRenderer _lineRenderer;
    bool _isActive;

    // privates for dynamic width
    bool _isLerpingWidth;
    AnimationCurve _widthCurve;

    // will be called if a object with a specific tag have been hit or not
    public event Action TargetWasHit;
    public event Action TargetMissed;

    // subscribe to get HitPoint eachFrame;
    public event Action <Vector3> RayHitPoint;


    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;

        // need to store a copy of the animationcurve in the linerenderer
        // curve in linerenderer will be changed on lerp making us lose the originalcurve values we need when lerping back 
        _widthCurve = new AnimationCurve(_lineRenderer.widthCurve.keys);     
        
        // store the startdirection in direction
        _direction = _startDirection;

        // if constant, force to isActive (will not ever be active otherwise)
        _isActive = _constant ? true : _startActive;

        // set laser and particles to active/inactive (neccesary if starts inactive)
        _lineRenderer.enabled = _isActive;

        for (int i = 0; i < _hitParticleSystems.Length; i++)
            ToggleParticle(_hitParticleSystems[i], _isActive);

        for (int i = 0; i < _otherParticleSystems.Length; i++)
            ToggleParticle(_otherParticleSystems[i], _isActive);

        // if laser is not constant start coroutine that will toogle on/off
        if (!_constant)
            StartCoroutine(ToggleRay());
    }
   
    void Update()
    {     
        // if laser is active do raycast to get the endposition of line
        if (_isActive)
           DoRayCast();                    
    }

    public Vector3 GetLaserDirection()
    {
        return (_lineRenderer.GetPosition(1) - _lineRenderer.GetPosition(0)).normalized;
    }

   public void SetColor(Color color)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    void DoRayCast()
    {
        // if the direction is not overiden from other class calculate direction
        if (!_OverideDirection)
            CalculateDirection();
                
        RaycastHit2D hit = Physics2D.Raycast(_laserStartPosition.position, _direction, 100.0f);
        if (hit)
        {
            // set the start/end position of linerenderer
            _lineRenderer.SetPosition(0, _laserStartPosition.position);
            _lineRenderer.SetPosition(1, _laserStartPosition.position + (_direction * hit.distance));

            // loop over all hitParticles and set position to hitPoint
            for (int i = 0; i < _hitParticleSystems.Length; i++)
                _hitParticleSystems[i].transform.position = _lineRenderer.GetPosition(1);

            // deal damage if object have health
            if (hit.collider.gameObject.GetComponent<IDamageable>() != null && _isLethal)
                hit.collider.gameObject.GetComponent<IDamageable>().ModifyHealth(-_damage);

            // check if the object we hit have the set tag or not and notify subscribers
            if (hit.transform.tag != _targetTag && TargetMissed != null)
                TargetMissed();
            else if (hit.transform.tag == _targetTag && TargetWasHit != null)
                TargetWasHit();

            if (RayHitPoint != null)
                RayHitPoint(hit.point);

        }
        else
        {
            // if not hit anything(can happen if it goes out of room transition areas) just set it to the lenght of 100 units
            _lineRenderer.SetPosition(0, _laserStartPosition.position);
            _lineRenderer.SetPosition(1, _laserStartPosition.position + (_direction * 100.0f));
        }
        
    }

    void CalculateDirection()
    {
        // transform to localspace or just normalize in worldspace
        if (_localSpace)
            _direction = transform.TransformDirection(_startDirection).normalized;
        else
            _direction = _startDirection.normalized;
    
    }

    void LerpLaserWidth(bool enlargen)
    {
        // if not alredy in coroutine start lerping
        if(!_isLerpingWidth)
          StartCoroutine(LerpLaserWidthCo(enlargen));
    }

    public void SetWidth(float width, bool keepCurve)
    {
        // set width with keept curve or not
        _lineRenderer.startWidth = keepCurve ? width * _widthCurve.Evaluate(0) : width;
        _lineRenderer.endWidth = keepCurve ? width * _widthCurve.Evaluate(1) : width;        
    }

    IEnumerator LerpLaserWidthCo(bool enlargen)
    {
        _isLerpingWidth = true;

        // get from/to values depending of lerping from small to big and vice versa
        float from = enlargen ? _widthMinMax.x : _widthMinMax.y;
        float to = enlargen ? _widthMinMax.y : _widthMinMax.x;

        float fraction = 0.0f;
        while (fraction < 1.0f)
        {
            
            fraction += Time.deltaTime / _changeWidthTime;

            // add the values from animationcurve to keep the different widths depending on start/end of line
            _lineRenderer.startWidth = Mathf.Lerp(from, to, fraction) * _widthCurve.Evaluate(0);
            _lineRenderer.endWidth = Mathf.Lerp(from, to, fraction) *_widthCurve.Evaluate(1);

            yield return null;
        }

        _isLerpingWidth = false;

    }

    IEnumerator ToggleRay()
    {
        float timer = 0.0f;
        int index = 0; // keep track of witch time we will use in array

        while (true)
        {
            timer += Time.deltaTime;

            // if we are using dynamicwidth we start to lerp width before the laser gets toggled off
            if (_dynamicWidthOnToggle && _isActive && timer >= toggleActiveTimes[index] - _changeWidthTime)
                LerpLaserWidth(false);
           
            if (timer >= toggleActiveTimes[index])
            {
                // toggle linerenderer and raycast
                _isActive = !_isActive;
                _lineRenderer.enabled = _isActive;

                // if the laser just got activated and we are using dynamicwidth, lerp back to maxsize
                if (_dynamicWidthOnToggle && _isActive)
                    LerpLaserWidth(true);

                // if we just got active do a raycast right away to avoid line being rendered on old positions
                if (_isActive)
                    DoRayCast();
                
                //loop over and toogle all particles
                for (int i = 0; i < _hitParticleSystems.Length; i++)                
                    ToggleParticle(_hitParticleSystems[i], _isActive);

                for (int i = 0; i < _otherParticleSystems.Length; i++)
                    ToggleParticle(_otherParticleSystems[i], _isActive);               

                // reset timer and move to next time in array
                timer = 0.0f;
                index++;

                // reset to 0 if outofbounds
                if (index == toggleActiveTimes.Length)
                    index = 0;
            }            
            yield return null;
        }

    }

    void ToggleParticle(ParticleSystem system, bool active)
    {
        if (!active)
        {            
            system.Stop(true, ParticleSystemStopBehavior.StopEmitting);           
        }
        else
        {            
            system.Play(true);            
        }

    }
}

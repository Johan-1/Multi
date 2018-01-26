using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{

    [SerializeField] Vector3 _startDirection;
    
    [SerializeField] Transform _laserStartPosition; public Transform raycastPoint { get{ return _laserStartPosition; }}

    [SerializeField] bool _constant;
    [SerializeField] bool _startActive;
    [SerializeField] bool _localSpace;
    [SerializeField] int _damage;

    // have array of toogletimes to be able to create moore diverse patterns
    [SerializeField] float[] toggleActiveTimes;

    // particles that will be positioned to laserhit or particles that just will toggle whit laser activestate
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

    // will be called if a object with a specific tag have been hit or not
    public event Action TargetWasHit;
    public event Action TargetMissed;

    // subscribe to get HitPoint eachFrame;
    public event Action <Vector3> RayHitPoint;


    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        
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

    IEnumerator ToggleRay()
    {
        float timer = 0.0f;
        int index = 0; // keep track of witch time we will use in array

        while (true)
        {
            timer += Time.deltaTime;

            if (timer >= toggleActiveTimes[index])
            {
                // toggle linerenderer and raycast
                _isActive = !_isActive;
                _lineRenderer.enabled = _isActive;

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

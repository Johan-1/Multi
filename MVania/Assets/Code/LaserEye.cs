using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Laser))]
public class LaserEye : MonoBehaviour
{

    [SerializeField] float _chargingTime = 3.0f;
    [SerializeField] Color _chargeZeroColor;
    [SerializeField] Color _chargeFullColor;
    [SerializeField] bool _resetOnLosingTarget;
    [SerializeField] bool _useFollowSmoothing;
    [SerializeField] float _followSpeed;
    [SerializeField] float _cooldown;
    [SerializeField] string _targetTag;


    [SerializeField] ParticleSystem[] _hitParticles;

    float _currentChargeTime;
    Laser _laser;
    GameObject _player;
    Vector3 _LaserHitPoint;


    Vector3 _laserPosition;

    Vector3 _lastDirection;
    Vector3 _fromDirection;
    float _smoothFraction;

    Vector3 kk;

    void Awake()
    {
        _player = FindObjectOfType<PlayerAbilitys>().gameObject;
        _laser = GetComponent<Laser>();
    }

    void Start()
    {

        // add functions to delegates that gets called if player gets hit or not
        // this makes chargetimer reset when object intersect with player(ray will always hit player when fullycharged)
        if (_resetOnLosingTarget)
        {
            _laser.TargetWasHit += ChargeLaser;
            _laser.TargetMissed += () => { _currentChargeTime = 0.0f; };           
        }       
        // get rayhitpoint
        _laser.RayHitPoint += (Vector3 hit) => { _LaserHitPoint = hit; };
        
        // we will overide direction set in editor of laserclass
        // start non lethal, set tag of target and get the origin of laser
        _laser.overideDirection = true;
        _laser.isLethal = false;
        _laser.targetTag = _targetTag;
        _laserPosition = _laser.raycastPoint.position;              
    }

    void Update()
    {
        FocusOnPlayer();

        if (!_resetOnLosingTarget)
            ChargeLaser();

        SetColor();
    }

    void FocusOnPlayer()
    {
        // get direction to player
        Vector3 direction = (_player.transform.position - _laserPosition).normalized;

        // do a slerp from current to target if using smoothing
        if (_useFollowSmoothing)
        {
            // reset lerp if target has changed
            if (_lastDirection != direction)
            {
                _smoothFraction = 0.0f;
                _fromDirection = _laser.GetLaserDirection();
                _lastDirection = direction;                             
            }
            
            // add to fraction and update direction
            _smoothFraction += Time.deltaTime * _followSpeed;
            direction = Vector3.Lerp(_fromDirection, direction, _smoothFraction);             
        }
       
        // set direction of laser
        _laser.direction = direction;
    }

    void ChargeLaser()
    {
        // chargeup laser
        // gets called from delegate in laserClass if set to resetOnLosingtarget, else called localy from update
        _currentChargeTime += Time.deltaTime;
        if (_currentChargeTime >= _chargingTime)        
            StartCoroutine(DoHit());
        
    }

    void SetColor()
    {
        // lerp between 0% chargedColor and 100% chargedColor
        float fraction = Mathf.InverseLerp(0, _chargingTime, _currentChargeTime);        
        _laser.SetColor(Color.Lerp(_chargeZeroColor, _chargeFullColor, fraction));
    }

    IEnumerator DoHit()
    {
        //set laser to lethal so it can do damage
        _laser.isLethal = true;       

        // loop over and instantiate all particles on hit point
        for (int i = 0; i < _hitParticles.Length; i++)
        {
            ParticleSystem particle = Instantiate(_hitParticles[i], _LaserHitPoint, Quaternion.identity);
            Destroy(particle, 5.0f);
        }

        // wait one frame before setting back to non lethal
        yield return null;
        _laser.isLethal = false;

        // yield during cooldown and keep setting chargetime to below zero(this makes the color stay on 0% chargecolor during cooldown)
        float timer = 0.0f;
        while (timer < _cooldown)
        {
            timer += Time.deltaTime;
            _currentChargeTime = -1.0f;
            yield return null;
        }

        // set back chargetime to 0 so everything can startover
        _currentChargeTime = 0.0f;        
    }
    

}

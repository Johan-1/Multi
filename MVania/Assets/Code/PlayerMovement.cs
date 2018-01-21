using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour 
{

    [Header("MOVEMENT AND JUMP"), Space(5)]
    [SerializeField] float _moveSpeed = 1.0f;   
    [SerializeField] float _jumpForce = 1.0f;
    [SerializeField] float _jumpInputTime = 0.25f;    
    [SerializeField] int _numberOfAirJumps = 1;

    [Space(10), Header("WALLJUMP"), Space(5)]
    [SerializeField] float _wallSlideSpeed = 10.0f;
    [SerializeField] float _dropFromWallDelay = 0.2f;
    [SerializeField] float _timeAddingWallPushOutForce = 0.2f;
    [SerializeField] Vector2 _wallPushOutForce;
    [SerializeField] AnimationCurve _wallPushOutCurveX;
    [SerializeField] AnimationCurve _wallPushOutCurveY;

    [Space(10), Header("DASH"), Space(5)]
    [SerializeField] float _timeDashing = 0.2f;
    [SerializeField] float _dashSpeed = 10.0f;
    [SerializeField] float _dashCooldown = 2.0f;
              
    // movement/jump privates   
    float _xInput;
    int _airJumpCount = 0;   
    bool _movingRight;

    float _wallTimer;
    // references
    Rigidbody2D _rigidBody;
    PlayerAbilitys _playerAbilitys;
   
    [Flags] enum MOVEMENTFLAGS
    {
        NONE            = 0,
        GROUNDED        = 1 << 0,
        LOCKEDVELOCITY  = 1 << 1,
        ONWALL          = 1 << 2,       
        DASHREADY       = 1 << 3,      
    }

    MOVEMENTFLAGS _MOVEFLAGS = MOVEMENTFLAGS.DASHREADY;


    void AddFlag( MOVEMENTFLAGS flag)
    {
        _MOVEFLAGS |= flag;
    }

    void RemoveFlag( MOVEMENTFLAGS flag)
    {
        _MOVEFLAGS &= (~flag);
    }

    bool HasFlag( MOVEMENTFLAGS flag)
    {        
        return (_MOVEFLAGS & flag) == flag;
    }

    // takes in multiple flags , only returns true if none of the flags is set 
    // if you only need to check if 1 flag is not set you can just use "!HasFlag()"
    bool DontHaveFlags(MOVEMENTFLAGS flags)
    {                
        return (_MOVEFLAGS & flags) == 0;     
    }

    void Start()
    {        
        _rigidBody = GetComponent<Rigidbody2D>();
        _playerAbilitys = GetComponent<PlayerAbilitys>();
        
        DontDestroyOnLoad(this);                               
    }

    void Update()
    {
        HandleMovement();
        HandleJumping();

        if(_playerAbilitys.AbilityUnlocked(PowerUp.POWERUPTYPE.WALLJUMP))
            HandleWalljumping();

        if(_playerAbilitys.AbilityUnlocked(PowerUp.POWERUPTYPE.DASH))
            HandleDashing();
    }
  
    void HandleMovement()
    {
        // get x input 
        _xInput = Input.GetAxisRaw("Horizontal");

        // only can move if velocity is not locked to an ability and we are not wall sliding(moving of wall is handled in wallsliding function)
        if (HasFlag(MOVEMENTFLAGS.LOCKEDVELOCITY) || HasFlag(MOVEMENTFLAGS.ONWALL) && !HasFlag(MOVEMENTFLAGS.GROUNDED))
            return;
               
        // change forward facing of player depending on moving left/right
        if (_xInput < 0)
        {
            transform.right = -Vector2.right;
            _movingRight = false;
        }
        else if (_xInput > 0)
        {
            transform.right = Vector2.right;
            _movingRight = true;
        }
                
         _rigidBody.velocity = new Vector2(_xInput * _moveSpeed, _rigidBody.velocity.y);

    }

    void HandleJumping()
    {
        // return if our velocity is controlled from something else
        if (HasFlag(MOVEMENTFLAGS.LOCKEDVELOCITY))
            return;

        // check if grounded 
        if (CheckGrounded())
        {
            if (Input.GetButtonDown("Jump"))            
                StartCoroutine(AnalogJump());                                           
        }
        else if (_playerAbilitys.AbilityUnlocked(PowerUp.POWERUPTYPE.AIRJUMP) && !HasFlag(MOVEMENTFLAGS.ONWALL)) // if not grounded check if airjump ability is unlocked and that we are not on a wall
        {
            if (Input.GetButtonDown("Jump") && _airJumpCount < _numberOfAirJumps) // do jump in air if we not alredy have done max airjumps
            {                               
                _airJumpCount++;
                StartCoroutine(AnalogJump());                
            }
        }
    }

    IEnumerator AnalogJump()
    {
      
        // check for jump input during the input window time
        // add y velocity ass long as jumpbutton is held down
        // else break out and cancel adding yforce
        float time = 0;
        while (time < _jumpInputTime)
        {
            time += Time.deltaTime;

            if (Input.GetButton("Jump"))            
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _jumpForce);                                                    
            else
                yield break;

            yield return null;
        }                               
        
    }

    bool CheckGrounded()
    {
       
        // ray info
        float[] xOffset = new float[3] { -0.35f, 0.0f, 0.35f };               
        float yOffset = -0.3f;
        float lenght = 0.2f;
        int hitCount = 0;

        for (int i = 0; i < 3; i++)
        {
            // set start pos of rays
            Vector3 origin = transform.position + new Vector3(xOffset[i], yOffset, 0);
                       
            // if hit add to hitcounter
            if (Physics2D.Raycast(origin, Vector3.down, lenght, LayerMask.GetMask("Ground")))            
                 hitCount++;

            Debug.DrawLine(origin, origin + new Vector3(0, -lenght, 0), Color.red); // debug lines  
        }

        // if atleast one hit we are standing on ground
        if (hitCount > 0)
        {            
            AddFlag(MOVEMENTFLAGS.GROUNDED);
            _airJumpCount = 0; // reset jump count when on ground
        }
        else
            RemoveFlag(MOVEMENTFLAGS.GROUNDED);

        return HasFlag(MOVEMENTFLAGS.GROUNDED);
            
    }

    void HandleWalljumping()
    {
       
        // check if we are hitting a wall
        RaycastWall();

        // if we are not grounded and is on wall
        if (!HasFlag(MOVEMENTFLAGS.GROUNDED) && HasFlag(MOVEMENTFLAGS.ONWALL))
        {
            // if on wall set velocity to slideSpeed
            if (_rigidBody.velocity.y < 0.0f)
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _wallSlideSpeed);

            // reset airjumps if on wall
            _airJumpCount = 0;

            // if jump is pressed and we are not dashing in wall do walljump
            if (Input.GetButtonDown("Jump") && !HasFlag(MOVEMENTFLAGS.LOCKEDVELOCITY))                          
                StartCoroutine(DoWalljump());
                           
        }       
           
    }

    IEnumerator DoWalljump()
    {
        // lock the player from controll while push out from wall  
        // set walljump to true so we add force during physics uppdate
        AddFlag(MOVEMENTFLAGS.LOCKEDVELOCITY);
                     
        // set positive or negative force depending on if wall is to our left or right
        float xVelocity = _movingRight ? -Vector3.right.x * _wallPushOutForce.x : Vector3.right.x * _wallPushOutForce.x;

        // the amount of time we will be locked in jumping away from wall
        float forcedJumpforceTimer = .0f;
        while (forcedJumpforceTimer < _timeAddingWallPushOutForce)
        {
            // animation curves for more control of acceleration/deacceleration of jump 
            float wallOutForceX = xVelocity * _wallPushOutCurveX.Evaluate(Mathf.InverseLerp(0.0f, _timeAddingWallPushOutForce, forcedJumpforceTimer));
            float wallOutForceY = _wallPushOutForce.y * _wallPushOutCurveY.Evaluate(Mathf.InverseLerp(0.0f, _timeAddingWallPushOutForce, forcedJumpforceTimer));

            forcedJumpforceTimer += Time.deltaTime;

            _rigidBody.velocity = new Vector2(wallOutForceX, wallOutForceY);
            
            yield return null;
        }

        // give player back the control        
        RemoveFlag(MOVEMENTFLAGS.LOCKEDVELOCITY);
        
    }

    void RaycastWall()
    {
        if (Physics2D.Raycast(transform.position, _movingRight ? Vector3.right : -Vector3.right, 0.6f, LayerMask.GetMask("Ground")))
        {
            AddFlag(MOVEMENTFLAGS.ONWALL);
            HandleSlideOffWall();           
        }           
        else
            RemoveFlag(MOVEMENTFLAGS.ONWALL);                                   
    }

    void HandleSlideOffWall()
    {
        _wallTimer += Time.deltaTime;

        // check if we have input away from wall
        // if true we will stick to wall for a little longer before we allow velocity change
        // this make us still be abble to walljump even if we have x-input away from wall before jump input
        if (_movingRight && _xInput < 0)
        {
            if (_wallTimer > _dropFromWallDelay)              
                _rigidBody.velocity = new Vector2(_xInput * _moveSpeed, _rigidBody.velocity.y);
        }
        else if (!_movingRight && _xInput > 0)
        {
            if (_wallTimer > _dropFromWallDelay)              
                _rigidBody.velocity = new Vector2(_xInput * _moveSpeed, _rigidBody.velocity.y);
        }
        else // reset timer if we have no input
            _wallTimer = 0;
    }

    void HandleDashing()
    {
        if (DontHaveFlags(MOVEMENTFLAGS.GROUNDED | MOVEMENTFLAGS.LOCKEDVELOCITY) && HasFlag(MOVEMENTFLAGS.DASHREADY) && Input.GetButtonDown("Fire1") )                   
            StartCoroutine(Dash());
        
    }

    IEnumerator Dash()
    {
        // add and remove flags
        AddFlag(MOVEMENTFLAGS.LOCKEDVELOCITY);      
        RemoveFlag(MOVEMENTFLAGS.DASHREADY);

        // set velocity depending if moving left/right
        Vector2 velocity = _movingRight ? Vector3.right * _dashSpeed : -Vector3.right * _dashSpeed;

        // if we are dashing from a wall, invert velocity and movedirection
        if (HasFlag(MOVEMENTFLAGS.ONWALL))
        {
            velocity = -velocity;
            _movingRight = !_movingRight;
            transform.right = -transform.right;           
        }

        // add velocity as long as in dash
        float timer = 0.0f;
        while (timer < _timeDashing)
        {
            _rigidBody.velocity = velocity;  
            
            timer += Time.deltaTime;
            yield return null;
        }

        // remove flags
        RemoveFlag(MOVEMENTFLAGS.LOCKEDVELOCITY);
        
        yield return new WaitForSeconds(_dashCooldown);
         // add flag that dashcooldown is over
        AddFlag(MOVEMENTFLAGS.DASHREADY);

    }

    void OnDisable()
    {
        // abort all specialmovement if any is ongoing
        // gets called on death and respawn
        StopAllCoroutines();
        _MOVEFLAGS = MOVEMENTFLAGS.DASHREADY;
    }

    // gets called from healthcomponent on getting hit
    public void HandleKnockback(Vector2 knockbackForce, float knockbackTime)
    {
        // stop all specialmovement when getting knockedback (ex if in dash when taking a hit)
        StopAllCoroutines();
        _MOVEFLAGS = MOVEMENTFLAGS.DASHREADY;

        StartCoroutine(DoKnockback(knockbackForce, knockbackTime));
    }

    IEnumerator DoKnockback(Vector2 knockbackForce, float knockbackTime)
    {        
        AddFlag(MOVEMENTFLAGS.LOCKEDVELOCITY);

        // set velcity of knockback depending if moving left/right
        _rigidBody.velocity = _movingRight ? (-Vector3.right * knockbackForce.x) + (Vector3.up * knockbackForce.y) : (Vector3.right * knockbackForce.x) + (Vector3.up * knockbackForce.y);

        // the amount of time the player looses control
        float timer = 0.0f;
        while (timer < knockbackTime)
        {           
            timer += Time.deltaTime;
            yield return null;
        }
      
        RemoveFlag(MOVEMENTFLAGS.LOCKEDVELOCITY);
        
    }

}

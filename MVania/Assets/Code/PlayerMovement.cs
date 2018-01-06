using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour 
{

    [Header("MOVEMENT AND JUMP"), Space(5)]
    [SerializeField] float _moveSpeed = 1.0f;
    [SerializeField] float _moveSpeedAir = 5.0f;
    [SerializeField] float _jumpForce = 1.0f;
    [SerializeField] float _jumpInputTime = 0.25f;
    [SerializeField] float _maxXVelocity = 20.0f;
    [SerializeField] int _numberOfAirJumps = 1;

    [Space(10), Header("WALLJUMP"), Space(5)]
    [SerializeField] float _gravityOnWall = 10.0f;
    [SerializeField] float _timeAddingWallPushOutForce = 0.2f;
    [SerializeField] Vector2 _wallPushOutForce;
    [SerializeField] AnimationCurve _wallPushOutCurveX;
    [SerializeField] AnimationCurve _wallPushOutCurveY;

    [Space(10), Header("DASH"), Space(5)]
    [SerializeField] float _timeDashing = 0.2f;
    [SerializeField] float _dashSpeed = 10.0f;
    [SerializeField] float _dashCooldown = 2.0f;
    
        
    // walljump privates   
    float _wallOutForceX;
    float _wallOutForceY;

    // movement/jump privates
    float _defaultGravity;
    float _xInput;
    int _airJumpCount = 0;   
    bool _movingRight;
   
    
    // references
    Rigidbody2D _rigidBody;
    PlayerAbilitys _playerAbilitys;

    
    [Flags] enum MOVEMENTFLAGS
    {
        NONE            = 0,
        GROUNDED        = 1 << 0,
        LOCKEDVELOCITY  = 1 << 1,
        ONWALL          = 1 << 2,
        WALLJUMPING     = 1 << 3,
        JUMPING         = 1 << 4,
        DASHING         = 1 << 5,
        DASHREADY       = 1 << 6,
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

        _defaultGravity = _rigidBody.gravityScale;

        DontDestroyOnLoad(this);                               
    }


    void Update()
    {
        HandleMovement();
        HandleJumping();

        if(_playerAbilitys.wallJumpUnlocked)
            HandleWalljumping();

        if(_playerAbilitys.dashUnlocked)
            HandleDashing();
    }

    void FixedUpdate()
    {
        ApplyPhysics();
        
    }

    void HandleMovement()
    {
        // get x input 
        _xInput = Input.GetAxisRaw("Horizontal");
       
        // change forward facing of player depending on moving left/right
        if (_rigidBody.velocity.x != 0 && _rigidBody.velocity.x < 0)
        {
            transform.right = -Vector2.right;
            _movingRight = false;
        }
        else if (_rigidBody.velocity.x != 0 && _rigidBody.velocity.x > 0)
        {
            transform.right = Vector2.right;
            _movingRight = true;
        }

        // if our velocity is locked return without setting 
        if (HasFlag(MOVEMENTFLAGS.LOCKEDVELOCITY))
            return;

        // if we are grounded set velocity to a constant speed
        if(HasFlag(MOVEMENTFLAGS.GROUNDED))
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
        else if (_playerAbilitys.airJumpsUnlocked && !HasFlag(MOVEMENTFLAGS.ONWALL)) // if not grounded check if airjump ability is unlocked and that we are not on a wall
        {
            if (Input.GetButtonDown("Jump") && _airJumpCount < _numberOfAirJumps) // do jump in air if we not alredy have done max airjumps
            {
                // set velocity to zero so we get the same power in jump no matter our fallspeed
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
                _airJumpCount++;
                StartCoroutine(AnalogJump());
                
            }
        }
    }

    IEnumerator AnalogJump()
    {
        
        AddFlag(MOVEMENTFLAGS.JUMPING);
        
        // check for jump input during the input window time
        // add y velocity ass long as jumpbutton is held down
        // else break out and cancel adding yforce
        float time = 0;
        while (time < _jumpInputTime)
        {
            time += Time.deltaTime;

            if (!Input.GetButton("Jump"))
            {
                RemoveFlag(MOVEMENTFLAGS.JUMPING);
                yield break;
            }                       
            yield return null;
        }
        RemoveFlag(MOVEMENTFLAGS.JUMPING);
    }

    bool CheckGrounded()
    {

        // always want back raycast to be a bit behind player when jumping of edges
        float[] xOffset;
        if (_movingRight)
            xOffset = new float[3] { -1.0f, 0.0f, 0.35f };
        else
            xOffset = new float[3] { -0.35f, 0.0f, 1.0f };

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
        // if we are not grounded do raycast to see if wall is hit

        RaycastWall();

        if (!HasFlag(MOVEMENTFLAGS.GROUNDED) && HasFlag(MOVEMENTFLAGS.ONWALL))
        {
            // if on wall lower gravity so we will slide down
            if(_rigidBody.velocity.y < 0.0f)
                _rigidBody.gravityScale = _gravityOnWall;

            // reset airjumps if on wall
            _airJumpCount = 0;

            // if jump is pressed we will set back gravity and do wall jump
            if (Input.GetButtonDown("Jump") && !HasFlag(MOVEMENTFLAGS.DASHING))
            {
                _rigidBody.gravityScale = _defaultGravity;
                StartCoroutine(DoWalljump());
            }
                
        }
        else
        {
            _rigidBody.gravityScale = _defaultGravity;
        }
           
    }

    IEnumerator DoWalljump()
    {
        // lock the player from controll while push out from wall  
        // set walljump to true so we add force during physics uppdate
        AddFlag(MOVEMENTFLAGS.LOCKEDVELOCITY);
        AddFlag(MOVEMENTFLAGS.WALLJUMPING);       

        // set velocity to zero so we get the same power in jump no matter the force we had before
        _rigidBody.velocity = Vector2.zero;

        // set positive or negative force depending on if wall is to our left or right
        float xVelocity = _movingRight ? -Vector3.right.x * _wallPushOutForce.x : Vector3.right.x * _wallPushOutForce.x;

        // the amount of time we will be locked in jumping away from wall
        float forcedJumpforceTimer = .0f;
        while (forcedJumpforceTimer < _timeAddingWallPushOutForce)
        {
            // animation curves for more control of acceleration/deacceleration of jump 
            _wallOutForceX = xVelocity * _wallPushOutCurveX.Evaluate(Mathf.InverseLerp(0.0f, _timeAddingWallPushOutForce, forcedJumpforceTimer));
            _wallOutForceY = _wallPushOutForce.y * _wallPushOutCurveY.Evaluate(Mathf.InverseLerp(0.0f, _timeAddingWallPushOutForce, forcedJumpforceTimer));

            forcedJumpforceTimer += Time.deltaTime;    
            
            yield return null;
        }

        // give player back the control        
        RemoveFlag(MOVEMENTFLAGS.LOCKEDVELOCITY);
        RemoveFlag(MOVEMENTFLAGS.WALLJUMPING);

    }

    void RaycastWall()
    {
        if (Physics2D.Raycast(transform.position, _movingRight ? Vector3.right : -Vector3.right, 0.6f, LayerMask.GetMask("Ground")))
            AddFlag(MOVEMENTFLAGS.ONWALL);
        else
            RemoveFlag(MOVEMENTFLAGS.ONWALL);                                   
    }

    void HandleDashing()
    {
        if (DontHaveFlags(MOVEMENTFLAGS.GROUNDED | MOVEMENTFLAGS.WALLJUMPING | MOVEMENTFLAGS.ONWALL) && HasFlag(MOVEMENTFLAGS.DASHREADY) && Input.GetButtonDown("Fire1") )
        {            
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        AddFlag(MOVEMENTFLAGS.LOCKEDVELOCITY);
        AddFlag(MOVEMENTFLAGS.DASHING);
        RemoveFlag(MOVEMENTFLAGS.DASHREADY);
        _rigidBody.gravityScale = 0.0f;

        float timer = 0.0f;
        while (timer < _timeDashing)
        {

            _rigidBody.velocity = _movingRight ? Vector3.right * _dashSpeed : -Vector3.right * _dashSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        _rigidBody.gravityScale = _defaultGravity;
        RemoveFlag(MOVEMENTFLAGS.LOCKEDVELOCITY);
        RemoveFlag(MOVEMENTFLAGS.DASHING);

        yield return new WaitForSeconds(_dashCooldown);

        AddFlag(MOVEMENTFLAGS.DASHREADY);

    }

    void ApplyPhysics()
    {

        // apply jump force if we are in a jump
        if (HasFlag(MOVEMENTFLAGS.JUMPING) && !HasFlag(MOVEMENTFLAGS.LOCKEDVELOCITY))
        {
            _rigidBody.AddForce(new Vector2(0, _jumpForce));
        }

        // apply force on x-axis if we are in air
        // x velocity is set directly to constant value if on ground
        if (DontHaveFlags(MOVEMENTFLAGS.GROUNDED | MOVEMENTFLAGS.LOCKEDVELOCITY))
        {
            _rigidBody.AddForce(new Vector2(_xInput * _moveSpeedAir, 0));

            // clamp velocity to max allowed speed
            float currentVelocityX = _rigidBody.velocity.x;
            currentVelocityX = Mathf.Clamp(currentVelocityX, -_maxXVelocity, _maxXVelocity);
            _rigidBody.velocity = new Vector2(currentVelocityX, _rigidBody.velocity.y);

        }

        // if in forced wallPushoutjump add wallpushforce
        if (HasFlag(MOVEMENTFLAGS.WALLJUMPING))
        {
            _rigidBody.AddForce(new Vector2(_wallOutForceX, _wallOutForceY));

        }
    }

}

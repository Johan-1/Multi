using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour 
{

    [SerializeField] float _moveSpeed = 1.0f;
    [SerializeField] float _moveSpeedAir = 5.0f;
    [SerializeField] float _jumpForce = 1.0f;
    [SerializeField] float _jumpInputTime = 0.25f;
    [SerializeField] float _maxXVelocity = 20.0f;
    [SerializeField] int _numberOfAirJumps = 1;
    [SerializeField] float _gravityOnWall = 10.0f;
    [SerializeField] float _timeAddingWallPushOutForce = 0.2f;
    [SerializeField] Vector2 _wallPushOutForce;
    
    

    [SerializeField] AnimationCurve _wallPushOutCurveX;
    [SerializeField] AnimationCurve _wallPushOutCurveY;

    float _wallOutForceX;
    float _wallOutForceY;
    float _defaultGravity;
    float _xInput;
    int _airJumpCount = 0;

    bool _grounded;
    bool _movingRight;
    bool _lockedVelocity;
    bool _onWall;
    bool _wallJumping;
    bool _jumping;
    
    Rigidbody2D _rigidBody;
    PlayerAbilitys _playerAbilitys;

    
   
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
        if (_lockedVelocity)
            return;

        // if we are grounded set velocity to a constant speed
        if(_grounded)
            _rigidBody.velocity = new Vector2(_xInput * _moveSpeed, _rigidBody.velocity.y);

    }

    void HandleJumping()
    {
        // return if our velocity is controlled from something else
        if (_lockedVelocity)
            return;

        // check if grounded 
        if (CheckGrounded())
        {
            if (Input.GetButtonDown("Jump"))
                StartCoroutine(AnalogJump());
        }
        else if (_playerAbilitys.airJumpsUnlocked && !_onWall) // if not grounded check if airjump ability is unlocked and that we are not on a wall
        {
            if (Input.GetButtonDown("Jump") && _airJumpCount < _numberOfAirJumps) // do jump in air if we not alredy have done max airjumps
            {
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
                StartCoroutine(AnalogJump());
                _airJumpCount++;
            }
        }
    }

    IEnumerator AnalogJump()
    {

        _jumping = true;      
        
        // check for jump input during the input window time
        // add y velocity ass long as jumpbutton is held down
        // else break out and cancel adding yforce
        float time = 0;
        while (time < _jumpInputTime)
        {
            time += Time.deltaTime;

            if (!Input.GetButton("Jump"))
            {
                _jumping = false;
                yield break;
            }                       
            yield return null;
        }
        _jumping = false;
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
            _grounded = true;           
            _airJumpCount = 0; // reset jump count when on ground
        }
        else       
            _grounded = false;
                                  
        return _grounded;
            
    }

    void HandleWalljumping()
    {
        // if we are not grounded do raycast to see if wall is hit
        if (!_grounded && RaycastWall())
        {
            // if on wall lower gravity so we will slide down
            if(_rigidBody.velocity.y < 0.0f)
                _rigidBody.gravityScale = _gravityOnWall;

            // reset airjumps if on wall
            _airJumpCount = 0;

            // if jump is pressed we will set back gravity and do wall jump
            if (Input.GetButtonDown("Jump"))
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
        _lockedVelocity = true;
        _wallJumping = true;

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
        _wallJumping = false;
        _lockedVelocity = false;

        

       
    }

    bool RaycastWall()
    {
        _onWall = (Physics2D.Raycast(transform.position, _movingRight ? Vector3.right : -Vector3.right, 0.6f, LayerMask.GetMask("Ground")));
        return _onWall;
                            
    }

    void ApplyPhysics()
    {

        // apply jump force if we are in a jump
        if (_jumping && !_lockedVelocity)
        {
            _rigidBody.AddForce(new Vector2(0, _jumpForce));
        }

        // apply force on x-axis if we are in air
        // x velocity is set directly to constant value if on ground
        if (!_grounded && !_lockedVelocity)
        {
            _rigidBody.AddForce(new Vector2(_xInput * _moveSpeedAir, 0));

            // clamp velocity to max allowed speed
            float currentVelocityX = _rigidBody.velocity.x;
            currentVelocityX = Mathf.Clamp(currentVelocityX, -_maxXVelocity, _maxXVelocity);
            _rigidBody.velocity = new Vector2(currentVelocityX, _rigidBody.velocity.y);

        }

        // if in forced wallPushoutjump add wallpushforce
        if (_wallJumping)
        {
            _rigidBody.AddForce(new Vector2(_wallOutForceX, _wallOutForceY));

        }
    }

}

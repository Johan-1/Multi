using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour 
{

    [SerializeField] float _moveSpeed = 1.0f;
    [SerializeField] float _jumpForce = 1.0f;
    [SerializeField] float _jumpInputTime = 0.25f;
    [SerializeField] int _numberOfAirJumps = 1;

    int _airJumpCount = 0;
    bool _grounded;
    bool _movingRight;
    Rigidbody2D _rigidBody;
    PlayerAbilitys _playerAbilitys;

    
   
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
    }


    void HandleMovement()
    {
        // get x input and set velocity
        float XInput = Input.GetAxisRaw("Horizontal");
        _rigidBody.velocity = new Vector2(XInput * _moveSpeed, _rigidBody.velocity.y);

        // change forward facing of player depending on moving left/right
        if (XInput != 0 && XInput < 0)
        {
            transform.right = -Vector2.right;
            _movingRight = false;
        }
        else if (XInput != 0 && XInput > 0)
        {
            transform.right = Vector2.right;
            _movingRight = true;
        }
            
    }

    void HandleJumping()
    {
        // check if grounded 
        if (CheckGrounded())
        {
            if (Input.GetButtonDown("Jump"))
                StartCoroutine(AnalogJump());
        }
        else if (_playerAbilitys.airJumpsUnlocked) // if not grounded check if airjump ability is unlocked
        {
            if (Input.GetButtonDown("Jump") && _airJumpCount < _numberOfAirJumps) // do jump in air if we not alredy have done max airjumps
            {
                StartCoroutine(AnalogJump());
                _airJumpCount++;
            }
        }
    }

    IEnumerator AnalogJump()
    {
        float time = 0;
        _airJumpCount++;

        // check for jump input during the input window time
        // add y velocity ass long as jumpbutton is held down
        // else break out and cancel adding yforce
        while (time < _jumpInputTime)
        {
            time += Time.deltaTime;

            if (Input.GetButton("Jump"))
                _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, _jumpForce, 0);
            else
                yield break;

            yield return null;
        }

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

}

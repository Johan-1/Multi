using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour 
{

    [SerializeField] float _moveSpeed = 1.0f;
    [SerializeField] float _jumpForce = 1.0f;
    [SerializeField] float _jumpInputTime = 0.25f;

    bool _grounded;
    Rigidbody2D _rigidBody;

    static int _numPlayers;

    void Awake()
    {
        if (_numPlayers > 0)
            DestroyImmediate(gameObject);

        _numPlayers++;
    }

    void Start()
    {        
        _rigidBody = GetComponent<Rigidbody2D>();
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
            transform.right = -Vector2.right;
        else if (XInput != 0 && XInput > 0)
            transform.right = Vector2.right;
    }

    void HandleJumping()
    {       
        // check if grounded and if jump just was pressed 
        if (CheckGrounded() && Input.GetButtonDown("Jump"))
            StartCoroutine(AnalogJump());            

    }

    IEnumerator AnalogJump()
    {
        float time = 0;

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
        // cast rays from 3 positions starting from the bottom left corner
        Vector3 startOrigin = transform.position + new Vector3(-.5f, -.5f, .0f);
        int hitCount = 0;

        for (int i = 0; i < 3; i++)
        {
            Debug.DrawLine(startOrigin, startOrigin + new Vector3(0, -.25f, 0), Color.red); // debug lines

            // if hit add to hitcounter
            if (Physics2D.Raycast(startOrigin, Vector3.down, 0.25f, LayerMask.GetMask("Ground")))            
                 hitCount++;
            
            startOrigin.x += .5f; // move origin so we cast from middle and right aswell

        }

        // if atleast one hit we are standing on ground
        if (hitCount > 0)
            _grounded = true;
        else
            _grounded = false;

        return _grounded;
    }

}

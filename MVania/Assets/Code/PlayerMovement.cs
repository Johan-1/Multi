using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour 
{

    [SerializeField] float _moveSpeed = 1.0f;
    [SerializeField] float _jumpForce = 1.0f;
    [SerializeField] float _jumpInputTime = 0.25f;

    Rigidbody2D _rigidBody;


    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();    
    }


    void Update()
    {
        HandleMovement();    
    }


    void HandleMovement()
    {
        float XInput = Input.GetAxisRaw("Horizontal");
        _rigidBody.velocity = new Vector2(XInput * _moveSpeed, _rigidBody.velocity.y);

        if (XInput != 0 && XInput < 0)
            transform.right = -Vector2.right;
        else if (XInput != 0 && XInput > 0)
            transform.right = Vector2.right;

    }


}

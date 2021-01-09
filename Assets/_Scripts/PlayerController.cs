using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(1, 10)]
    private float _speed = 5f;
    [SerializeField, Range(1, 20)]
    private float _jumpForce = 12f;
    private float _groundCheckRadius = 0.2f;
    private bool _grounded = true;
    private bool _facingRight = true;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private LayerMask _groundLayerMask;

    public Transform groundCheckOrigin;

    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _groundLayerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    private void Update()
    {
        Move();

        if (_grounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        
        // Blend between taking off and falling states when jumping
        _animator.SetFloat(Constants.VERTICALSPEED_F, _rigidbody.velocity.y);
    }

    private void FixedUpdate()
    {
        GroundCheck();
    }

    /// <summary>
    /// Player's movement
    /// </summary>
    private void Move()
    {
        float direction = Input.GetAxis("Horizontal");
        float movementAmount = _speed * direction;
        
        // Flip character
        if (movementAmount > 0f && !_facingRight)
        {
            Flip();
        }
        else if (movementAmount < 0f && _facingRight)
        {
            Flip();
        }
        
        transform.Translate(movementAmount * Time.deltaTime, 0f, 0f);
        _animator.SetFloat(Constants.SPEED_F, Mathf.Abs(direction));
    }

    /// <summary>
    /// Flip character according to the direction he's moving
    /// </summary>
    private void Flip()
    {
        _facingRight = !_facingRight;
        
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    /// <summary>
    /// Player's jump
    /// </summary>
    private void Jump()
    {
        _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        _animator.SetBool(Constants.ISJUMPING_B, true);
    }

    /// <summary>
    /// Check if player is or not grounded
    /// </summary>
    private void GroundCheck()
    {
        bool wasGrounded = _grounded;
        _grounded = false;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2) groundCheckOrigin.position,
                                                            _groundCheckRadius, 
                                                            _groundLayerMask);
        
        if (colliders.Length > 0)
        {
            _grounded = true;
            if (!wasGrounded)
            {
                _animator.SetBool(Constants.ISJUMPING_B, false);
            }
        }
        else if (colliders.Length == 0)
        {
            _grounded = false;
        }
    }
}
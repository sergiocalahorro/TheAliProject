using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Components
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private AudioSource _audioSource;
    
    // Player's movement
    [SerializeField]
    private float _movementSpeed;
    private float _horizontalInput;
    private bool _facingRight;

    // Player's jump
    [SerializeField]
    private float _jumpForce;
    private bool _canJump;
    private bool _isJumping;
    [SerializeField]
    private float _jumpHeightFactor;
    [SerializeField]
    private float _movementForceInAir;
    [SerializeField]
    private float _airDragMultiplier;

    // Ground check
    private LayerMask _groundLayerMask;
    [SerializeField]
    private float _groundCheckRadius;
    private bool _isGrounded;
    public Transform groundCheck;

    // Slope check
    [SerializeField]
    private float _slopeCheckDistance;
    private float _slopeDownAngle;
    private float _slopeSideAngle;
    private float _lastSlopeDownAngle;
    [SerializeField]
    private float _maxSlopeAngle;
    private bool _isOnSlope;
    private bool _canWalkOnSlope;
    private Vector2 _slopeNormalPerpendicular;

    // Materials
    [SerializeField]
    private PhysicsMaterial2D _zeroFriction;
    [SerializeField]
    private PhysicsMaterial2D _fullFriction;

    // Audio
    public AudioClip footstepsAudioClip;
    public AudioClip jumpAudioClip;
    public AudioClip takeDamageAudioClip;

    // Effects
    public GameObject dirt;

    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        _groundLayerMask = LayerMask.GetMask("Ground");

        _facingRight = true;
    }

    // Update is called once per frame
    private void Update()
    {
        CheckInput();
        AnimatorUpdate();
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        GroundCheck();
        SlopeCheck();
        Move();
    }

    /// <summary>
    /// Check keyboard input
    /// </summary>
    private void CheckInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // Stop jump at a variable height
        if (Input.GetButtonUp("Jump"))
        {
            VariableJump();
        }
    }

    /// <summary>
    /// Update animator's parameters
    /// </summary>
    private void AnimatorUpdate()
    {
        _animator.SetFloat(Constants.SPEED_F, Mathf.Abs(_horizontalInput));
        _animator.SetFloat(Constants.VERTICALSPEED_F, _rigidbody.velocity.y);
        _animator.SetBool(Constants.ISJUMPING_B, _isJumping);
        _animator.SetBool(Constants.ISGROUNDED_B, _isGrounded);
    }

    /// <summary>
    /// Player's movement
    /// </summary>
    private void Move()
    {
        // Flip character depending on his movement direction
        if (_horizontalInput > 0f && !_facingRight)
        {
            Flip();
        }
        else if (_horizontalInput < 0f && _facingRight)
        {
            Flip();
        }

        if (_isGrounded && !_isOnSlope && !_isJumping)
        {
            // If not on a slope
            _rigidbody.velocity = new Vector2(_movementSpeed * _horizontalInput, 0f);
        }
        else if (_isGrounded && _isOnSlope && _canWalkOnSlope && !_isJumping)
        {
            // If on a slope
            _rigidbody.velocity = new Vector2(_movementSpeed * _slopeNormalPerpendicular.x *
                                              -_horizontalInput,
                                              _movementSpeed * _slopeNormalPerpendicular.y *
                                              -_horizontalInput);
        }
        else if (!_isGrounded)
        {
            // If in air
            if (_horizontalInput != 0f)
            {
                // Add a force when moving in mid-air
                Vector2 airForce = new Vector2(_movementForceInAir * _horizontalInput, 0f);
                _rigidbody.AddForce(airForce);

                if (Mathf.Abs(_rigidbody.velocity.x) > _movementSpeed)
                {
                    _rigidbody.velocity = new Vector2(_movementSpeed * _horizontalInput,
                                                      _rigidbody.velocity.y);
                }
            }
            else
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x * _airDragMultiplier,
                                                  _rigidbody.velocity.y);
            }
        }

        if (_isGrounded && _horizontalInput != 0f)
        {
            // Play footsteps sound
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = footstepsAudioClip;
                _audioSource.volume = 0.1f;
                _audioSource.pitch = 1.4f;
                _audioSource.Play();
            }

            // Play dirt particles effect effect when walking
            Quaternion dirtRotation = Quaternion.identity;

            if (_facingRight)
            {
                dirtRotation.y = 270f;
            }
            else if (!_facingRight)
            {
                dirtRotation.y = 90f;
            }

            Instantiate(dirt, groundCheck.position, dirtRotation);
        }
    }

    /// <summary>
    /// Flip character's sprite according to the direction he's moving
    /// </summary>
    private void Flip()
    {
        _facingRight = !_facingRight;
        _spriteRenderer.flipX = _facingRight;
    }

    /// <summary>
    /// Player's jump
    /// </summary>
    private void Jump()
    {
        if (_canJump)
        {
            _canJump = false;
            _isJumping = true;
            _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);

            // Play sound
            _audioSource.volume = 0.3f;
            _audioSource.pitch = 1f;
            _audioSource.PlayOneShot(jumpAudioClip);
        }
    }

    /// <summary>
    /// Player's jump with variable height
    /// </summary>
    private void VariableJump()
    {
        // The factor is applied only when the player is not falling
        if (_rigidbody.velocity.y >= 0f && _isJumping)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x,
                                              _rigidbody.velocity.y * _jumpHeightFactor);
        }
    }

    /// <summary>
    /// Player takes damage
    /// </summary>
    public void TakeDamage()
    {
        // Play sound
        _audioSource.volume = 0.4f;
        _audioSource.pitch = 1f;
        _audioSource.PlayOneShot(takeDamageAudioClip);
    }

    /// <summary>
    /// Check if player is grounded
    /// </summary>
    private void GroundCheck()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, _groundCheckRadius,
                                              _groundLayerMask);

        if (_isGrounded && _rigidbody.velocity.y <= 0f)
        {
            _isJumping = false;
        }

        if (_isGrounded && !_isJumping && _slopeDownAngle <= _maxSlopeAngle)
        {
            _canJump = true;
        }   
    }

    /// <summary>
    /// Check if player is on a slope
    /// </summary>
    private void SlopeCheck()
    {
        Vector2 checkPosition = groundCheck.position;

        SlopeCheckVertical(checkPosition);
        SlopeCheckHorizontal(checkPosition);

        // Determine if it's possible to walk on a slope depending on its angle
        if (_slopeDownAngle > _maxSlopeAngle || _slopeSideAngle > _maxSlopeAngle)
        {
            _canWalkOnSlope = false;
        }
        else
        {
            _canWalkOnSlope = true;
        }

        // Prevent the player from moving when idling on a slope by changing friction behaviour
        if (_isOnSlope && _canWalkOnSlope && _horizontalInput == 0f)
        {
            _rigidbody.sharedMaterial = _fullFriction;
        }
        else
        {
            _rigidbody.sharedMaterial = _zeroFriction;
        }
    }

    /// <summary>
    /// Look for slopes horizontally
    /// </summary>
    /// <param name="checkPosition"> Origin position to check slopes from </param>
    private void SlopeCheckHorizontal(Vector2 checkPosition)
    {
        RaycastHit2D hitFront = Physics2D.Raycast(checkPosition, transform.right,
                                                  _slopeCheckDistance, _groundLayerMask);

        RaycastHit2D hitBack = Physics2D.Raycast(checkPosition, -transform.right,
                                                 _slopeCheckDistance, _groundLayerMask);

        if (hitFront)
        {
            _isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(hitFront.normal, Vector2.up);
        }
        else if (hitBack)
        {
            _isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(hitBack.normal, Vector2.up);
        }
        else
        {
            _isOnSlope = false;
            _slopeSideAngle = 0f;
        }
    }

    /// <summary>
    /// Look for slopes vertically
    /// </summary>
    /// <param name="checkPosition"> Origin position to check slopes from </param>
    private void SlopeCheckVertical(Vector2 checkPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.down,
                                             _slopeCheckDistance, _groundLayerMask);

        if (hit)
        {
            _slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;
            _slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (_slopeDownAngle != _lastSlopeDownAngle)
            {
                _isOnSlope = true;
            }

            _lastSlopeDownAngle = _slopeDownAngle;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Attributes
    public int _numberOfLives = 3;

    // Movement
    [Header("Movement")]
    [SerializeField]
    private float _movementSpeed;
    private float _horizontalInput;
    private float _facingDirection;
    private bool _facingRight;

    // Jump
    [Header("Jump")]
    [SerializeField]
    private float _jumpForce;
    private bool _isJumping;
    private bool _canJump;
    [SerializeField]
    private float _jumpHeightFactor;
    [SerializeField]
    private float _movementForceInAir;
    [SerializeField]
    private float _airDragMultiplier;
    private float _timeInAir;
    [SerializeField]
    private float _minTimeInAir;

    // Ground check
    [Header("Ground check")]
    public Transform groundCheckTransform;
    [SerializeField]
    private float _groundCheckRadius;
    private bool _isGrounded;
    private LayerMask _groundLayerMask;

    // Slope check
    [Header("Slope check")]
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

    // Special abilities
    private bool _unlockedDoubleJump;
    private bool _unlockedWallSliding;

    // Double jump
    private bool _canDoubleJump;
    private bool _isDoubleJumping;

    // Wall sliding
    [Header("Wall sliding")]
    public Transform wallCheckTransform;
    [SerializeField]
    private float _wallCheckDistance;
    [SerializeField]
    private float _wallSlideSpeed;
    private bool _isTouchingWall;
    private bool _isWallSliding;

    // Wall jump
    [Header("Wall jump")]
    private Vector2 _wallHopDirection;
    private Vector2 _wallJumpDirection;
    [SerializeField]
    private float _wallHopForce;
    [SerializeField]
    private float _wallJumpForce;

    // Materials
    [Header("Materials")]
    [SerializeField]
    private PhysicsMaterial2D _zeroFriction;
    [SerializeField]
    private PhysicsMaterial2D _fullFriction;

    // Audio
    [Header("Audio")]
    public AudioClip footstepsAudioClip;
    public AudioClip jumpAudioClip;
    public AudioClip fartAudioClip;
    public AudioClip takeDamageAudioClip;

    // Effects
    [Header("Particle Systems")]
    public ParticleSystem dirtParticleSystem;
    public ParticleSystem fartParticleSystem;
    public GameObject dust;

    // Components
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        // Components
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        // Ground layer
        _groundLayerMask = LayerMask.GetMask("Ground");

        // Player is facing right
        _facingRight = true;
        _facingDirection = 1f;

        // Wall jump
        _wallHopDirection = new Vector2(1f, 0.5f);
        _wallHopDirection.Normalize();
        _wallJumpDirection = new Vector2(1f, 2f);
        _wallJumpDirection.Normalize();
    }

    // Update is called once per frame
    private void Update()
    {
        // Listen to player's input
        CheckInput();

        // Feed animator controller's parameters with updated values
        UpdateAnimatorParameters();
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        // Check if player is on ground, on a slope or touching a wall for wall sliding/jumping
        GroundCheck();
        SlopeCheck();

        if (_unlockedWallSliding)
        {
            WallCheck();
            WallSlidingCheck();
        }

        // Check when player has landed after a jump and the time spent on air
        CheckLanding();
        CheckTimeInAir();

        // Move player
        Move();
    }

    /// <summary>
    /// Check keyboard input
    /// </summary>
    private void CheckInput()
    {
        // Movement
        _horizontalInput = Input.GetAxis("Horizontal");

        // Jump
        if ((_isGrounded || _isTouchingWall) && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        else if (_unlockedDoubleJump && Input.GetButtonDown("Jump"))
        {
            DoubleJump();
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
    private void UpdateAnimatorParameters()
    {
        _animator.SetFloat(Constants.SPEED_F, Mathf.Abs(_horizontalInput));
        _animator.SetFloat(Constants.VERTICALSPEED_F, _rigidbody.velocity.y);
        _animator.SetBool(Constants.ISGROUNDED_B, _isGrounded);
        _animator.SetBool(Constants.ISJUMPING_B, _isJumping);
        _animator.SetBool(Constants.ISDOUBLEJUMPING_B, _isDoubleJumping);
        _animator.SetBool(Constants.ISWALLSLIDING_B, _isWallSliding);
    }

    /// <summary>
    /// Apply player's movement
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
            // Normal movement
            _rigidbody.velocity = new Vector2(_movementSpeed * _horizontalInput, 
                                              _rigidbody.velocity.y);
        }
        else if (_isGrounded && _isOnSlope && _canWalkOnSlope && !_isJumping)
        {
            // Movement if on a slope
            _rigidbody.velocity = new Vector2(_movementSpeed * _slopeNormalPerpendicular.x *
                                              -_horizontalInput,
                                              _movementSpeed * _slopeNormalPerpendicular.y *
                                              -_horizontalInput);
        }
        else if (!_isGrounded && !_isWallSliding && _horizontalInput != 0f)
        {
            // Add a force when moving in mid-air
            _rigidbody.AddForce(new Vector2(_movementForceInAir * _horizontalInput, 0f));

            if (Mathf.Abs(_rigidbody.velocity.x) > _movementSpeed)
            {
                _rigidbody.velocity = new Vector2(_movementSpeed * _horizontalInput,
                                                  _rigidbody.velocity.y);
            }
        }
        else if (!_isGrounded && !_isWallSliding && _horizontalInput == 0f)
        {
            // Add a drag when not moving in mid-air
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x * _airDragMultiplier,
                                              _rigidbody.velocity.y);
        }

        // Play sound and particles when player is moving
        if (_isGrounded && _horizontalInput != 0f)
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = footstepsAudioClip;
                _audioSource.volume = 0.1f;
                _audioSource.pitch = 1.4f;
                _audioSource.Play();
            }

            dirtParticleSystem.Play();
        }

        // Wall sliding
        if (_isWallSliding)
        {
            if (_rigidbody.velocity.y < -_wallSlideSpeed)
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -_wallSlideSpeed);
            }
        }
    }

    /// <summary>
    /// Flip character's sprite according to the direction he's moving
    /// </summary>
    private void Flip()
    {
        if (!_isWallSliding)
        {
            _facingDirection *= -1f;
            _facingRight = !_facingRight;
            _spriteRenderer.flipX = _facingRight;
        }
    }

    /// <summary>
    /// Player's jump
    /// </summary>
    private void Jump()
    {
        if (_canJump)
        {
            // Normal jump
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);

            _canJump = false;
            _isJumping = true;
            _canDoubleJump = true;

            // Play sound
            _audioSource.volume = 0.3f;
            _audioSource.pitch = 1f;
            _audioSource.PlayOneShot(jumpAudioClip);
        }
        else if (_canJump && _isWallSliding && _horizontalInput == 0)
        {
            // Wall hop
            Vector2 forceToAdd = new Vector2(_wallHopForce * _wallHopDirection.x *
                                             -_facingDirection,
                                             _wallHopForce * _wallHopDirection.y);
            _rigidbody.AddForce(forceToAdd, ForceMode2D.Impulse);

            _isWallSliding = false;
            _canJump = false;
            _isJumping = true;
            _canDoubleJump = true;
        }
        else if (_canJump && (_isWallSliding || _isTouchingWall) && _horizontalInput != 0)
        {
            // Wall jump
            Vector2 forceToAdd = new Vector2(_wallJumpForce * _wallJumpDirection.x *
                                             _horizontalInput,
                                             _wallJumpForce * _wallJumpDirection.y);
            _rigidbody.AddForce(forceToAdd, ForceMode2D.Impulse);

            _isWallSliding = false;
            _canJump = false;
            _isJumping = true;
            _canDoubleJump = true;
        }
    }

    /// <summary>
    /// Player's double jump
    /// </summary>
    private void DoubleJump()
    {
        if (_isJumping && _canDoubleJump)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);

            _isJumping = false;
            _isDoubleJumping = true;
            _canDoubleJump = false;

            // Play sound
            _audioSource.volume = 0.3f;
            _audioSource.pitch = 1.7f;
            _audioSource.PlayOneShot(fartAudioClip);

            // Play particles
            ParticleSystem.MainModule main = fartParticleSystem.main;

            if (_facingRight)
            {
                main.flipRotation = 0f;
            }
            else if (!_facingRight)
            {
                main.flipRotation = 1f;
            }

            fartParticleSystem.Play();
        }
    }

    /// <summary>
    /// Player's jump with variable height
    /// </summary>
    private void VariableJump()
    {
        // The factor is applied only when the player is jumping
        if ((_isJumping || _isDoubleJumping) && _rigidbody.velocity.y >= 0f)
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
        // Decrease number of lives
        _numberOfLives--;

        // Send number of lives to the GameManager
        GameManager.Instance.CheckNumberOfLives(_numberOfLives);

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
        _isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, _groundCheckRadius,
                                              _groundLayerMask);

        // Player is on ground
        if (_isGrounded && _rigidbody.velocity.y <= 0f)
        {
            _isJumping = false;
            _isDoubleJumping = false;
        }
        {
            dirtParticleSystem.Stop();
        }

        // Player can jump
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
        Vector2 checkPosition = groundCheckTransform.position;

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

    /// <summary>
    /// Check if player is touching a wall
    /// </summary>
    private void WallCheck()
    {
        _isTouchingWall = Physics2D.Raycast(wallCheckTransform.position, 
                                            transform.right * _facingDirection,
                                            _wallCheckDistance, _groundLayerMask);
    }

    /// <summary>
    /// Check if player is wall sliding
    /// </summary>
    private void WallSlidingCheck()
    {
        if (_isTouchingWall && !_isGrounded && _rigidbody.velocity.y < 0f)
        {
            _isWallSliding = true;
            _isJumping = false;
            _isDoubleJumping = false;
            _canJump = true;
            _canDoubleJump = false;
        }
        else
        {
            _isWallSliding = false;
        }
    }

    /// <summary>
    /// Calculate the time the player spent in air
    /// </summary>
    private void CheckTimeInAir()
    {
        if (_isGrounded)
        {
            _timeInAir = 0f;
        }
        else
        {
            _timeInAir += Time.deltaTime;
        }
    }

    /// <summary>
    /// Check if the player is grounded after being in air
    /// </summary>
    private void CheckLanding()
    {
        if (_timeInAir > _minTimeInAir)
        {
            if (_isGrounded)
            {
                Instantiate(dust, groundCheckTransform.position, dust.transform.rotation);
            }
        }
    }
}
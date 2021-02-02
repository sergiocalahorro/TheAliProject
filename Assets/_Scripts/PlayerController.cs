using UnityEngine;

//TODO: VARIABLE JUMP HEIGHT
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Components
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    // Player movement
    [SerializeField, Range(1, 10)]
    private float _movementSpeed;
    private float _horizontalInput;
    private bool _facingRight = true;
    [SerializeField, Range(1, 20)]
    private float _jumpForce;
    private bool _canJump;
    private bool _isJumping;

    // Ground check
    private LayerMask _groundLayerMask;
    private float _groundCheckRadius = 0.2f;
    private bool _isGrounded;
    public Transform groundCheck;

    // Slope check
    [SerializeField]
    private float _slopeCheckDistance = 0.5f;
    private float _slopeDownAngle;
    private float _slopeSideAngle;
    private float _lastSlopeDownAngle;
    private float _maxSlopeAngle = 90f;
    public bool _isOnSlope;
    private bool _canWalkOnSlope;
    private Vector2 _slopeNormalPerpendicular;

    // Physics
    [SerializeField]
    private PhysicsMaterial2D _zeroFriction;
    [SerializeField]
    private PhysicsMaterial2D _fullFriction;

    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _groundLayerMask = LayerMask.GetMask("Ground");
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

        // Flip character
        if (_horizontalInput > 0f && !_facingRight)
        {
            Flip();
        }
        else if (_horizontalInput < 0f && _facingRight)
        {
            Flip();
        }

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    /// <summary>
    /// Player's movement
    /// </summary>
    private void Move()
    {
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
            _rigidbody.velocity = new Vector2(_movementSpeed * _horizontalInput,
                                              _rigidbody.velocity.y);
        }
    }

    /// <summary>
    /// Flip character's sprite according to the direction he's moving
    /// </summary>
    private void Flip()
    {
        _spriteRenderer.flipX = _facingRight;
        _facingRight = !_facingRight;
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
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
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
}
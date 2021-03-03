using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    // Player reference
    private PlayerController _player;

    // Ground check
    [Header("Ground check")]
    public float groundCheckRadius;
    public Transform groundTransform;
    public LayerMask groundLayerMask;

    // Slope check
    [Header("Slope check")]
    [SerializeField]
    private float _slopeCheckDistance;
    [SerializeField]
    private float _maxSlopeAngle;
    private float _slopeDownAngle;
    private float _slopeSideAngle;
    private float _lastSlopeDownAngle;
    private Vector2 _slopeNormalPerpendicular;
    public Vector2 slopeNormalPerpendicular
    {
        get
        {
            return _slopeNormalPerpendicular;
        }
    }

    // Physics
    [Header("Physics materials")]
    public PhysicsMaterial2D zeroFriction;
    public PhysicsMaterial2D fullFriction;

    // Start is called before the first frame update
    private void Start()
    {
        _player = GetComponent<PlayerController>();
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        // Check if player is on ground
        CheckIfGrounded();

        // Check if player is on a slope
        CheckIfOnSlope();
    }

    /// <summary>
    /// Check if player is grounded
    /// </summary>
    private void CheckIfGrounded()
    {
        _player.isGrounded = Physics2D.OverlapCircle(groundTransform.position,
                                                     groundCheckRadius,
                                                     groundLayerMask);

        // Player is on ground
        if (_player.isGrounded && _player.rb2D.velocity.y <= 0f)
        {
            _player.isJumping = false;
            _player.isDoubleJumping = false;
        }
        {
            _player.dirtParticleSystem.Stop();
        }

        // Player can jump
        if (_player.isGrounded && !_player.isJumping && _slopeDownAngle <= _maxSlopeAngle)
        {
            _player.canJump = true;
        }
    }

    /// <summary>
    /// Check if player is on a slope
    /// </summary>
    private void CheckIfOnSlope()
    {
        Vector2 checkPosition = groundTransform.position;

        SlopeCheckVertical(checkPosition);
        SlopeCheckHorizontal(checkPosition);

        // Determine if it's possible to walk on a slope depending on its angle
        if (_slopeDownAngle > _maxSlopeAngle || _slopeSideAngle > _maxSlopeAngle)
        {
            _player.canWalkOnSlope = false;
        }
        else
        {
            _player.canWalkOnSlope = true;
        }

        // Stop player movement when not movingon a slope by changing friction behaviour
        if (_player.isOnSlope && _player.canWalkOnSlope && _player.movementAmount == 0f)
        {
            _player.rb2D.sharedMaterial = fullFriction;
        }
        else
        {
            _player.rb2D.sharedMaterial = zeroFriction;
        }
    }

    /// <summary>
    /// Look for slopes horizontally
    /// </summary>
    /// <param name="checkPosition"> Origin position to check slopes from </param>
    private void SlopeCheckHorizontal(Vector2 checkPosition)
    {
        RaycastHit2D hitFront = Physics2D.Raycast(checkPosition, transform.right,
                                                  _slopeCheckDistance, groundLayerMask);

        RaycastHit2D hitBack = Physics2D.Raycast(checkPosition, -transform.right,
                                                 _slopeCheckDistance, groundLayerMask);

        if (hitFront)
        {
            _player.isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(hitFront.normal, Vector2.up);
        }
        else if (hitBack)
        {
            _player.isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(hitBack.normal, Vector2.up);
        }
        else
        {
            _player.isOnSlope = false;
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
                                             _slopeCheckDistance, groundLayerMask);

        if (hit)
        {
            _slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;
            _slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (_slopeDownAngle != _lastSlopeDownAngle)
            {
                _player.isOnSlope = true;
            }

            _lastSlopeDownAngle = _slopeDownAngle;
        }
    }
}
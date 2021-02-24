using UnityEngine;

public class WallCheck : MonoBehaviour
{
    // Player reference
    private PlayerController _player;

    // Wall sliding
    [Header("Wall sliding")]
    public Transform wallCheckTransform;
    public float wallCheckDistance;
    public float wallSlideSpeed;

    // Wall jump
    [Header("Wall jump")]
    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;
    public float wallHopForce;
    public float wallJumpForce;

    // Start is called before the first frame update
    private void Start()
    {
        _player = GetComponent<PlayerController>();

        // Wall jump
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        if (_player.unlockedWallSliding)
        {
            CheckIfTouchingWall();
            CheckIfWallSliding();
        }
    }

    /// <summary>
    /// Check if player is touching a wall
    /// </summary>
    private void CheckIfTouchingWall()
    {
        // Change direction of the raycast depending on the direction the player is facing
        if (_player.facingRight)
        {
            _player.isTouchingWall = Physics2D.Raycast(wallCheckTransform.position,
                                                       -transform.right * _player.facingDirection,
                                                       wallCheckDistance, 
                                                       _player.groundCheck.groundLayerMask);
        }
        else
        {
            _player.isTouchingWall = Physics2D.Raycast(wallCheckTransform.position,
                                                       transform.right * _player.facingDirection,
                                                       wallCheckDistance, 
                                                       _player.groundCheck.groundLayerMask);
        }
    }

    /// <summary>
    /// Check if player is wall sliding
    /// </summary>
    private void CheckIfWallSliding()
    {
        // Player is wall sliding if is touching a wall while in air and losing vertical velocity
        if (_player.isTouchingWall && !_player.isGrounded && 
            _player.rigidbodyPlayer.velocity.y < 0f)
        {
            _player.isWallSliding = true;
            _player.isJumping = false;
            _player.isDoubleJumping = false;
            _player.canJump = true;
            _player.canDoubleJump = false;
        }
        else
        {
            _player.isWallSliding = false;
        }
    }
}
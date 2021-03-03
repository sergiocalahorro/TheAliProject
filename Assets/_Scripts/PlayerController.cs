using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Attributes
    [Header("Attributes")]
    public int healthPoints;
    public int numberOfCoins;
    [SerializeField]
    private float _hurtDuration;
    [SerializeField]
    private float _invulnerabilityDuration;
    private bool _canTakeDamage;
    private bool _isHurt;
    public bool isHurt
    {
        get
        {
            return _isHurt;
        }
    }
    private bool _isDead;
    public bool isDead
    {
        get
        {
            return _isDead;
        }
    }

    // Movement
    [Header("Movement")]
    [SerializeField]
    private float _movementSpeed;
    private float _facingDirection;
    public float facingDirection
    {
        get
        {
            return _facingDirection;
        }
    }
    private bool _facingRight;
    public bool facingRight
    {
        get
        {
            return _facingRight;
        }
    }
    [System.NonSerialized]
    public float movementAmount;
    [System.NonSerialized]
    public bool canMove;

    // Jump
    [Header("Jump")]
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private float _jumpHeightFactor;
    [SerializeField]
    private float _movementForceInAir;
    [SerializeField]
    private float _airDragMultiplier;
    [SerializeField]
    private float _minTimeInAir;
    private float _timeInAir;
    [System.NonSerialized]
    public bool canJump;
    [System.NonSerialized]
    public bool isJumping;

    // Weapon
    [Header("Weapon")]
    [SerializeField]
    private float _shotDelay;
    private Weapon _sock;
    private bool _isShooting;
    public bool isShooting
    {
        get
        {
            return _isShooting;
        }
    }

    // Ground check
    [System.NonSerialized]
    public bool isGrounded;
    [System.NonSerialized]
    public bool isOnSlope;
    [System.NonSerialized]
    public bool canWalkOnSlope;
    private GroundCheck _groundCheck;
    public GroundCheck groundCheck
    {
        get
        {
            return _groundCheck;
        }
    }

    // Double jump
    [Header("Double jump")]
    public bool unlockedDoubleJump;
    [System.NonSerialized]
    public bool canDoubleJump;
    [System.NonSerialized]
    public bool isDoubleJumping;

    // Wall sliding
    [Header("Wall sliding")]
    public bool unlockedWallSliding;
    [System.NonSerialized]
    public bool isTouchingWall;
    [System.NonSerialized]
    public bool isWallSliding;
    private WallCheck _wallCheck;

    // Audio
    [Header("Audio")]
    [SerializeField]
    private AudioClip _footstepsAudioClip;
    [SerializeField]
    private AudioClip _jumpAudioClip;
    [SerializeField]
    private AudioClip _fartAudioClip;
    [SerializeField]
    private AudioClip[] _takeDamageAudioClips;
    [SerializeField]
    private AudioClip[] _deathAudioClips;
    private AudioSource _audioSource;

    // Particle Systems
    [Header("Particle systems")]
    public ParticleSystem dirtParticleSystem;
    public ParticleSystem fartParticleSystem;
    public GameObject dust;

    // Animations
    private Animator _animator;
    private bool _deadAnimationPlayed;
    public bool deadAnimationPlayed
    {
        get
        {
            return _deadAnimationPlayed;
        }
    }
    [System.NonSerialized]
    public PlayerAnimationState currentState;

    // Physics
    [System.NonSerialized]
    public Rigidbody2D rb2D;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Components
        _sock = GetComponent<Weapon>();
        rb2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        // Player is facing right
        _facingRight = true;
        _facingDirection = 1f;
    }

    // OnEnable is called when the object becomes enabled and active
    private void OnEnable()
    {
        // Player is alive and can take damage
        canMove = true;
        _isDead = false;
        _isHurt = false;
        _canTakeDamage = true;
        _deadAnimationPlayed = false;

        // Set player facing right
        if (!_facingRight)
        {
            Flip();
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        _groundCheck = GetComponent<GroundCheck>();
        _wallCheck = GetComponent<WallCheck>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Update Animator
        UpdateAnimatorState();

        // Prevent player from moving when shooting
        if (_isShooting)
        {
            movementAmount = 0f;
            dirtParticleSystem.Stop();
            canMove = false;
        }
        else
        {
            canMove = true;
        }
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        // Player's movement
        Move();

        // Check when player has landed after a jump and the time spent on air
        CheckLanding();
        CheckTimeInAir();
    }

    /// <summary>
    /// Change the animation that is currently playing
    /// </summary>
    /// <param name="newState"> New animation state to be played </param>
    private void ChangeAnimationState(PlayerAnimationState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        _animator.SetInteger(Constants.PLAYER_STATE, (int)newState);
        currentState = newState;
    }

    /// <summary>
    /// Update Animator's state with the next animation to be played
    /// </summary>
    private void UpdateAnimatorState()
    {
        if (!_isHurt && !_isDead)
        {
            if (isGrounded && movementAmount == 0f)
            {
                // Player is idle
                ChangeAnimationState(PlayerAnimationState.Idle);
            }

            if (isGrounded && movementAmount != 0f)
            {
                // Player is walking
                ChangeAnimationState(PlayerAnimationState.Walk);
            }

            if (isJumping && rb2D.velocity.y > 0f)
            {
                // Player is jumping
                ChangeAnimationState(PlayerAnimationState.Jump);
            }

            if (!isWallSliding && !isGrounded && rb2D.velocity.y < 0f)
            {
                // Player is falling
                ChangeAnimationState(PlayerAnimationState.Fall);
            }

            if (isGrounded && _isShooting)
            {
                // Player is throwing sock
                ChangeAnimationState(PlayerAnimationState.Throw);
            }

            if (isDoubleJumping && rb2D.velocity.y > 0f)
            {
                // Player is double jumping
                ChangeAnimationState(PlayerAnimationState.DoubleJump);
            }

            if (isWallSliding)
            {
                // Player is wall sliding
                ChangeAnimationState(PlayerAnimationState.WallSlide);
            }
        }

        if (_isHurt)
        {
            // Player is hurt
            ChangeAnimationState(PlayerAnimationState.Hurt);
        }

        if (_isDead)
        {
            // Player is dead
            ChangeAnimationState(PlayerAnimationState.Dead);
        }
    }

    /// <summary>
    /// Apply player's movement
    /// </summary>
    private void Move()
    {
        // Flip character depending on his movement direction
        if (movementAmount > 0f && !_facingRight)
        {
            Flip();
        }
        else if (movementAmount < 0f && _facingRight)
        {
            Flip();
        }

        if (isGrounded && !isOnSlope && !isJumping)
        {
            // Normal movement
            rb2D.velocity = new Vector2(_movementSpeed * movementAmount,
                                                   rb2D.velocity.y);
        }
        else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping)
        {
            // Movement if on a slope
            rb2D.velocity = new Vector2(_movementSpeed * 
                                                   _groundCheck.slopeNormalPerpendicular.x *
                                                   -movementAmount,
                                                   _movementSpeed * 
                                                   _groundCheck.slopeNormalPerpendicular.y *
                                                   -movementAmount);
        }
        else if (!isGrounded && !isWallSliding && movementAmount != 0f)
        {
            // Add a force when moving in mid-air
            rb2D.AddForce(new Vector2(_movementForceInAir * movementAmount, 0f));

            if (Mathf.Abs(rb2D.velocity.x) > _movementSpeed)
            {
                rb2D.velocity = new Vector2(_movementSpeed * movementAmount,
                                                       rb2D.velocity.y);
            }
        }
        else if (!isGrounded && !isWallSliding && movementAmount == 0f)
        {
            // Add a drag when not moving in mid-air
            rb2D.velocity = new Vector2(rb2D.velocity.x *
                                                   _airDragMultiplier,
                                                   rb2D.velocity.y);
        }

        // Play sound and particles when player is moving
        if (isGrounded && movementAmount != 0f && !_isHurt)
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = _footstepsAudioClip;
                _audioSource.volume = 0.1f;
                _audioSource.pitch = 1.4f;
                _audioSource.Play();
            }

            dirtParticleSystem.Play();
        }

        // Wall sliding
        if (isWallSliding)
        {
            if (rb2D.velocity.y < -_wallCheck.wallSlideSpeed)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x,
                                                       -_wallCheck.wallSlideSpeed);
            }
        }
    }

    /// <summary>
    /// Flip character's sprite according to the direction he's moving
    /// </summary>
    private void Flip()
    {
        if (!isWallSliding)
        {
            _facingDirection *= -1f;
            _facingRight = !_facingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    /// <summary>
    /// Player throws sock
    /// </summary>
    /// <returns> Time until next shot is availabe </returns>
    public IEnumerator ThrowSock()
    {
        _sock.Shoot();
        _isShooting = true;
        yield return new WaitForSeconds(_shotDelay);
        _isShooting = false;
    }

    /// <summary>
    /// Player's jump
    /// </summary>
    public void Jump()
    {
        if (canJump)
        {
            // Normal jump
            rb2D.velocity = new Vector2(rb2D.velocity.x, 0f);
            rb2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);

            canJump = false;
            isJumping = true;
            canDoubleJump = true;

            // Play sound
            _audioSource.volume = 0.3f;
            _audioSource.pitch = 1f;
            _audioSource.PlayOneShot(_jumpAudioClip);
        }
        else if (canJump && isWallSliding && movementAmount == 0)
        {
            // Wall hop
            Vector2 forceToAdd = new Vector2(_wallCheck.wallHopForce * 
                                             _wallCheck.wallHopDirection.x *
                                             -_facingDirection,
                                             _wallCheck.wallHopForce * 
                                             _wallCheck.wallHopDirection.y);
            rb2D.AddForce(forceToAdd, ForceMode2D.Impulse);

            isWallSliding = false;
            canJump = false;
            isJumping = true;
            canDoubleJump = true;
        }
        else if (canJump && (isWallSliding || isTouchingWall) && movementAmount != 0)
        {
            // Wall jump
            Vector2 forceToAdd = new Vector2(_wallCheck.wallJumpForce * 
                                             _wallCheck.wallJumpDirection.x *
                                             movementAmount,
                                             _wallCheck.wallJumpForce * 
                                             _wallCheck.wallJumpDirection.y);
            rb2D.AddForce(forceToAdd, ForceMode2D.Impulse);

            isWallSliding = false;
            canJump = false;
            isJumping = true;
            canDoubleJump = true;
        }
    }

    /// <summary>
    /// Player's jump with variable height
    /// </summary>
    public void VariableJump()
    {
        // The factor is applied only when the player is jumping
        if ((isJumping || isDoubleJumping) && rb2D.velocity.y >= 0f)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x,
                                                   rb2D.velocity.y * 
                                                   _jumpHeightFactor);
        }
    }

    /// <summary>
    /// Player's double jump
    /// </summary>
    public void DoubleJump()
    {
        if (isJumping && canDoubleJump)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, 0f);
            rb2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);

            isJumping = false;
            isDoubleJumping = true;
            canDoubleJump = false;

            // Play sound
            _audioSource.volume = 0.3f;
            _audioSource.pitch = 1.7f;
            _audioSource.PlayOneShot(_fartAudioClip);

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
    /// Calculate the time the player spent in air
    /// </summary>
    private void CheckTimeInAir()
    {
        if (isGrounded)
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
            if (isGrounded)
            {
                Instantiate(dust, _groundCheck.groundTransform.position, dust.transform.rotation);
            }
        }
    }

    /// <summary>
    /// Player takes damage
    /// </summary>
    public IEnumerator TakeDamage(int damageAmount)
    {
        if (_canTakeDamage)
        {
            // Decrease number of lives
            healthPoints -= damageAmount;

            // Player is hurt and becomes invulnerable for some time
            _isHurt = true;
            _canTakeDamage = false;

            if (healthPoints > 0)
            {
                // Play random sound
                int randomIndex = Random.Range(0, _takeDamageAudioClips.Length);
                _audioSource.clip = _takeDamageAudioClips[randomIndex];
                _audioSource.volume = 0.4f;
                _audioSource.pitch = 1f;
                _audioSource.Play();

                // Player stops being hurt but is still invulnerable
                yield return new WaitForSeconds(_hurtDuration);
                _isHurt = false;
            }
            else
            {
                // Player is dead
                canMove = false;
                _isDead = true;

                // Play sound
                int randomIndex = Random.Range(0, _deathAudioClips.Length);
                _audioSource.clip = _deathAudioClips[randomIndex];
                _audioSource.volume = 0.4f;
                _audioSource.pitch = 1f;
                _audioSource.Play();

                // Wait until the sound finished playing
                yield return new WaitForSeconds(_audioSource.clip.length);
                _deadAnimationPlayed = true;
            }
        }

        // Player stops being invulnerable after some time
        yield return new WaitForSeconds(_hurtDuration + _invulnerabilityDuration);
        if (!_isDead)
        {
            _canTakeDamage = true;
        }
    }

    /// <summary>
    /// Add knock back force
    /// </summary>
    public void KnockBack(Vector2 direction, float powerX, float powerY)
    {
        if (_canTakeDamage && !_isDead)
        {
            rb2D.velocity = Vector2.zero;
            rb2D.inertia = 0;
            rb2D.AddForce(new Vector2(direction.x * powerX, direction.y * powerY), 
                          ForceMode2D.Impulse);
        }
    }
}
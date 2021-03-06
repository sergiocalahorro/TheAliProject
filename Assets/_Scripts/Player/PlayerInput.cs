using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Player reference
    private PlayerController _player;

    // Start is called before the first frame update
    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_player.isDead)
        {
            // Listen to player's input
            if (_player.canMove)
            {
                CheckInput();
            }
        }
    }

    /// <summary>
    /// Listen to player's input
    /// </summary>
    private void CheckInput()
    {
        // Movement
        _player.movementAmount = Input.GetAxis("Horizontal");

        // Jump
        if ((_player.isGrounded || _player.isTouchingWall) && !_player.isHurt &&
            Input.GetButtonDown("Jump"))
        {
            _player.Jump();
        }
        else if (_player.unlockedDoubleJump && Input.GetButtonDown("Jump"))
        {
            _player.DoubleJump();
        }

        // Stop jump at a variable height
        if (Input.GetButtonUp("Jump"))
        {
            _player.VariableJump();
        }

        // Melee attack
        if (_player.isGrounded && !_player.isAttacking && Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Attack");
            StartCoroutine(_player.Attack());
        }

        // Throw sock
        if (_player.isGrounded && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(_player.ThrowSock());
        }

        // Pause/resume game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.Instance.paused)
            {
                GameManager.Instance.Pause();
            }
            else
            {
                GameManager.Instance.Resume();
            } 
        }
    }
}
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerController _player;

    // Start is called before the first frame update
    private void Start()
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

            // Prevent player from moving when shooting
            if (_player.isShooting)
            {
                _player.movementAmount = 0f;
                _player.dirtParticleSystem.Stop();
                _player.canMove = false;
            }
            else
            {
                _player.canMove = true;
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

        // Throw sock
        if (_player.isGrounded && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(_player.ThrowSock());
        }

        // Pause game
        if (!GameManager.Instance.paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.Pause();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.Resume();
            }
        }
    }
}
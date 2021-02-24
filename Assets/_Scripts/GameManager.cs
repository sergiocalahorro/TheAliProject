using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioManager), typeof(GUIManager))]
public class GameManager : Singleton<GameManager>
{
    // Control
    private PlayerController _player;
    private List<GameObject> _totalCoins;
    public List<GameObject> totalCoins
    {
        get
        {
            return _totalCoins;
        }
    }
    private CheckPointController _checkPoint;
    public CheckPointController checkPoint
    {
        set
        {
            _checkPoint = value;
        }
    }
    private bool _isGameOver;
    public bool isGameOver
    {
        get
        {
            return _isGameOver;
        }
    }
    private bool _insideGameOver;
    public bool insideGameOver
    {
        get
        {
            return _insideGameOver;
        }
    }
    private bool _paused;
    public bool paused
    {
        get
        {
            return _paused;
        }
    }

    // Components
    private AudioManager _audioManager;
    private GUIManager _guiManager;
    private CameraManager _cameraManager;

    // Start is called before the first frame update
    private void Start()
    {
        // Components
        _audioManager = GetComponent<AudioManager>();
        _guiManager = GetComponent<GUIManager>();
        _cameraManager = GetComponent<CameraManager>();

        // Start playing background music
        StartCoroutine(_audioManager.PlayBackgroundMusic());

        // Control
        _player = FindObjectOfType<PlayerController>();
        _totalCoins = new List<GameObject>();
        _isGameOver = false;
        _insideGameOver = false;

        // Set player's starting position
        Vector3 startPosition = new Vector3(-14f, -5.2f, 0f);
        Spawn(startPosition);
    }

    // Update is called once per frame
    private void Update()
    {
        // Shake camera when player is hurt
        if (_player.isHurt)
        {
            // Avoid negative number
            if (_player.numberOfLives < 0)
            {
                _player.numberOfLives = 0;
            }

            UpdateNumberOfLives(_player.numberOfLives);
            _cameraManager.Shake();
        }
        else
        {
            _cameraManager.ResetShake();
        }

        // Zoom in on player when he's dead
        if (_player.isDead)
        {
            _cameraManager.ZoomIn();
        }
        else
        {
            _cameraManager.ResetZoomIn();
        }

        // Game is over
        if (_player.deadAnimationPlayed)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Add coin
    /// </summary>
    public void CoinPickUp(GameObject coin)
    {
        // Add coin
        totalCoins.Add(coin);

        // Update UI
        _guiManager.DisplayCoins(totalCoins.Count);
    }

    /// <summary>
    /// Update the number of lives the player has
    /// </summary>
    /// <param name="numberOfLives"> Number of lives the player has </param>
    private void UpdateNumberOfLives(int numberOfLives)
    {
        // Update UI
        _guiManager.DisplayCurrentLivesImages(numberOfLives);
    }

    /// <summary>
    /// Game Over
    /// </summary>
    private void GameOver()
    {
        if (!_insideGameOver)
        {
            // Player is dead
            _isGameOver = true;
            _player.enabled = false;

            // Play music
            _audioManager.PlayGameOverMusic();

            // Game state is in game over
            _insideGameOver = true;
        }
    }

    /// <summary>
    /// Spawn player at a given position
    /// </summary>
    private void Spawn(Vector3 respawnPosition)
    {
        // Hide mouse cursor
        Cursor.visible = false;

        // Player spawns at given position
        _player.transform.position = respawnPosition;

        // Player has 3 lives
        _player.numberOfLives = 3;
    }

    /// <summary>
    /// Restart game from last check point reached
    /// </summary>
    public void RestartFromCheckPoint()
    {
        // Game control
        _isGameOver = false;
        _insideGameOver = false;
        _player.enabled = true;
        Spawn(_checkPoint.transform.position);

        // Compare the total coins with the ones collected on the last check point reached
        for (int i = totalCoins.Count - 1; i >= 0; i--)
        {
            if (!_checkPoint.coins.Contains(totalCoins[i]))
            {
                _totalCoins[i].SetActive(true);
                _totalCoins.RemoveAt(i);
            }
        }

        // Update UI
        _guiManager.HideGameOverScreen();
        _guiManager.DisplayAllLivesImages();
        _guiManager.DisplayCoins(_totalCoins.Count);

        // Play music
        StartCoroutine(_audioManager.PlayBackgroundMusic());
    }

    /// <summary>
    /// Pause game
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0f;
        _paused = true;

        // Update UI
        _guiManager.DisplayPauseScreen();
        Cursor.visible = true;
    }

    /// <summary>
    /// Resume game
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1f;
        _paused = false;

        // Update UI
        _guiManager.HidePauseScreen();
        Cursor.visible = false;
    }

    /// <summary>
    /// Exit the game
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
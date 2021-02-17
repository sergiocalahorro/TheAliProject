using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioManager), typeof(GUIManager))]
public class GameManager : Singleton<GameManager>
{
    // Control
    private PlayerController _player;
    public bool isGameOver;

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

    // Components
    private AudioManager _audioManager;
    private GUIManager _guiManager;

    // Start is called before the first frame update
    private void Start()
    {
        // Components
        _audioManager = GetComponent<AudioManager>();
        _guiManager = GetComponent<GUIManager>();
        StartCoroutine(_audioManager.PlayBackgroundMusic());

        // Control
        isGameOver = false;
        _player = FindObjectOfType<PlayerController>();
        _totalCoins = new List<GameObject>();

        // Set player's starting position
        Vector3 startPosition = new Vector3(-14f, -5.2f, 0f);
        Spawn(startPosition);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isGameOver)
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
    public void CheckNumberOfLives(int numberOfLives)
    {
        // Update UI
        _guiManager.UpdateLivesImages(numberOfLives);
    }

    /// <summary>
    /// Game Over
    /// </summary>
    public void GameOver()
    {
        // Disable player
        _player.enabled = false;

        // Update UI
        _guiManager.DisplayGameOverScreen();

        // Play music
        _audioManager.PlayGameOverMusic();
    }

    /// <summary>
    /// Spawn player at a given position
    /// </summary>
    public void Spawn(Vector3 respawnPosition)
    {
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
        isGameOver = false;
        _player.enabled = true;
        Spawn(_checkPoint.position);

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
    /// Exit the game
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
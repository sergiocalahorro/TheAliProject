using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Game control
    private int _coinCount;
    private bool _isGameOver;

    // Audio
    [Header("Audio")]
    public AudioClip coinAudioClip;

    // Components
    private AudioSource _audioSource;
    private GUIManager _guiManager;

    // Start is called before the first frame update
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _guiManager = GetComponent<GUIManager>();

        _isGameOver = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isGameOver)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Add coin
    /// </summary>
    public void CoinPickUp()
    {
        // Add coin
        _coinCount++;

        // Play sound
        _audioSource.PlayOneShot(coinAudioClip);

        // Update UI
        _guiManager.DisplayCoinsText(_coinCount);
    }

    /// <summary>
    /// Checks the number of lives the player has
    /// </summary>
    /// <param name="numberOfLives"> Number of lives the player has </param>
    public void CheckNumberOfLives(int numberOfLives)
    {
        // Player has run out of lives
        if (numberOfLives == 0)
        {
            _isGameOver = true;
        }
        
        // Update UI
        _guiManager.DisplayLivesImages(numberOfLives);
    }

    /// <summary>
    /// Game Over
    /// </summary>
    public void GameOver()
    {
        // Update UI
        _guiManager.DisplayGameOverScreen();
    }
}
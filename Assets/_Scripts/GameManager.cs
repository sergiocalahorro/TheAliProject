using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    // Game control
    private PlayerController _player;
    public List<GameObject> coins;
    private bool _isGameOver;

    // Components
    private AudioManager _audioManager;
    private GUIManager _guiManager;

    // Start is called before the first frame update
    private void Start()
    {
        // Components
        _audioManager = GetComponent<AudioManager>();
        _guiManager = GetComponent<GUIManager>();
        _audioManager.PlayBackgroundMusic();        

        // Control
        _player = FindObjectOfType<PlayerController>();
        coins = new List<GameObject>();
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
    public void CoinPickUp(GameObject coin)
    {
        // Add coin
        coins.Add(coin);

        // Update UI
        _guiManager.DisplayCoins(coins.Count);
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
        _guiManager.UpdateLivesImages(numberOfLives);
    }

    /// <summary>
    /// Game Over
    /// </summary>
    public void GameOver()
    {
        // Update UI
        _guiManager.DisplayGameOverScreen();

        // Play music
        _audioManager.PlayGameOverMusic();
    }

    /// <summary>
    /// Restart game from last check point reached
    /// </summary>
    public void RestartFromCheckPoint()
    {
        // Game control
        _isGameOver = false;
        _player.Spawn(CheckPointController.LastPosition);

        // Compare the total coins with the ones collected on the last check point reached
        for (int i = coins.Count - 1; i >= 0; i--)
        {
            if (!CheckPointController.Coins.Contains(coins[i]))
            {
                coins[i].SetActive(true);
                coins.RemoveAt(i);
            }
        }

        // Update UI
        _guiManager.HideGameOverScreen();
        _guiManager.DisplayAllLivesImages();
        _guiManager.DisplayCoins(coins.Count);

        // Play music
        _audioManager.PlayBackgroundMusic();
    }

    /// <summary>
    /// Exit the game
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
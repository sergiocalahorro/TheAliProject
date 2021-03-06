using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    // Coins
    [Header("Coins")]
    public TextMeshProUGUI coinsText;

    // Lives
    [Header("Lives")]
    public List<GameObject> livesImages;

    // Game Over
    [Header("Game Over")]
    [SerializeField]
    private float _fadeDuration;
    private float _timer;
    public CanvasGroup gameOverCanvasGroup;
    public GameObject panelGameOverButtons;

    // Pause
    public GameObject panelPause;

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.Instance.insideGameOver)
        {
            DisplayGameOverScreen();
        }
    }

    /// <summary>
    /// Display text with the current number of coins collected by the player
    /// </summary>
    /// <param name="coinCount"> Number of coins collected by the player </param>
    public void DisplayCoins(int coinCount)
    {
        coinsText.text = coinCount.ToString();
    }

    /// <summary>
    /// Display all the lives' images
    /// </summary>
    public void DisplayAllLivesImages()
    {
        for (int i = 0; i < livesImages.Count; i++)
        {
            Image lifeImage = livesImages[i].GetComponent<Image>();
            Color lifeImageColor = lifeImage.color;
            lifeImageColor.a = 1f;
            lifeImage.color = lifeImageColor;
        }
    }

    /// <summary>
    /// Display the number of lives the player currently has
    /// </summary>
    /// <param name="numberOfLives"> Number of lives the player has </param>
    public void DisplayCurrentLivesImages(int numberOfLives)
    {
        if (numberOfLives >= 0)
        {
            for (int i = livesImages.Count - 1; i >= numberOfLives; i--)
            {
                Image lifeImage = livesImages[i].GetComponent<Image>();
                Color lifeImageColor = lifeImage.color;
                lifeImageColor.a = 0.3f;
                lifeImage.color = lifeImageColor;
            }
        }
    }

    /// <summary>
    /// Display Game Over screen
    /// </summary>
    public void DisplayGameOverScreen()
    {
        // Game over screen's fade in
        _timer += Time.deltaTime;
        gameOverCanvasGroup.alpha = Mathf.Clamp(_timer / _fadeDuration, 0f, 1f);

        // Display buttons
        if (_timer > _fadeDuration)
        {
            panelGameOverButtons.SetActive(true);
        }
    }

    /// <summary>
    /// Hide Game Over screen
    /// </summary>
    public void HideGameOverScreen()
    {
        _timer = 0f;
        gameOverCanvasGroup.alpha = 0f;
        panelGameOverButtons.SetActive(false);
    }

    /// <summary>
    /// Display screen when game is paused
    /// </summary>
    public void DisplayPauseScreen()
    {
        panelPause.SetActive(true);
    }

    /// <summary>
    /// Hide screen when game is resumed
    /// </summary>
    public void HidePauseScreen()
    {
        panelPause.SetActive(false);
    }
}
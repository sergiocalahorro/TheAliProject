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

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    /// <summary>
    /// Display text with the current number of coins collected by the player
    /// </summary>
    /// <param name="coinCount"> Number of coins collected by the player </param>
    public void DisplayCoinsText(int coinCount)
    {
        coinsText.text = coinCount.ToString();
    }

    /// <summary>
    /// Display the number of lives the player has
    /// </summary>
    /// <param name="numberOfLives"> </param>
    public void DisplayLivesImages(int numberOfLives)
    {
        if (numberOfLives >= 0)
        {
            Image lifeImage = livesImages[numberOfLives].GetComponent<Image>();
            Color lifeImageColor = lifeImage.color;
            lifeImageColor.a = 0.3f;
            lifeImage.color = lifeImageColor;
        }
    }

    /// <summary>
    /// Display Game Over screen
    /// </summary>
    public void DisplayGameOverScreen()
    {
        // Show Game Over screen
        _timer += Time.deltaTime;
        gameOverCanvasGroup.alpha = Mathf.Clamp(_timer / _fadeDuration, 0f, 1f);

        // Display buttons
        if (_timer > _fadeDuration)
        {
            DisplayMenu();
        }
    }

    public void DisplayMenu()
    {

    }
}

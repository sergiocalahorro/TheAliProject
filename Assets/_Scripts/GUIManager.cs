using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

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
    /// Update text with the current number of coins collected by the player
    /// </summary>
    /// <param name="coinCount"> Number of coins collected by the player </param>
    public void UpdateCoinsText(int coinCount)
    {
        coinsText.text = coinCount.ToString();
    }
}

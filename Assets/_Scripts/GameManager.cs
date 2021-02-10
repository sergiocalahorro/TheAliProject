using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Components
    private AudioSource _audioSource;
    private GUIManager _guiManager;

    private int _coinCount;

    public AudioClip coinAudioClip;

    // Start is called before the first frame update
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _guiManager = GetComponent<GUIManager>();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    /// <summary>
    /// Add coin
    /// </summary>
    public void CoinPickUp()
    {
        _coinCount++;

        // Play sound
        _audioSource.PlayOneShot(coinAudioClip);

        // Update UI
        _guiManager.UpdateCoinsText(_coinCount);
    }
}

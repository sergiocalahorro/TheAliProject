using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Components
    private AudioSource _audioSource;

    private int _coinCount;

    public AudioClip coinAudioClip;

    // Prevent non-Singleton constructor use
    protected GameManager() { }

    // Start is called before the first frame update
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
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
        GUIManager.Instance.UpdateCoinsText(_coinCount);
    }
}

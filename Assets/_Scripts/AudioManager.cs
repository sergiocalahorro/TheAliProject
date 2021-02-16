using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    // Control
    public bool gameOverMusicPlayed;

    // Audio
    [Header("Audio")]
    private AudioSource _audioSource;
    public AudioClip[] audioClipsBackground;
    public AudioClip audioClipGameOver;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Play music on background
    /// </summary>
    public void PlayBackgroundMusic()
    {
        gameOverMusicPlayed = false;

        for (int i = 0; i < audioClipsBackground.Length; i++)
        {
            _audioSource.clip = audioClipsBackground[i];
            _audioSource.Play();
        }
    }

    /// <summary>
    /// Play music on game over
    /// </summary>
    public void PlayGameOverMusic()
    {
        // Play music only once
        if (!gameOverMusicPlayed)
        {
            // Stop background music
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();

                // Play game over music
                _audioSource.clip = audioClipGameOver;
                _audioSource.Play();
                gameOverMusicPlayed = true;
            }
        }
    }
}
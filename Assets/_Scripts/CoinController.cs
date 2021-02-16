using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    // Audio
    private AudioSource _audioSource;

    // Particles
    private ParticleSystem _particleSystem;

    // Start is called before the first frame update
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _particleSystem = GetComponent<ParticleSystem>();
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player picks up coin
        if (other.gameObject.CompareTag("Player"))
        {
            _audioSource.Play();
            _particleSystem.Play();

            GameManager.Instance.CoinPickUp(gameObject);
            StartCoroutine(DisableAfterTime());
        }
    }

    /// <summary>
    /// Coroutine to disable this GameObject
    /// </summary>
    /// <returns> Time until this GameObject is disabled </returns>
    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(_audioSource.clip.length);

        gameObject.SetActive(false);
    }
}
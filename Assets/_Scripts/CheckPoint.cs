using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CheckPoint : MonoBehaviour
{
    // Control
    private bool _checkPointReached;

    // Lights
    [SerializeField]
    private float _lightMaxIntensity;
    [SerializeField]
    private float _lightSmoothingFactor;
    
    public Light2D[] lights;
    public GameObject lightContainer;

    // Audio
    private AudioSource _audioSource;
    public AudioClip checkPointClip;

    // Start is called before the first frame update
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    // FixedUpdate is called every fixed framerate
    private void FixedUpdate()
    {
        if (_checkPointReached && lights[0].intensity != _lightMaxIntensity)
        {
            ChangeLightIntensity();
        }
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Turn on the lights when player enters the trigger
            if (!_checkPointReached)
            {
                lightContainer.SetActive(true);
                _checkPointReached = true;

                _audioSource.PlayOneShot(checkPointClip);
            }
        }
    }

    /// <summary>
    /// Smoothly change lights' intensity
    /// </summary>
    private void ChangeLightIntensity()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = Mathf.Lerp(lights[i].intensity, _lightMaxIntensity, 
                                             Time.fixedDeltaTime * _lightSmoothingFactor);
        }
    }
}
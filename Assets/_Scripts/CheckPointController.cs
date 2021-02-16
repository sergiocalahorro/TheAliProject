using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CheckPointController : MonoBehaviour
{
    // Position
    public static Vector3 LastPosition;

    // Control
    private bool _checkPointReached;

    // Lights
    [Header("Lights")]
    [SerializeField]
    private float _lightMaxIntensity;
    [SerializeField]
    private float _mainLightMaxIntensity;
    [SerializeField]
    private float _lightSmoothingFactor;
    private bool _lightsTurnedOn;
    public Light2D[] lights;
    public Light2D mainLight;

    // Audio
    [Header("Audio")]
    private AudioSource _audioSource;
    public AudioClip checkPointClip;

    // Start is called before the first frame update
    private void Start()
    {
        // Components
        _audioSource = GetComponent<AudioSource>();

        // Last check point's reached position
        LastPosition = Vector3.zero;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    // FixedUpdate is called every fixed framerate
    private void FixedUpdate()
    {
        // Increase lights' intensity while they haven't reached max intensity
        if (_checkPointReached && !_lightsTurnedOn)
        {
            ChangeLightIntensity();
        }
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Turn on the lights and store position when player enters the trigger
            if (!_checkPointReached)
            {
                _checkPointReached = true;
                LastPosition = transform.position;

                // Play sound
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

        mainLight.intensity = Mathf.Lerp(mainLight.intensity, _mainLightMaxIntensity,
                                         Time.fixedDeltaTime * _lightSmoothingFactor);

        // Lights are turned on
        if (Mathf.Abs(mainLight.intensity - _mainLightMaxIntensity) <= Utilities.EPSILON)
        {
            _lightsTurnedOn = true;
        }
    }
}
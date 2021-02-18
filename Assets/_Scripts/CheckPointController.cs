using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CheckPointController : MonoBehaviour
{
    // Control
    private bool _checkPointReached;
    private List<GameObject> _coins;
    public List<GameObject> coins
    {
        get
        {
            return _coins;
        }
    }

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
    private AudioSource _audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        // Components
        _audioSource = GetComponent<AudioSource>();

        // Last check point's position and collected coins
        _coins = new List<GameObject>();
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

                // Set the collected coins when this check point was reached
                List<GameObject> totalCoins = GameManager.Instance.totalCoins;
                for (int i = 0; i < totalCoins.Count; i++)
                {
                    _coins.Add(totalCoins[i]);
                }

                // Send this check point to the game manager
                GameManager.Instance.checkPoint = this;

                // Play sound
                _audioSource.Play();
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
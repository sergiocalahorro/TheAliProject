using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    // Cinemachine camera
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _perlin;

    // Shake
    [Header("Shake")]
    [SerializeField]
    private float _shakeIntensity;
    [SerializeField]
    private float _shakeDuration;
    private float _shakeTimer;
    private float _shakeTimerTotal;
    private bool _isShaking;
    private bool _isShakeAvailable;

    // Zoom
    [Header("Zoom")]
    [SerializeField]
    private float _startZoomAmount;
    [SerializeField]
    private float _zoomAmount;
    [SerializeField]
    private float _zoomDuration;
    private float _zoomTimer;
    private float _zoomTimerTotal;
    private bool _isZooming;
    private bool _isZoomAvailable;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        _perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _isShakeAvailable = true;
        _isZoomAvailable = true;
    }

    /// <summary>
    /// Shake camera with a certain intensity during some time
    /// </summary>
    public void Shake()
    {
        if (!_isShaking)
        {
            // Set shake intensity
            _perlin.m_AmplitudeGain = _shakeIntensity;

            // Set duration of the shake
            _shakeTimerTotal = _shakeDuration;
            _shakeTimer = _shakeDuration;

            _isShaking = true;
            _isShakeAvailable = false;
        }

        // Decrease shake intensity over time
        if (_isShaking)
        {
            if (_shakeTimer > 0f)
            {
                _shakeTimer -= Time.deltaTime;
                _perlin.m_AmplitudeGain = Mathf.Lerp(_shakeIntensity, 0f,
                                                    1f - (_shakeTimer / _shakeTimerTotal));
            }
        }
    }

    /// <summary>
    /// Reset camera shake effect
    /// </summary>
    public void ResetShake()
    {
        if (!_isShakeAvailable)
        {
            _isShaking = false;
            _isShakeAvailable = true;
        }
    }

    /// <summary>
    /// Make a zoom in
    /// </summary>
    public void ZoomIn()
    {
        if (!_isZooming)
        {
            // Set zoom amount
            _virtualCamera.m_Lens.OrthographicSize = _zoomAmount;

            // Set duration of the zoom in
            _zoomTimerTotal = _zoomDuration;
            _zoomTimer = _zoomDuration;

            _isZooming = true;
            _isZoomAvailable = false;
        }

        // Increase zoom amount over time
        if (_isZooming)
        {
            if (_zoomTimer > 0f)
            {
                _zoomTimer -= Time.deltaTime;
                _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(_startZoomAmount, _zoomAmount,
                                                         1f - (_zoomTimer / _zoomTimerTotal));
            }
        }
    }

    /// <summary>
    /// Reset camera zoom in to its initial value
    /// </summary>
    public void ResetZoomIn()
    {
        if (!_isZoomAvailable)
        {
            _virtualCamera.m_Lens.OrthographicSize = _startZoomAmount;
            _isZooming = false;
            _isZoomAvailable = true;
        }
    }
}
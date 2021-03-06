using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    private List<ParallaxLayer> _parallaxLayers = new List<ParallaxLayer>();
    private ParallaxCamera _parallaxCamera;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (_parallaxCamera == null)
        {
            _parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();
        }

        if (_parallaxCamera != null)
        {
            _parallaxCamera.delegateCameraMove += Move;
        }

        SetLayers();
    }
  
    /// <summary>
    /// Set layers from the background to which parallax effect will be applied
    /// </summary>
    private void SetLayers()
    {
        _parallaxLayers.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();
  
            if (layer != null)
            {
                _parallaxLayers.Add(layer);
            }
        }
    }
    
    /// <summary>
    /// Move all background layers
    /// </summary>
    /// <param name="delta"> Camera's movement amount </param>
    private void Move(float delta)
    {
        foreach (ParallaxLayer layer in _parallaxLayers)
        {
            layer.Move(delta);
        }
    }
}
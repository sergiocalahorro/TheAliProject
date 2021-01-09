using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField]
    private float _parallaxFactor;
    
    /// <summary>
    /// Move layer applying a factor
    /// </summary>
    /// <param name="delta"> Movement amount </param>
    public void Move(float delta)
    {
        Vector3 currentPosition = transform.localPosition;
        currentPosition.x -= delta * _parallaxFactor;
        transform.localPosition = currentPosition;
    }
}
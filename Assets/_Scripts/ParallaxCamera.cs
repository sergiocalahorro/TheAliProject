using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour 
{
    private float _startPositionX;

    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate delegateCameraMove;
    
    // Start is called before the first frame update
    private void Start()
    {
        _startPositionX = transform.position.x;
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (transform.position.x != _startPositionX)
        {
            if (delegateCameraMove != null)
            {
                float delta = _startPositionX - transform.position.x;
                delegateCameraMove(delta);
            }
            _startPositionX = transform.position.x;
        }
    }
}
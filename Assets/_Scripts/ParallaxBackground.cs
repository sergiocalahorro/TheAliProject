using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Vector2 _parallaxMultiplier;
    private Transform _cameraTransform;
    private Vector3 _lastCameraPosition;
    private Vector2 _textureUnitSize;

    // Start is called before the first frame update
    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        _lastCameraPosition = _cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        _textureUnitSize.x = texture.width / sprite.pixelsPerUnit;
        _textureUnitSize.y = texture.height / sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        // Move backgrounds according to camera movement
        Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * _parallaxMultiplier.x,
                                          deltaMovement.y * _parallaxMultiplier.y);
        _lastCameraPosition = _cameraTransform.position;

        // Infinite repeating in horizontal Axis
        if (Mathf.Abs(_cameraTransform.position.x - transform.position.x) >=
            _textureUnitSize.x)
        {
            float offsetPositionX = (_cameraTransform.position.x - transform.position.x) %
                                    _textureUnitSize.x;
            transform.position = new Vector3(_cameraTransform.position.x + offsetPositionX,
                transform.position.y);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 _offset = new Vector3(0f, 1f, -10f);
    
    public Transform player;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 lookAt = player.position + _offset;
        transform.position = lookAt;
    }
}
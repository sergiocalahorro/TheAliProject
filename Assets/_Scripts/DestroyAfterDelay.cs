using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField]
    private float _delay;

    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, _delay);
    }
}

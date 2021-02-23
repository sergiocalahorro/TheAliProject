using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float _cooldownTime;
    private float _counter;
    private bool _isShooting;
    public bool isShooting
    {
        get
        {
            return _isShooting;
        }
    }

    public Transform firePoint;
    public GameObject bulletPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        _counter = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isShooting)
        {
            _counter += Time.deltaTime;
            if (_counter >= _cooldownTime)
            {
                _isShooting = false;
                _counter = 0f;
            }
        }
    }

    /// <summary>
    /// Shooting logic
    /// </summary>
    public void Shoot()
    {
        if (!_isShooting)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            _isShooting = true;
        }
    }
}
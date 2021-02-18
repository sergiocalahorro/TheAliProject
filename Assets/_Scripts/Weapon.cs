using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PlayerController _player;

    public Transform firePoint;
    public GameObject bulletPrefab;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        _player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_player != null)
        {
            // Player is shooting
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            // Enemy is shooting
        }
    }

    /// <summary>
    /// Shooting logic
    /// </summary>
    private void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}

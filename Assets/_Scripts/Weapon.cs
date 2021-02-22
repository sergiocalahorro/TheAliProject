using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PlayerController _player;

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
            if (_player.isGrounded && Input.GetButtonDown("Fire1"))
            {
                Shoot();
                _isShooting = true;
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                _isShooting = false;
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

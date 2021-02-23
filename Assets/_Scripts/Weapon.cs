using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float _cooldownTime;
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

    /// <summary>
    /// Weapon shooting logic
    /// </summary>
    /// <returns> Delay between shots </returns>
    public IEnumerator Shoot()
    {
        // Weapon shoots a bullet
        _isShooting = true;
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Weapon can shoot again after a delay
        yield return new WaitForSeconds(_cooldownTime);
        _isShooting = false;
    }
}
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Bullet")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    /// <summary>
    /// Instantiate a bullet at the fire point position
    /// </summary>
    /// <returns> Delay between shots </returns>
    public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
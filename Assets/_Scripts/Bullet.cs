using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _damageAmount;
    private Rigidbody2D _rigidbody;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody.velocity = -transform.right * _speed;
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Platform"))
        {
            Destroy(gameObject);
        }

        if (other.gameObject.tag.Equals("Enemy"))
        {
            Destroy(gameObject);
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(_damageAmount);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpikes : Enemy
{
    // Start is called before the first frame update
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public override void Attack()
    {
        StartCoroutine(playerController.TakeDamage(attackDamage));
    }

    public override void Die()
    {
        numberOfLives = 0;
    }

    // OnColliderEnter2D is called when the Collider2D other enters this collider
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Attack();
        }
    }
}
using UnityEngine;

public class EnemySpikes : Enemy
{
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        healthPoints = 0;
        isImmortal = true;
    }

    public override void Attack(PlayerController player)
    {
        if (!player.isDead)
        {
            StartCoroutine(player.TakeDamage(attackDamage));
        }
    }

    // OnColliderEnter2D is called when the Collider2D other enters this collider
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            // Attack player
            player.KnockBack();
            Attack(player);
        }
    }
}
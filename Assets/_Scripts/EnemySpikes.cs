using UnityEngine;

public class EnemySpikes : Enemy
{
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
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
            ContactPoint2D contact = other.GetContact(0);
            Vector2 playerPosition = new Vector2(player.transform.position.x, 
                                                 player.transform.position.y);
            Vector2 direction = contact.point - playerPosition;
            direction = -direction.normalized;
            player.KnockBack(direction, 5000f, 20f);
            Attack(player);
        }
    }
}
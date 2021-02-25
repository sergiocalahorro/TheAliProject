using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int numberOfLives;
    [SerializeField]
    protected int attackDamage;
    protected bool isImmortal;

    /// <summary>
    /// Enemy's attack behaviour
    /// </summary>
    /// <param name="player"> Player that is attacked </param>
    public abstract void Attack(PlayerController player);

    /// <summary>
    /// Enemy takes a certain amount of damage
    /// </summary>
    /// <param name="damageAmount"> Amount of damage the enemy takes </param>
    public void TakeDamage(int damageAmount)
    {
        if (isImmortal)
        {
            return;  
        }

        numberOfLives -= damageAmount;
        if (numberOfLives <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Enemy's death behaviour
    /// </summary>
    private void Die()
    {
        gameObject.SetActive(false);
    }
}
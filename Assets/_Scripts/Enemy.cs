using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int numberOfLives;
    [SerializeField]
    protected int attackDamage;
    protected PlayerController playerController;

    /// <summary>
    /// Enemy's attack behaviour
    /// </summary>
    public abstract void Attack();

    /// <summary>
    /// Enemy's death behaviour
    /// </summary>
    public abstract void Die();

    /// <summary>
    /// Enemy takes a certain amount of damage
    /// </summary>
    /// <param name="damageAmount"> Amount of damage the enemy takes </param>
    public void TakeDamage(int damageAmount)
    {
        numberOfLives -= damageAmount;
        if (numberOfLives <= 0)
        {
            Die();
        }
    }
}
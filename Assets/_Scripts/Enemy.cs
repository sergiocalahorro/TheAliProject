using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected int _health;
    protected int _damage;
    protected PlayerController _playerController;

    /// <summary>
    /// Enemy's attack behaviour
    /// </summary>
    public abstract void Attack();

    /// <summary>
    /// Enemy's death behaviour
    /// </summary>
    public abstract void Die();
}

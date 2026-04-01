using UnityEngine;

/// <summary>
/// Vida, daño de contacto opcional y muerte. Emite evento al morir para mejoras tipo "on kill".
/// </summary>
public class EnemyBase : MonoBehaviour
{
    public static System.Action<Vector2> OnEnemyKilled;
    public static int AliveCount { get; private set; }

    [SerializeField] protected int maxHealth = 30;
    [SerializeField] protected int contactDamage = 10;
    [SerializeField] protected float contactCooldown = 0.8f;

    protected int currentHealth;
    float nextContactDamageTime;

    public bool IsAlive => currentHealth > 0;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        AliveCount++;
        var sr = GetComponent<SpriteRenderer>();
        RuntimeVisuals.EnsureSprite(sr);
        if (sr != null)
            sr.sortingOrder = Mathf.Max(sr.sortingOrder, 5);
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        AliveCount = Mathf.Max(0, AliveCount - 1);
        OnEnemyKilled?.Invoke(transform.position);
        Destroy(gameObject);
    }

    protected void TryDamagePlayer(Collider2D other)
    {
        var p = other.GetComponentInParent<PlayerController>();
        if (p == null)
            return;
        if (Time.time < nextContactDamageTime)
            return;
        nextContactDamageTime = Time.time + contactCooldown;
        p.TakeDamage(contactDamage);
    }
}

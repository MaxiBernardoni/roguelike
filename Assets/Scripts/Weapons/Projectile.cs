using UnityEngine;

/// <summary>
/// Proyectil que avanza, aplica daño y se destruye al impactar (o rebota / explota según flags).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] LayerMask enemyMask;

    Rigidbody2D rb;
    Collider2D col;
    int damage;
    float speed;
    int bouncesLeft;
    int pierceExtraHits;
    bool explosive;
    float explosionRadius;
    int explosionDamage;
    bool isPlayerBullet;
    Vector2 velocityDir;

    public void Init(
        Vector2 direction,
        int dmg,
        float spd,
        int bounces,
        int pierceExtra,
        bool explode,
        float expRadius,
        int expDmg,
        bool playerBullet)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        if (col != null)
            col.isTrigger = true;
        damage = dmg;
        speed = spd;
        bouncesLeft = bounces;
        pierceExtraHits = pierceExtra;
        explosive = explode;
        explosionRadius = expRadius;
        explosionDamage = expDmg;
        isPlayerBullet = playerBullet;
        if (explosive)
            enemyMask = GameLayers.GetEnemyMask(enemyMask);
        velocityDir = direction.sqrMagnitude > 0.01f ? direction.normalized : Vector2.right;
        rb.linearVelocity = velocityDir * speed;
        float ang = Mathf.Atan2(velocityDir.y, velocityDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, ang);
        var srVis = GetComponent<SpriteRenderer>();
        RuntimeVisuals.EnsureSprite(srVis);
        if (srVis != null && isPlayerBullet)
            srVis.sortingOrder = Mathf.Max(srVis.sortingOrder, 8);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerBullet)
        {
            var eb = other.GetComponent<EnemyBase>();
            if (eb != null)
            {
                eb.TakeDamage(damage);
                if (explosive)
                    DoExplosion(other.transform.position);
                if (pierceExtraHits > 0)
                {
                    pierceExtraHits--;
                    NudgeThrough(other);
                    return;
                }
                if (bouncesLeft > 0)
                {
                    bouncesLeft--;
                    TryBounce(other.transform.position);
                    return;
                }
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            var p = other.GetComponent<PlayerController>();
            if (p != null)
            {
                p.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    void DoExplosion(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, explosionRadius, enemyMask);
        foreach (var c in hits)
        {
            var eb = c.GetComponent<EnemyBase>();
            if (eb != null)
                eb.TakeDamage(explosionDamage);
        }
    }

    void TryBounce(Vector2 hitPos)
    {
        Vector2 inward = ((Vector2)transform.position - hitPos).normalized;
        if (inward.sqrMagnitude < 0.01f)
            inward = -velocityDir;
        velocityDir = Vector2.Reflect(velocityDir, inward);
        rb.linearVelocity = velocityDir * speed;
        float ang = Mathf.Atan2(velocityDir.y, velocityDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, ang);
    }

    void NudgeThrough(Collider2D hit)
    {
        transform.position += (Vector3)(velocityDir * 0.2f);
        if (col != null && hit != null)
            Physics2D.IgnoreCollision(col, hit, true);
    }
}

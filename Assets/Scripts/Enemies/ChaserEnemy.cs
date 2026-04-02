using UnityEngine;

/// <summary>
/// Persigue al jugador y hace daño por contacto.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class ChaserEnemy : EnemyBase
{
    [SerializeField] float moveSpeed = 3.2f;
    [SerializeField] float screenBoundsMargin = 0.45f;

    Transform player;
    Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
        CombatVisuals.ApplyChaser(GetComponent<SpriteRenderer>());
    }

    void Start()
    {
        if (PlayerController.Instance != null)
            player = PlayerController.Instance.transform;
    }

    void FixedUpdate()
    {
        if (player == null && PlayerController.Instance != null)
            player = PlayerController.Instance.transform;

        if (player == null || !gameObject.activeSelf)
            return;

        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
        ScreenBounds.ClampRigidbody(rb, screenBoundsMargin);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }
}

using UnityEngine;

/// <summary>
/// Movimiento WASD con Rigidbody2D, dash con cooldown y vida.
/// Expone referencia al arma para que las mejoras la modifiquen.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 14f;
    [SerializeField] float dashDuration = 0.12f;
    [SerializeField] float dashCooldownSeconds = 1f;

    [Header("Health")]
    [SerializeField] int maxHealth = 100;

    [Header("Refs")]
    [SerializeField] WeaponController weapon;

    Rigidbody2D rb;
    Vector2 moveInput;
    Vector2 lastMoveDir = Vector2.right;

    bool dashing;
    float dashEndTime;
    float nextDashTime;

    public WeaponController Weapon =>
        weapon != null ? weapon : (weapon = GetComponentInChildren<WeaponController>(true));
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public float HealthRatio => maxHealth <= 0 ? 0f : (float)CurrentHealth / maxHealth;

    public float DashCooldownMax => dashCooldownSeconds;
    public float DashCooldownRemaining => Mathf.Max(0f, nextDashTime - Time.time);

    public bool IsLowHealth => HealthRatio <= 0.3f;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        CurrentHealth = maxHealth;
        if (weapon == null)
            weapon = GetComponentInChildren<WeaponController>(true);

        CombatVisuals.ApplyPlayer(GetComponent<SpriteRenderer>());
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDir = moveInput.normalized;

        if (Input.GetKeyDown(KeyCode.Space) && !dashing && Time.time >= nextDashTime)
            StartDash();
    }

    void FixedUpdate()
    {
        if (dashing)
        {
            if (Time.time >= dashEndTime)
            {
                dashing = false;
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    void StartDash()
    {
        dashing = true;
        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldownSeconds;
        Vector2 dir = moveInput.sqrMagnitude > 0.01f ? moveInput.normalized : lastMoveDir;
        rb.linearVelocity = dir * dashSpeed;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        if (CurrentHealth <= 0)
            gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        CurrentHealth += amount;
    }

    public void SetMoveSpeedMultiplier(float mult)
    {
        moveSpeed *= mult;
    }

    public void ReduceDashCooldown(float seconds)
    {
        dashCooldownSeconds = Mathf.Max(0.2f, dashCooldownSeconds - seconds);
    }
}

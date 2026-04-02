using UnityEngine;

/// <summary>
/// Colores y escalas fijos para distinguir jugador, enemigos y balas sin assets externos.
/// </summary>
public static class CombatVisuals
{
    public static readonly Color PlayerColor = new Color(0.18f, 0.62f, 0.92f);
    public static readonly Color PlayerProjectileColor = new Color(0.82f, 0.95f, 1f);
    public static readonly Color EnemyProjectileColor = new Color(1f, 0.32f, 0.1f);
    public static readonly Color ChaserColor = new Color(0.92f, 0.16f, 0.2f);
    public static readonly Color ShooterColor = new Color(0.78f, 0.38f, 0.95f);

    public const float PlayerScale = 1f;
    public const float PlayerProjectileScale = 0.42f;
    public const float EnemyProjectileScale = 0.38f;
    public const float ChaserScale = 1.3f;
    public const float ShooterScale = 1f;

    public const int SortPlayer = 3;
    public const int SortEnemy = 5;
    public const int SortEnemyBullet = 6;
    public const int SortPlayerBullet = 9;
    public const int SortExplosion = 25;

    public static void ApplyPlayer(SpriteRenderer sr)
    {
        if (sr == null)
            return;
        RuntimeVisuals.EnsureSprite(sr);
        sr.color = PlayerColor;
        sr.sortingOrder = Mathf.Max(sr.sortingOrder, SortPlayer);
        sr.transform.localScale = Vector3.one * PlayerScale;
    }

    public static void ApplyProjectile(SpriteRenderer sr, bool isPlayerBullet)
    {
        if (sr == null)
            return;
        RuntimeVisuals.EnsureSprite(sr);
        if (isPlayerBullet)
        {
            sr.color = PlayerProjectileColor;
            sr.sortingOrder = SortPlayerBullet;
            sr.transform.localScale = Vector3.one * PlayerProjectileScale;
        }
        else
        {
            sr.color = EnemyProjectileColor;
            sr.sortingOrder = SortEnemyBullet;
            sr.transform.localScale = Vector3.one * EnemyProjectileScale;
        }
    }

    public static void ApplyChaser(SpriteRenderer sr)
    {
        if (sr == null)
            return;
        RuntimeVisuals.EnsureSprite(sr);
        sr.color = ChaserColor;
        sr.sortingOrder = SortEnemy;
        sr.transform.localScale = Vector3.one * ChaserScale;
    }

    public static void ApplyShooter(SpriteRenderer sr)
    {
        if (sr == null)
            return;
        RuntimeVisuals.EnsureSprite(sr);
        sr.color = ShooterColor;
        sr.sortingOrder = SortEnemy;
        sr.transform.localScale = Vector3.one * ShooterScale;
    }
}

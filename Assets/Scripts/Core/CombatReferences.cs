using UnityEngine;

/// <summary>
/// Referencias mínimas compartidas en runtime para que el tirador enemigo pueda usar el mismo proyectil que el jugador sin cablear dos veces el prefab.
/// </summary>
public static class CombatReferences
{
    public static Projectile PlayerProjectilePrefab { get; private set; }

    public static void RegisterPlayerProjectile(Projectile prefab)
    {
        if (prefab != null)
            PlayerProjectilePrefab = prefab;
    }
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Catálogo de mejoras con efectos claros en combate (no solo +1% stats).
/// </summary>
public static class UpgradeDatabase
{
    static Upgrade[] _templates;

    public static Upgrade[] GetAll()
    {
        if (_templates != null)
            return _templates;

        _templates = new[]
        {
            new Upgrade
            {
                Name = "Multidisparo",
                Description = "Dispara un proyectil extra en abanico.",
                ApplyEffect = p => p.Weapon.AddExtraProjectile()
            },
            new Upgrade
            {
                Name = "Ricochete",
                Description = "Los proyectiles rebotan una vez al impactar.",
                ApplyEffect = p => p.Weapon.SetProjectileBounces(1)
            },
            new Upgrade
            {
                Name = "Explosivo",
                Description = "Impactos generan una pequeña explosión en área.",
                ApplyEffect = p => p.Weapon.EnableExplosive(2.4f, 10)
            },
            new Upgrade
            {
                Name = "Hiperdisparo",
                Description = "Aumenta mucho la cadencia de disparo.",
                ApplyEffect = p => p.Weapon.AddFireRateMultiplier(1.35f)
            },
            new Upgrade
            {
                Name = "Dash ágil",
                Description = "Reduce el enfriamiento del dash.",
                ApplyEffect = p => p.ReduceDashCooldown(0.35f)
            },
            new Upgrade
            {
                Name = "Furia desesperada",
                Description = "Mucho más daño con poca vida (≤30%).",
                ApplyEffect = p => p.Weapon.SetLowHpDamageMultiplier(1.6f)
            },
            new Upgrade
            {
                Name = "Semilla de caos",
                Description = "Al matar un enemigo, brota un proyectil desde su cuerpo.",
                ApplyEffect = p => p.Weapon.EnableSpawnOnKill()
            },
            new Upgrade
            {
                Name = "Corazón endurecido",
                Description = "Aumenta la vida máxima y cura la misma cantidad.",
                ApplyEffect = p => p.AddMaxHealth(25)
            },
            new Upgrade
            {
                Name = "Zancadas largas",
                Description = "Velocidad de movimiento mayor.",
                ApplyEffect = p => p.SetMoveSpeedMultiplier(1.12f)
            },
            new Upgrade
            {
                Name = "Perforación",
                Description = "Los proyectiles atraviesan a un enemigo adicional.",
                ApplyEffect = p => p.Weapon.AddPierceHits(1)
            }
        };

        return _templates;
    }

    public static Upgrade[] PickRandom(int count)
    {
        var list = new List<Upgrade>(GetAll());
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }

        int n = Mathf.Min(count, list.Count);
        var result = new Upgrade[n];
        for (int i = 0; i < n; i++)
            result[i] = Clone(list[i]);
        return result;
    }

    static Upgrade Clone(Upgrade source)
    {
        return new Upgrade
        {
            Name = source.Name,
            Description = source.Description,
            ApplyEffect = source.ApplyEffect
        };
    }
}

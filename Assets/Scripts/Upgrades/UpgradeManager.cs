using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registra las mejoras obtenidas y las aplica sobre el jugador.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public List<Upgrade> Acquired { get; } = new List<Upgrade>();

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Apply(Upgrade upgrade, PlayerController player)
    {
        if (upgrade == null || player == null)
            return;

        Acquired.Add(upgrade);
        upgrade.ApplyEffect?.Invoke(player);
    }
}

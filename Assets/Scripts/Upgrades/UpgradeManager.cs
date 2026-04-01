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
        if (upgrade == null || player == null || upgrade.ApplyEffect == null)
            return;

        try
        {
            upgrade.ApplyEffect.Invoke(player);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("UpgradeManager: falló al aplicar la mejora: " + ex.Message);
            return;
        }

        Acquired.Add(upgrade);
    }
}

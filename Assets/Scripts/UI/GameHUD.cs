using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra vida, enfriamiento del dash y número de oleada.
/// </summary>
public class GameHUD : MonoBehaviour
{
    [SerializeField] Text hpText;
    [SerializeField] Text dashText;
    [SerializeField] Text waveText;
    [SerializeField] WaveManager waveManager;

    void Awake()
    {
        if (waveManager == null)
            waveManager = FindFirstObjectByType<WaveManager>();
    }

    void Update()
    {
        var p = PlayerController.Instance;
        if (p != null && hpText != null)
            hpText.text = "HP: " + p.CurrentHealth + " / " + p.MaxHealth;

        if (p != null && dashText != null)
        {
            float cd = p.DashCooldownRemaining;
            dashText.text = cd > 0.05f ? "Dash: " + cd.ToString("0.0") + " s" : "Dash: listo";
        }

        if (waveManager != null && waveText != null)
            waveText.text = "Oleada: " + waveManager.WaveNumber;
    }
}

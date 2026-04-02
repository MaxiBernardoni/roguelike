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
    [SerializeField] Text ammoText;
    [SerializeField] Color ammoNormalColor = Color.white;
    [SerializeField] Color ammoLowColor = new Color(1f, 0.45f, 0.35f);
    [SerializeField] Color ammoReloadColor = new Color(0.95f, 0.65f, 0.35f);
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

        if (p != null && ammoText != null)
        {
            var w = p.Weapon;
            if (w != null)
            {
                if (w.IsReloading)
                {
                    ammoText.text = "RELOADING...";
                    ammoText.color = ammoReloadColor;
                }
                else
                {
                    ammoText.text = "Ammo: " + w.CurrentAmmo + " / " + w.MaxAmmo;
                    float frac = w.MaxAmmo > 0 ? (float)w.CurrentAmmo / w.MaxAmmo : 1f;
                    bool low = frac <= 0.25f && w.CurrentAmmo > 0;
                    ammoText.color = low ? ammoLowColor : ammoNormalColor;
                }
            }
        }
    }
}

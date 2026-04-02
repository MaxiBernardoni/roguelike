using UnityEngine;

/// <summary>
/// Pausa el juego, muestra 3 mejoras y reanuda al elegir una.
/// </summary>
public class RewardManager : MonoBehaviour
{
    [SerializeField] RewardUI rewardUi;

    void Awake()
    {
        if (rewardUi == null)
            rewardUi = FindFirstObjectByType<RewardUI>();
    }

    public void BeginReward(int waveNumber, System.Action onClosed)
    {
        if (rewardUi == null)
        {
            Debug.LogWarning("RewardManager: RewardUI no asignada; se omite la elección de mejora.");
            onClosed?.Invoke();
            return;
        }

        Time.timeScale = 0f;
        Upgrade[] options = UpgradeDatabase.PickRandom(3);
        rewardUi.Show(
            waveNumber,
            options,
            upgrade =>
            {
                if (UpgradeManager.Instance != null && PlayerController.Instance != null && upgrade != null)
                    UpgradeManager.Instance.Apply(upgrade, PlayerController.Instance);

                Time.timeScale = 1f;
                onClosed?.Invoke();
            },
            () => UpgradeDatabase.PickRandom(3));
    }
}

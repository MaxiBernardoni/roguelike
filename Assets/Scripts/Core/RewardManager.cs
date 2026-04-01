using UnityEngine;

/// <summary>
/// Pausa el juego, muestra 3 mejoras y reanuda al elegir una.
/// </summary>
public class RewardManager : MonoBehaviour
{
    [SerializeField] RewardUI rewardUi;

    public void BeginReward(int waveNumber, System.Action onClosed)
    {
        if (rewardUi == null)
        {
            onClosed?.Invoke();
            return;
        }

        Time.timeScale = 0f;
        Upgrade[] options = UpgradeDatabase.PickRandom(3);
        rewardUi.Show(waveNumber, options, upgrade =>
        {
            Time.timeScale = 1f;
            if (UpgradeManager.Instance != null && PlayerController.Instance != null)
                UpgradeManager.Instance.Apply(upgrade, PlayerController.Instance);
            onClosed?.Invoke();
        });
    }
}

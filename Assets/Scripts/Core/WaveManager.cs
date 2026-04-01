using UnityEngine;

/// <summary>
/// Spawnea enemigos durante un tiempo; al terminar espera a que no quede nadie y abre recompensas.
/// </summary>
public class WaveManager : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] float spawnInterval = 1.1f;
    [SerializeField] float waveDurationSeconds = 25f;
    [SerializeField] float spawnRadius = 11f;
    [SerializeField] RewardManager rewardManager;

    int waveNumber = 1;
    float spawnTimer;
    float waveTimer;
    bool spawning;
    bool waitingClear;

    public int WaveNumber => waveNumber;

    void Start()
    {
        BeginWave();
    }

    void Update()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            return;

        if (spawning)
        {
            waveTimer += Time.deltaTime;
            spawnTimer += Time.deltaTime;

            if (waveTimer < waveDurationSeconds && spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                SpawnOne();
            }

            if (waveTimer >= waveDurationSeconds)
            {
                spawning = false;
                waitingClear = true;
            }
        }
        else if (waitingClear && EnemyBase.AliveCount == 0)
        {
            waitingClear = false;
            OpenRewards();
        }
    }

    void BeginWave()
    {
        spawning = true;
        waitingClear = false;
        waveTimer = 0f;
        spawnTimer = spawnInterval * 0.5f;
    }

    void SpawnOne()
    {
        if (PlayerController.Instance == null)
            return;

        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 pos = PlayerController.Instance.transform.position + (Vector3)offset;
        Instantiate(prefab, pos, Quaternion.identity);
    }

    void OpenRewards()
    {
        if (rewardManager != null)
            rewardManager.BeginReward(waveNumber, OnRewardClosed);
        else
            OnRewardClosed();
    }

    void OnRewardClosed()
    {
        waveNumber++;
        BeginWave();
    }
}

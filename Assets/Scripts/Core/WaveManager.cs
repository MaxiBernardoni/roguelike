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

    void Awake()
    {
        if (rewardManager == null)
            rewardManager = FindFirstObjectByType<RewardManager>();
    }

    int waveNumber = 1;
    float spawnTimer;
    float waveTimer;
    bool spawning;
    bool waitingClear;

    public int WaveNumber => waveNumber;

    void Start()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            Debug.LogError("WaveManager: asigna al menos un prefab de enemigo.");
        if (rewardManager == null)
            Debug.LogWarning("WaveManager: sin RewardManager no habrá pantalla de mejoras (solo avanza la oleada).");

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
        var player = PlayerController.Instance != null
            ? PlayerController.Instance
            : FindFirstObjectByType<PlayerController>();
        if (player == null)
            return;

        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        if (prefab == null)
            return;

        Vector2 raw = Random.insideUnitCircle;
        if (raw.sqrMagnitude < 0.01f)
            raw = Vector2.right;
        Vector2 offset = raw.normalized * spawnRadius;
        Vector3 pos = player.transform.position + (Vector3)offset;
        var spawned = Instantiate(prefab, pos, Quaternion.identity);
        RuntimeVisuals.EnsureSpritesUnder(spawned);
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

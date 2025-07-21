using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] List<GameObject> powerupPrefabs;
    [SerializeField] float spawnInterval = 10f;

    Vector3 lastSpawnPosition = Vector3.zero;
    float minX = -12f, maxX = 12f;
    float minZ = -6.5f, maxZ = 6.5f;
    float yPos = 25f;
    public static SpawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (!GameManager.Instance.IsGameOver)
        {
            SpawnPowerup();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPowerup()
    {
        if (powerupPrefabs.Count == 0) return;

        // Pick a random prefab
        GameObject prefab = powerupPrefabs[Random.Range(0, powerupPrefabs.Count)];

        // Find a spawn position further away from the last one
        Vector3 spawnPos;
        int maxAttempts = 20;
        float minDistance = 5f;

        do
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);
            spawnPos = new Vector3(x, yPos, z);
            maxAttempts--;
        } while (Vector3.Distance(spawnPos, lastSpawnPosition) < minDistance && maxAttempts > 0);

        lastSpawnPosition = spawnPos;

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

}

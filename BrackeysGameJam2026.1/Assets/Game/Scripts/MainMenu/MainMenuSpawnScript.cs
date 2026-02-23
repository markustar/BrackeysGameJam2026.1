using UnityEngine;
using System.Collections;

public class MainMenuSpawnScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay = 2.0f;

    private GameObject currentEnemy;
    private bool isSpawning = false;

    void Update()
    {
        if (currentEnemy == null && !isSpawning)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        isSpawning = true;

        yield return new WaitForSeconds(spawnDelay);

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        currentEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);

        isSpawning = false;
    }
}

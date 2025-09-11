using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner01 : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemyCount = 5;
    public float spacing = 1f; // 出現時の横間隔
    public Transform spawnPoint;

    public float spawnInterval = 5f; // Wave間隔
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 pos = spawnPoint.position + new Vector3(i * spacing, 0, 0);
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            enemy.AddComponent<EnemyMover>(); // 敵1体ごとに動きを設定
        }
    }
}

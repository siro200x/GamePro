using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public Transform[] spawnPoints; // 出現位置を複数登録できる

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        int rand = Random.Range(0, spawnPoints.Length);
        // Debug.Log("Spawn at index:" + rand);
        Instantiate(enemyPrefab, spawnPoints[rand].position, Quaternion.identity);
    }

    void Update()
    {

    }
}

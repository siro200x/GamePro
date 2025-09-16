using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner03 : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemyCount = 5;
    public float spacing = 1f; // 出現時の横間隔
    public Transform spawnPoint;

    public float spawnInterval = 5f; // Wave間隔
    public int maxWaves = 3; // 出現上限
    public int spawnedWaves = 0; // 現在までに出現したWave数

    public float initialDelay = 5f; //ゲーム開始から敵を出すまでの秒数

    private float timer;

    void Start()
    {
        timer = -initialDelay; //Startでtimerを負の値にして遅延を作る
    }

    void Update()
    {
        if (spawnedWaves >= maxWaves) return;//上限に達したら終了
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
            enemy.AddComponent<EnemyMoverHome>(); // 敵1体ごとに動きを設定
        }
        spawnedWaves++;
    }
}

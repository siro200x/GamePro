using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveManager : MonoBehaviour
{
    public GameObject formationPrefab; // EnemyFormation プレハブ
    public Transform spawnPoint; // 右端の出現位置
    public float spawnInterval = 5f; //Wave間隔(秒)

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
        Instantiate(formationPrefab, spawnPoint.position, Quaternion.identity);
    }
}

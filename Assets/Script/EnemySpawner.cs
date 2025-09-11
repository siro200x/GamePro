using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public Transform[] spawnPoints; // 出現位置を複数登録できる

    private GameObject player;

    void Start()
    {
        // 最初の一度だけ探す
        player = GameObject.Find("Azarashi");
        InvokeRepeating("SpawnEnemy", 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        int rand = Random.Range(0, spawnPoints.Length);
        // Debug.Log("Spawn at index:" + rand);
        GameObject ene = Instantiate(enemyPrefab, spawnPoints[rand].position, Quaternion.identity);
        ene.GetComponent<Enemy>().player = player; // キャッシュ済みを渡す
    }

    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFormation : MonoBehaviour
{
    public GameObject enemyPrefab; // 雑魚敵プレハブ
    public int enemyCount = 5; // 編隊の数
    public float spacing = 1.0f; // 横の間隔
    public float speed = 2f; // 左への移動速度
    public float amplitude = 1.5f; // Z字の上下幅
    public float frequency = 2f; // Z字の速さ

    private GameObject[] enemies;
    private float timer;



    void Start()
    {

        enemies = new GameObject[enemyCount];

        // 編隊を右から作成
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 pos = transform.position + new Vector3(i * spacing, 0, 0);
            enemies[i] = Instantiate(enemyPrefab, pos, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null) continue;

            // X方向に左へ移動
            float x = -speed * timer;

            // Y方向にZ字パターン
            float offset = i * 0.3f;
            float y = Mathf.Sin((timer + offset) * frequency) * amplitude;

            enemies[i].transform.position = transform.position + new Vector3(x + i * spacing, y, 0);
        }

        // 編隊全体が画面左端を超えたら破棄
        if (enemies[enemyCount - 1] != null && enemies[enemyCount - 1].transform.position.x < -13f)
        {
            Destroy(gameObject);
        }
    }
}

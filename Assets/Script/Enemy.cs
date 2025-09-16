using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    public int hp = 1;
    public int scoreValue = 100; // この敵を倒した時のスコア

    private ScoreManager scoreManager;

    public GameObject hitEffectPrefab; // InspectorでHitEffectプレハブをセット

    // ドロップアイテム
    public GameObject bulletUpItemPrefab; // 弾数アップ
    public GameObject speedUpItemPrefab;  // スピードアップ
    [Range(0f, 1f)]
    public float dropRate = 0.9f; // 出現確率

    public GameObject player; // プレイヤーのTransformをInspectorでセット
    public float itemSpeed = 7f; // アイテム初速
    public float itemRandomAngle = 30f; // ±ランダム角度

    void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        if (player == null)
            player = GameObject.Find("Majoko");
    }

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        // 画面外に出たら削除
        if (transform.position.x < -12f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); // 弾を消す
            hp--;

            if (hp <= 0)
            {
                // ヒットエフェクト
                if (hitEffectPrefab != null)
                    Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

                // スコア加算
                UIManager ui = FindAnyObjectByType<UIManager>();
                if (ui != null)
                    ui.AddScore(scoreValue);

                // アイテムドロップ（弾数アップかスピードアップのどちらか1個）
                if (Random.value <= dropRate)
                {
                    GameObject itemToDrop = null;

                    if (Random.value < 0.7f)
                        itemToDrop = bulletUpItemPrefab;
                    else
                        itemToDrop = speedUpItemPrefab;

                    if (itemToDrop != null)
                    {
                        GameObject item = Instantiate(itemToDrop, transform.position, Quaternion.identity);
                        Destroy(item, 10f); // 自動消滅

                        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
                        if (rb != null && player != null)
                        {
                            Vector2 dir = (player.transform.position - transform.position).normalized;

                            // ±ランダム角度
                            float angle = Random.Range(-itemRandomAngle, itemRandomAngle);
                            Quaternion rot = Quaternion.Euler(0, 0, angle);
                            Vector2 finalDir = rot * dir;

                            rb.velocity = finalDir * itemSpeed;
                        }
                    }
                }

                SEManager.Instance.PlaySE(SEManager.Instance.enemyDestroySE);
                Destroy(gameObject);
            }
        }
    }
}

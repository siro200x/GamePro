using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    public int hp = 1;

    public GameObject hitEffectPrefab; // InspectorでHitEffectプレハブをセット
    public GameObject dropItemPrefab; // アイテム
    [Range(0f, 1f)]
    public float dropRate = 0.9f; // 0.3なら30％の確率でドロップ

    public GameObject player; // プレイヤーのTransformをInspectorでセット
    public float itemSpeed = 7f; // アイテム初速
    public float itemRandomAngle = 30f; // ±ランダム角度

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        // 画面外に出たら削除
        if (transform.position.x < -12f)
        {
            Destroy(gameObject);
        }
    }

    // 弾と衝突したら
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); // 弾を消す
            hp--;

            if (hp <= 0)
            {
                // エフェクトを生成
                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                }

                // アイテム出現
                if (dropItemPrefab != null && Random.value <= dropRate)
                {
                    GameObject item = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);

                    // x秒後に自動消滅
                    Destroy(item, 10f);

                    Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
                    player = GameObject.Find("Azarashi");
                    if (rb != null && player != null)
                    {
                        Debug.Log("通過中");
                        Vector2 dir = (Vector2)(transform.position - player.transform.position);// プレイヤー方向ベクトル
                        dir = dir.normalized;

                        // 逆方向
                        dir = -dir;

                        // ±ランダム角度
                        float angle = Random.Range(-itemRandomAngle, itemRandomAngle); // ±30度ランダム
                        Quaternion rot = Quaternion.Euler(0, 0, angle);
                        Vector2 finalDir = rot * dir;

                        // 速度を設定
                        rb.velocity = finalDir * itemSpeed;
                    }
                }
                Destroy(gameObject); // 敵を消す(ここで爆発エフェクトを出すと◎)
            }
        }
    }
}

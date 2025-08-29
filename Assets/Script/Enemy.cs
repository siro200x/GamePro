using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    public int hp = 1;
    public GameObject hitEffectPrefab; // InspectorでHitEffectプレハブをセット

    void Start()
    {

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

    // 弾と衝突したら
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit:" + other.name);
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); // 弾を消す
            hp--;

            // エフェクトを生成
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            if (hp <= 0)
            {
                Destroy(gameObject); // 敵を消す(ここで爆発エフェクトを出すと◎)
            }
        }
    }
}

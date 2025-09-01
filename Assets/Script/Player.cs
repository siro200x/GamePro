using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerShooting shooting; // PlayerShootingをInspectorでセット
    public GameObject newBulletPrefab; // アイテムで変わる弾

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            // ここでパワーアップ処理
            Debug.Log("アイテムゲット！");

            if (shooting != null && newBulletPrefab != null)
            {
                shooting.ChangeBullet(newBulletPrefab);
            }

            Destroy(other.gameObject);
        }
    }
}

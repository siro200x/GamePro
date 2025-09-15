using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController shooting; // PlayerShootingをInspectorでセット

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            // ここでパワーアップ処理
            Debug.Log("アイテムゲット！");

            if (shooting != null)
            {
                shooting.OnItemCollected(); // ここで追加弾を1発発射
                //shooting.ChangeBullet(newBulletPrefab);
            }

            Destroy(other.gameObject);
        }
    }
}

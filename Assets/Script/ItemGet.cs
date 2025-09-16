using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGet : MonoBehaviour
{
    public AudioClip getSE; // Inspectorで取得時に鳴らすSEを設定
    public float speed = 5f; // アイテム初速（Enemy側で設定可能）

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // SE再生
            if (getSE != null)
                AudioSource.PlayClipAtPoint(getSE, transform.position);

            // アイテム取得処理（必要に応じてスコアや回復など追加）
            Destroy(gameObject);
        }
    }
}
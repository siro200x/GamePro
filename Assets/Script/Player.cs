using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController shooting; // PlayerController をInspectorでセット

    [Header("Speed Up Settings")]
    public int maxSpeedLevel = 5;      // スピードアップ最大段階
    public float speedIncrement = 1f;  // 1段階あたりの速度増加

    private int currentSpeedLevel = 0; // 現在の速度段階

    void OnTriggerEnter2D(Collider2D other)
    {
        // 弾数アップ
        if (other.CompareTag("dropItem_P"))
        {
            if (shooting != null)
            {
                shooting.OnItemCollected(); // 追加弾数アップ
            }

            Destroy(other.gameObject);
        }
        // スピードアップ
        else if (other.CompareTag("dropItem_S"))
        {
            if (currentSpeedLevel < maxSpeedLevel && shooting != null)
            {
                shooting.moveSpeed += speedIncrement;
                currentSpeedLevel++;
                Debug.Log("スピードアップ！ 現在の速度: " + shooting.moveSpeed + " (段階: " + currentSpeedLevel + ")");
            }
            else
            {
                Debug.Log("スピードは最大段階です");
            }

            Destroy(other.gameObject);
        }
    }
}

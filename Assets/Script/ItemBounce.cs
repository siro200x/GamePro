using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBounce : MonoBehaviour
{
    public float minX = -12f, maxX = 12f; // 画面横範囲
    public float minY = 0f, maxY = 13f; // 画面縦範囲

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true; // 回転防止
    }

    void Update()
    {
        Vector2 pos = transform.position;

        // 横反射
        if (pos.x <= minX || pos.x >= maxX)
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            pos.x = Mathf.Clamp(pos.x, minX, maxX); // はみ出し防止
            transform.position = pos;
        }

        // 縦反射
        if (pos.y <= minY || pos.y >= maxY)
        {
            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        }
    }
}

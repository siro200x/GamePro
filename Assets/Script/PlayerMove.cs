using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f; // 移動スピード
    private Rigidbody2D rb; // Rigidbody2D参照
    private Vector2 moveInput; // 入力ベクトル

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // WASD入力を取得(Horizontal = A/D,Vertcal = W/S)
        float moveX = Input.GetAxisRaw("Horizontal"); // -1 ~ 1
        float moveY = Input.GetAxisRaw("Vertical"); // -1 ~ 1

        // 正規化して斜め移動の速さが不自然にならないようにする
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // Rigidbody2Dで移動
        rb.velocity = moveInput * moveSpeed;
    }
}

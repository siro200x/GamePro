using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public int lives = 3; // 残機
    public float moveSpeed = 5f;

    [Header("Respawn Settings")]
    public float respawnDelay = 1f; // 復活までの時間
    public float invincibleTime = 1f; // 無敵時間
    public Vector2 respawnPosition = new Vector2(-10f, 5f); // 復活位置

    [Header("Death/Bounce Settings")]
    public float deathUpwardForce = 8f;
    public float deathHorizontalRange = 1f;
    public float deathGravity = 2f;
    public float bounciness = 0.5f;
    public float friction = 0.4f;
    public float fallDuration = 3f;

    [Header("Shooting Setting")]
    public GameObject bulletPrefab; // 弾Prefab
    public Transform firePoint; // 弾の発射位置(子オブジェクトにEmptyを置いて指定)
    public float fireRate = 0.2f; // 連射間隔(秒)


    private bool isDead = false;
    private bool isInvincible = false;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float nextFireTime = 0f;
    private float initialMoveSpeed;
    private Quaternion initialRotation;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // 通常は重力なし

        initialMoveSpeed = moveSpeed; // 保存
        initialRotation = transform.rotation; // 初期Rotationを保存
    }

    void Update()
    {
        if (!isDead)
        {
            Move();
            Shoot();
        }
    }

    private void Move()
    {
        // 入力取得(WASD or 十字キー)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 移動ベクトル
        Vector3 move = new Vector3(moveX, moveY, 0).normalized;

        // 移動処理
        transform.position += move * moveSpeed * Time.deltaTime;

        // 画面外に出ないように制御(数値はカメラに合わせて調整)
        float clampedX = Mathf.Clamp(transform.position.x, -12f, 12f);
        float clampedY = Mathf.Clamp(transform.position.y, 0f, 13f);

        transform.position = new Vector3(clampedX, clampedY, 0f);
    }

    private void Shoot()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        }
    }
    // アイテム取得時に弾を切り替える
    public void ChangeBullet(GameObject newBullet)
    {
        bulletPrefab = newBullet;
        Debug.Log("弾の種類を変更！");
    }

    // 物理衝突で呼ばれる
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || isInvincible) return;

        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet") || collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        isDead = true;
        lives--;

        // 移動不可にする
        moveSpeed = 0;

        // 死亡演出用: 重力ON & 少し飛ばす
        rb.gravityScale = deathGravity;
        rb.constraints = RigidbodyConstraints2D.None; //　回転もOK
        rb.velocity = new Vector2(UnityEngine.Random.Range(-deathHorizontalRange, deathHorizontalRange), deathUpwardForce);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            PhysicsMaterial2D mat = new PhysicsMaterial2D();
            mat.bounciness = bounciness;
            mat.friction = friction;
            col.sharedMaterial = mat;
        }

        // 一度バウンドしてからすり抜け落下
        bool hasBounced = false;
        float timer = 0f;

        while (timer < fallDuration)
        {
            if (!hasBounced && rb.velocity.y < 0f)
            {
                hasBounced = true;
                if (col != null) col.enabled = false; // 地面Collider無効化ですり抜け
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // gameObject.SetActive(false);

        if (lives > 0)
        {
            Debug.Log("SetActive動作");
            yield return new WaitForSeconds(respawnDelay);
            Respawn();
        }
        else
        {
            Debug.Log("GAME OVER");
            // GameManagerなどに通知
        }
    }

    private void Respawn()
    {
        Debug.Log("Respawn呼ばれた");
        transform.position = respawnPosition;
        transform.rotation = initialRotation; // ここで回転を戻す
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true; // Colliderを戻す

        moveSpeed = initialMoveSpeed; // 移動速度をもとに戻す

        gameObject.SetActive(true);
        isDead = false;

        StartCoroutine(InvincibleMode());
    }

    private IEnumerator InvincibleMode()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibleTime)
        {
            sr.enabled = !sr.enabled; // 点滅
            yield return new WaitForSeconds(0.2f);
            timer += 0.2f;
        }

        sr.enabled = true;
        isInvincible = false;
    }
}

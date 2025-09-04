using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    private bool shootPressed;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float nextFireTime = 0f;
    private float initialMoveSpeed;
    private Quaternion initialRotation;
    private Vector2 inputVector;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // 通常は重力なし
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        initialMoveSpeed = moveSpeed; // 保存
        initialRotation = transform.rotation; // 初期Rotationを保存

    }


    void Update()
    {
        if (!isDead)
        {
            inputVector.x = Input.GetAxisRaw("Horizontal");
            inputVector.y = Input.GetAxisRaw("Vertical");
            Move(inputVector);
            if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
            {
                Shoot();
            }
        }
    }

    private void Move(Vector2 move)
    {
        // 入力取得(WASD or 十字キー)
        //float moveX = Input.GetAxisRaw("Horizontal");
        //float moveY = Input.GetAxisRaw("Vertical");

        // 移動ベクトル
        //Vector2 move = new Vector2(moveX, moveY);

        if (move.magnitude > 1f) move = move.normalized;
        // 移動処理
        rb.velocity = move * moveSpeed;

        Vector2 pos = rb.position;
        pos.x = Mathf.Clamp(pos.x, -12f, 12f);
        pos.y = Mathf.Clamp(pos.y, 0f, 13f);
        rb.position = pos;

        // 画面外に出ないように制御(数値はカメラに合わせて調整)
        //float clampedX = Mathf.Clamp(rb.position.x, -12f, 12f);
        //float clampedY = Mathf.Clamp(rb.position.y, 0f, 13f);

        //rb.position = new Vector2(clampedX, clampedY);
        //Debug.Log($"moveX:{moveX} moveY:{moveY} | Space:{Input.GetKey(KeyCode.Space)}");

    }

    private void Shoot()
    {
        nextFireTime = Time.time + fireRate;

        Vector2 shootDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (shootDir == Vector2.zero) shootDir = Vector2.right;

        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
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

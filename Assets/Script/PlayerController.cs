using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab; // 弾Prefab
    public Transform firePoint; // 弾の発射位置(子オブジェクトにEmptyを置いて指定)

    public float fireRate = 0.2f; // 連射間隔(秒)
    public float nextFire = 0f;

    void Start()
    {

    }

    void Update()
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

        // 弾の発射処理
        if (Input.GetKey(KeyCode.Space) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        }
    }
}

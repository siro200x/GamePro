using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{

    [Header("Boss Settings")]
    public int maxHp = 30; // 耐久値
    public float speed = 3f; // 上下移動速度
    public float topY = 10f; // 移動範囲上限
    public float bottomY = 0f; // 移動範囲下限

    [Header("Attack Settings")]
    public GameObject bulletPrefab; // ボスの弾Prefab
    public Transform firePoint; // 弾を発射する位置(Emptyオブジェクト)
    public float attackInterval = 2f; // 攻撃間隔
    public int bulletCount = 5; // 扇形弾の数
    public float spreadAngle = 60f; // 扇形角度(°)
    public float bulletSpeed = 5f; // 弾の速度

    private int currentHp;
    private int direction = 1;
    private Rigidbody2D rb;
    private float nextAttackTime = 0f;
    [HideInInspector] public bool isInvincible = true; // 初期は無敵

    public GameObject destructionEffectPrefab;

    [SerializeField] private AudioClip bossExplosionSE; // Inspectorで設定
    [SerializeField] private float seVolume = 0.5f;
    [SerializeField] private AudioClip bossHitSE;


    void Start()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    void Update()
    {
        if (currentHp <= 0) return;

        MoveUpDown();

        if (Time.time >= nextAttackTime)
        {
            ShootFanAtPlayer();
            nextAttackTime = Time.time + attackInterval;
        }
    }

    private void MoveUpDown()
    {
        Vector2 pos = rb.position;
        pos.y += speed * direction * Time.deltaTime;

        if (pos.y > topY)
        {
            pos.y = topY;
            direction = -1;
        }
        else if (pos.y < bottomY)
        {
            pos.y = bottomY;
            direction = 1;
        }

        rb.MovePosition(pos);
    }

    private void ShootFanAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 toPlayer = (player.transform.position - firePoint.position).normalized;
        float startAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        float angleStep = (bulletCount > 1) ? spreadAngle / (bulletCount - 1) : 0f;
        float angle = startAngle - spreadAngle / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D brb = bullet.GetComponent<Rigidbody2D>();
            if (brb != null) brb.velocity = dir * bulletSpeed;

            angle += angleStep;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Boss Trigger Hit: " + collision.name);
        if (collision.CompareTag("Bullet"))
        {
            if (!isInvincible)
            {
                Destroy(collision.gameObject);
                TakeDamage(1);
            }
            else
            {   // 無敵中なので弾だけ消すとか処理できる

            }
        }
    }

    private void TakeDamage(int damage)
    {
        // ダメージ分ループしてSEを鳴らす場合
        for (int i = 0; i < damage; i++)
        {
            SEManager.Instance.PlaySE(bossHitSE);
        }
        currentHp -= damage;
        Debug.Log("Boss HP:" + currentHp);
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Boss爆発演出");
        FindObjectOfType<UIManager>().AddScore(1000);

        // ボス破壊演出を開始
        BossDestructionEffect.PlayAt(gameObject, destructionEffectPrefab);

        // 3秒間、0.2秒ごとに同じ爆発SEを連打
        StartCoroutine(PlayBossExplosionSE(5f, 1f));

        GameManager.Instance.GameClear();
    }

    private IEnumerator PlayBossExplosionSE(float duration, float interval)
    {
        float timer = 0f;
        while (timer < duration)
        {
            SEManager.Instance.PlaySE(bossExplosionSE); // 同じクリップを再生
            yield return new WaitForSeconds(interval);
            timer += interval;
        }
    }

    //★ BossEventControllerから呼ぶ用のメソッド
    public void RemoveInvincible()
    {
        isInvincible = false;
        Debug.Log("Boss無敵解除!");
    }
}


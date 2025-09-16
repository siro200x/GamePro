using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public int lives = 3;
    public float moveSpeed = 5f;

    [Header("Respawn Settings")]
    public float respawnDelay = 1f;
    public float invincibleTime = 1f;
    public Vector2 respawnPosition = new Vector2(-10f, 5f);

    [Header("Death/Bounce Settings")]
    public float deathUpwardForce = 8f;
    public float deathHorizontalRange = 1f;
    public float deathGravity = 2f;
    public float bounciness = 0.5f;
    public float friction = 0.4f;
    public float fallDuration = 3f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;

    private bool isDead = false;
    private bool isInvincible = false;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;
    private float nextFireTime = 0f;
    private float initialMoveSpeed;
    private Quaternion initialRotation;
    private Vector2 inputVector;
    private bool hasExtraBullet = false;
    private int extraBullets = 0; // 追加弾の数

    // キャッシュ
    private UIManager uiManager;
    private BGMManager bgmManager;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        initialMoveSpeed = moveSpeed;
        initialRotation = transform.rotation;

        // キャッシュ
        uiManager = FindObjectOfType<UIManager>();
        bgmManager = FindObjectOfType<BGMManager>();

        bgmManager?.PlayStageBGM();
    }

    void Update()
    {
        if (isDead) return;

        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");

        Move(inputVector);

        // 左クリックで通常弾
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
        }

        // Animator 更新
        if (animator != null)
        {
            bool leftClickPressed = Input.GetMouseButton(0);
            animator.SetBool("IsClickAnim", leftClickPressed);
            animator.SetFloat("MoveX", inputVector.x);
            animator.SetFloat("MoveY", inputVector.y);
        }
    }

    private void Move(Vector2 move)
    {
        if (move.magnitude > 1f) move = move.normalized;
        rb.velocity = move * moveSpeed;

        Vector2 pos = rb.position;
        pos.x = Mathf.Clamp(pos.x, -12f, 12f);
        pos.y = Mathf.Clamp(pos.y, 0f, 13f);
        rb.position = pos;
    }

    private void Shoot()
    {
        nextFireTime = Time.time + fireRate;

        // 通常弾
        Vector2 shootDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (shootDir == Vector2.zero) shootDir = Vector2.right;

        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));

        // 追加弾を発射（アイテム取得数に応じて）
        for (int i = 0; i < extraBullets; i++)
        {
            float randomAngle = Random.Range(-30f, 30f); // ±30°ランダム
            Vector2 extraDir = Quaternion.Euler(0, 0, randomAngle) * Vector2.right;
            float extraAngle = Mathf.Atan2(extraDir.y, extraDir.x) * Mathf.Rad2Deg;

            Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, extraAngle));
        }
    }

    // アイテム取得時に弾数を増やす
    public void OnItemCollected()
    {
        extraBullets++; // 弾数+1
        Debug.Log("追加弾を取得！ 現在の追加弾数: " + extraBullets);
    }

    // public void ChangeBullet(GameObject newBullet)
    // {
    //     bulletPrefab = newBullet;
    //     Debug.Log("弾の種類を変更！");
    // }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || isInvincible) return;

        if (collision.gameObject.CompareTag("Enemy") ||
            collision.gameObject.CompareTag("EnemyBullet") ||
            collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        // 死亡SE再生
        SEManager.Instance.PlaySE(SEManager.Instance.playerDeathSE);

        isDead = true;
        lives--;

        moveSpeed = 0;
        uiManager?.LoseLife();

        rb.gravityScale = deathGravity;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = new Vector2(Random.Range(-deathHorizontalRange, deathHorizontalRange), deathUpwardForce);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            PhysicsMaterial2D mat = new PhysicsMaterial2D { bounciness = bounciness, friction = friction };
            col.sharedMaterial = mat;
        }

        bool hasBounced = false;
        float timer = 0f;
        while (timer < fallDuration)
        {
            if (!hasBounced && rb.velocity.y < 0f)
            {
                hasBounced = true;
                if (col != null) col.enabled = false;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (lives > 0)
        {
            yield return new WaitForSeconds(respawnDelay);
            Respawn();
        }
        else
        {
            GameManager.Instance.GameOver(false);
        }
    }

    public void Respawn()
    {
        transform.position = respawnPosition;
        transform.rotation = initialRotation;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        isDead = false;
        moveSpeed = initialMoveSpeed;

        // 復活SE再生
        SEManager.Instance.PlaySE(SEManager.Instance.playerRespawnSE);

        StartCoroutine(InvincibleMode());
    }

    private IEnumerator InvincibleMode()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibleTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.2f);
            timer += 0.2f;
        }

        sr.enabled = true;
        isInvincible = false;
    }
}

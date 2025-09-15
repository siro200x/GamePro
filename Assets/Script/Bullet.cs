using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;      // 弾のスピード
    public float lifeTime = 3f;    // 自動消滅時間
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 発射方向に進む（生成時の回転を反映）
        rb.velocity = transform.right * speed;

        // 一定時間後に自動消滅
        Destroy(gameObject, lifeTime);
    }

    // 画面外に出たら削除
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

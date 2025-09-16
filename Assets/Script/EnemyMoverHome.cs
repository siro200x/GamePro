using UnityEngine;

public class EnemyMoverHome : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;             // 移動速度
    public float rushDuration = 1f;      // プレイヤーに向かう突撃時間
    public float destroyMarginX = 12f;   // 画面外判定 X方向
    public float destroyMarginY = 7f;    // 画面外判定 Y方向

    private Transform player;
    private Vector3 direction;
    private float timer = 0f;
    private bool isRushing = true;

    void Start()
    {
        // Player を取得
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 出現時は突撃方向を設定
        if (player != null)
            direction = (player.position - transform.position).normalized;
        else
            direction = Vector3.left; // プレイヤー不在時は左
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 突撃中は常にプレイヤーの現在位置を狙う
        if (isRushing && player != null)
        {
            direction = (player.position - transform.position).normalized;

            // 突撃時間終了
            if (timer >= rushDuration)
            {
                isRushing = false;
                timer = 0f; // 次の用途のためにリセット
            }
        }

        // 移動
        transform.position += direction * speed * Time.deltaTime;

        // 突撃終了後のみ画面外判定で消す
        if (!isRushing)
        {
            if (transform.position.x < -destroyMarginX || transform.position.x > destroyMarginX ||
                transform.position.y < -destroyMarginY || transform.position.y > destroyMarginY)
            {
                Destroy(gameObject);
            }
        }
    }
}

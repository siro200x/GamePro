using UnityEngine;

public class EnemyMoverZ : MonoBehaviour
{
    public float speed = 2f;

    private int phase = 0; // 0=左直進, 1=右下, 2=左直進
    private Vector3 direction;

    void Start()
    {
        direction = Vector3.left; // 初期は左直進
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        switch (phase)
        {
            case 0: // 左直進
                if (transform.position.x < -7f)
                {
                    phase = 1;
                    direction = new Vector3(3f, -1f, 0); // 右下へ
                }
                break;

            case 1: // 右下直進
                if (transform.position.x > 1f)
                {
                    phase = 2;
                    direction = Vector3.left; // 左直進
                }
                break;

            case 2: // 左直進 → 画面外で消去
                if (transform.position.x < -12f)
                    Destroy(gameObject);
                break;
        }
    }
}

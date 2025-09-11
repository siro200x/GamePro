using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public float speed = 2f; // スクロール速度
    private float spriteWidth;
    private Camera mainCam;

    private Transform[] groundPieces;

    void Start()
    {
        mainCam = Camera.main;

        // 子オブジェクト全部を「地面ピース」として扱う
        groundPieces = new Transform[transform.childCount];
        for (int i = 0; i < groundPieces.Length; i++)
        {
            groundPieces[i] = transform.GetChild(i);
        }

        // 1枚の幅を取得（全部同じ想定）
        spriteWidth = groundPieces[0].GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // カメラの左端
        float cameraLeft = mainCam.transform.position.x - mainCam.orthographicSize * mainCam.aspect;

        foreach (Transform piece in groundPieces)
        {
            // 左に移動
            // piece.Translate(Vector2.left * speed * Time.deltaTime);
            Vector3 pos = piece.position;
            pos.x -= speed * Time.deltaTime;

            // このピースの右端
            float rightEdge = piece.position.x + spriteWidth / 2f;

            // 画面外に完全に出たら、一番右に移動
            if (rightEdge < cameraLeft)
            {
                // 一番右のピースを探す
                float maxX = float.MinValue;
                foreach (Transform p in groundPieces)
                {
                    if (p != piece && p.position.x > maxX) maxX = p.position.x;
                }

                // その右端に並べる
                // Vector3 pos = piece.position;
                pos.x = maxX + spriteWidth;

            }
            piece.position = pos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEventController : MonoBehaviour
{
    public float eventTime = 30f; // 発動タイミング

    public Image fadeImage; // フェード用UI Image
    public float fadeDuration = 3f; // フェードにかける時間
    public float darkHoldDuration = 3f; // 暗転状態を維持する時間

    public Renderer background; // 背景SpriteRenderer
    public Material bossMaterial; // ボス用マテリアル
    public BackgroundScroller bgScroll; // 背景スクロール

    public GameObject bossPrefab; // ボスPrefab
    public Transform bossTargetPos; // ボス停止位置
    public float bossMoveSpeed = 2f; // ボスの出現速度

    public WaveManager waveManager; // 雑魚wave管理

    private bool eventStarted = false;
    private float timer = 0f;
    private GameObject bossInstance;

    void Update()
    {
        timer += Time.deltaTime;

        if (!eventStarted && timer >= eventTime)
        {
            eventStarted = true;
            StartCoroutine(BossSequence());
        }
    }

    IEnumerator BossSequence()
    {
        // 雑魚Waveと背景スクロールを停止
        if (waveManager != null)
        {
            waveManager.gameObject.SetActive(false);
            Debug.Log("waveManager親オブジェクト無効化!子も停止");
        }
        // フェードアウト
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 暗転状態を維持
        yield return new WaitForSeconds(darkHoldDuration);

        // 背景差し替え
        if (background != null && bossMaterial != null)
        {
            // ボス用マテリアルに切り替え
            Debug.Log("ボス背景切り替え");
            background.sharedMaterial = bossMaterial;

            // MaterialのOffsetとScaleを初期化(完全静止用)
            background.sharedMaterial.mainTextureOffset = Vector2.zero;
            background.sharedMaterial.mainTextureScale = Vector2.one;
        }

        // ボスを右外から生成
        if (bossPrefab != null && bossTargetPos != null)
        {
            Vector3 spawnPos = bossTargetPos.position + new Vector3(10f, 0, 0);
            bossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

            //★ ボス登場中は行動停止&無敵状態 ★
            var bossEnemy = bossInstance.GetComponent<BossEnemy>();
            if (bossEnemy != null)
            {
                bossEnemy.enabled = false;
                bossEnemy.isInvincible = true;
            }

            //var colliders = bossInstance.GetComponentsInChildren<Collider2D>();
            //foreach (var col in colliders) col.enabled = false;
        }
        // フェードイン(3秒かけて新背景が現れる)
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // フェードイン&移動終了後に有効化
        if (bossInstance != null)
        {
            while (Vector3.Distance(bossInstance.transform.position, bossTargetPos.position) > 0.05f)
            {
                bossInstance.transform.position = Vector3.MoveTowards(
                    bossInstance.transform.position,
                    bossTargetPos.position,
                    Time.deltaTime * bossMoveSpeed
                );
                yield return null;
            }
            //★ 移動完了後に行動開始&当たり判定オン ★
            var bossEnemy = bossInstance.GetComponent<BossEnemy>();
            if (bossEnemy != null)
            {
                bossEnemy.enabled = true; //AI開始
                bossEnemy.RemoveInvincible(); // 無敵解除
            }

            // var colliders = bossInstance.GetComponentsInChildren<Collider2D>(true);
            //foreach (var col in colliders)
            //{
            //   col.enabled = true;
            //}

            Debug.Log("ボス行動開始!");
        }
        // ボス到達後、必要ならWaveやスクロール再開も可能
        // waveManager.enabled = true;
        // bgScroll.enabled =true;
        // Debug.Log("ボス戦開始!");
    }
}

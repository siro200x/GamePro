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
        if (waveManager != null) waveManager.enabled = false;
        // if (bgScroll != null) bgScroll.enabled = false;

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

            // 背景スクロースを停止
            //if (bgScroll != null)
            //{
            //    bgScroll.enabled = false; // Updateが止まる
            //}
        }

        // ボスを右外から生成
        if (bossPrefab != null && bossTargetPos != null)
        {
            Vector3 spawnPos = bossTargetPos.position + new Vector3(10f, 0, 0);
            bossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
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

        // ボスをゆっくり移動
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
        }
        // ボス到達後、必要ならWaveやスクロール再開も可能
        // waveManager.enabled = true;
        // bgScroll.enabled =true;
        // Debug.Log("ボス戦開始!");
    }
}

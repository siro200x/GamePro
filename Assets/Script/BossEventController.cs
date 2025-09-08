using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEventController : MonoBehaviour
{
    public float eventTime = 30f; // 発動タイミング

    public Image fadeImage; // フェード用UI Image
    public float fadeSpeed = 1.5f; // フェード速度

    public SpriteRenderer background; // 背景SpriteRenderer
    public Sprite bossBackground; // 差し替え用背景
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
        if (bgScroll != null) bgScroll.enabled = false;

        // フェードアウト
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 背景差し替え
        if (background != null && bossBackground != null)
        {
            background.sprite = bossBackground;
            background.transform.position = Vector3.zero;
        }

        // ボスを右外から生成
        if (bossPrefab != null && bossTargetPos != null)
        {
            Vector3 spawnPos = bossTargetPos.position + new Vector3(10f, 0, 0);
            bossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        }
        // フェードイン
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
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
                    Time.deltaTime * 2f
                );
                yield return null;
            }
        }
        // ボス到達後、必要ならWaveやスクロール再開も可能
        // waveManager.enabled = true;
        // bgScroll.enabled =true;
        Debug.Log("ボス戦開始!");
    }
}

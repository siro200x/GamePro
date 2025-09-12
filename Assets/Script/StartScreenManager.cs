using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform titleImage; // 上から降ってくる画像
    public Button startButton; // スタートボタン
    public string gameSceneName = "GameScene"; //遷移したいシーン名

    [Header("Animation Settings")]
    public float dropDistance = 500f; // 降下速度
    public float dropDuration = 3f; // 落下時間
    public float bounceAmount = 50f; //弾む量
    public float bounceDuration = 0.6f; //弾み高さ
    public float buttonFadeDuration = 1f;

    private RectTransform titleRect;
    private CanvasGroup buttonCanvasGroup;

    void Start()
    {
        if (titleImage == null || startButton == null)
        {
            Debug.LogError("Title Image またはStart Button がセットされていません!");
            return;
        }

        titleRect = titleImage;

        //canvas取得
        Canvas canvas = titleRect.GetComponentInParent<Canvas>();
        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

        // TitleImageを画面上の外へ配置
        titleRect.anchoredPosition = new Vector2(titleRect.anchoredPosition.x, canvasHeight / 2 + titleRect.rect.height / 2);

        //ボタンは非表示
        buttonCanvasGroup = startButton.GetComponent<CanvasGroup>();
        if (buttonCanvasGroup == null)
        {
            buttonCanvasGroup = startButton.gameObject.AddComponent<CanvasGroup>();
        }
        buttonCanvasGroup.alpha = 0f;
        startButton.interactable = false;

        // ボタン押下時の処理を登録
        startButton.onClick.AddListener(OnStartButtonClicked);

        StartCoroutine(DelayedTitleAnimation(1f, canvasHeight));
    }

    private IEnumerator DelayedTitleAnimation(float delaySeconds, float canvasHeight)
    {
        yield return new WaitForSecondsRealtime(delaySeconds); // 1秒待機
        yield return StartCoroutine(PlayTitleAnimation(canvasHeight)); // 元のアニメーション開始
    }
    private IEnumerator PlayTitleAnimation(float canvasHeight)
    {
        Vector2 startPos = titleRect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, 0f); // 中央まで落とす場合

        float timer = 0f;

        // 下に落ちるアニメーション
        while (timer < dropDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, timer / dropDuration);
            titleImage.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        titleRect.anchoredPosition = targetPos;

        // 弾むアニメーション
        timer = 0f;
        Vector2 bouncePos = targetPos + Vector2.up * bounceAmount;
        while (timer < bounceDuration)
        {
            float t = Mathf.Sin((timer / bounceDuration) * Mathf.PI); // sinで自然な弾み 
            titleRect.anchoredPosition = Vector2.Lerp(targetPos, bouncePos, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        titleRect.anchoredPosition = targetPos;

        // 3.ボタンフェードイン
        timer = 0f;
        while (timer < buttonFadeDuration)
        {
            buttonCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / buttonFadeDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        buttonCanvasGroup.alpha = 1f;
        startButton.interactable = true;
    }

    private void OnStartButtonClicked()
    {
        // シーン制御
        Debug.Log("Start Button Clicked!");
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("GameSceneName がセットされていません！");
        }
    }
}
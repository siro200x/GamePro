using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject gameOverUI;
    public GameObject clearUI;
    public TextMeshProUGUI clearMessage;
    public GameObject titleButton;

    private bool isCleared = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        gameOverUI.SetActive(false);
        clearUI.SetActive(false);
    }

    // GameOver処理
    public void GameOver(bool revive)
    {
        if (isCleared) return;

        if (revive)
        {
            Time.timeScale = 1f;
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null) player.Respawn();
            gameOverUI.SetActive(false);
        }
        else
        {
            // 完全に終了 → タイトルに戻す
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
            StartCoroutine(ReturnToTitleCoroutine());
        }
    }

    // GiveUpボタン専用
    public void GiveUp()
    {
        GameOver(false); // タイトルへ
    }

    // リスタートボタン専用
    public void Restart()
    {
        // タイトルに戻る処理を止める
        StopAllCoroutines();

        Time.timeScale = 1f;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) player.Respawn();

        gameOverUI.SetActive(false);
    }

    // クリア処理
    public void GameClear()
    {
        if (isCleared) return;
        isCleared = true;

        WaveManager wave = FindObjectOfType<WaveManager>();
        if (wave != null) wave.gameObject.SetActive(false);

        var bossEvent = FindObjectOfType<BossEventController>();
        if (bossEvent != null) bossEvent.enabled = false;

        StartCoroutine(ShowClearCoroutine());
    }

    private IEnumerator ShowClearCoroutine()
    {
        yield return new WaitForSeconds(3f);

        PlayerController player = FindObjectOfType<PlayerController>();
        FindObjectOfType<BGMManager>().PlayEndingBGM();

        if (player != null) player.enabled = false;

        if (clearUI != null) clearUI.SetActive(true);
        if (clearMessage != null)
            clearMessage.text = "\n\nCongratulations!!\nThank you for playing my game!";

        if (titleButton != null) titleButton.SetActive(true);
    }

    private IEnumerator ReturnToTitleCoroutine()
    {
        yield return new WaitForSecondsRealtime(1f); // Time.timeScale=0でも待機可能
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene"); // タイトルシーン名に置き換える
    }
}

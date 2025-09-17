using System.Collections;
using System.Collections.Generic;
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

        //Time.timeScale = 1f;
    }

    // GameOver処理
    public void GameOver(bool revive)
    {
        if (isCleared) return;
        if (revive)
        {
            Time.timeScale = 1f;
            // 復活処理(例:PlayerRespawnを呼ぶ)
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null) player.Respawn();
            gameOverUI.SetActive(false);
        }
        else
        {
            // 完全に終了
            Time.timeScale = 0f; // 全停止
            gameOverUI.SetActive(true);

            // タイトルに戻す
            StartCoroutine(ReturnToTitleCoroutine());
        }
    }

    // GiveUpボタン専用メソッド
    public void GiveUp()
    {
        GameOver(false);
    }

    // クリア処理
    public void GameClear()
    {
        if (isCleared) return;
        isCleared = true;

        // WaveManagerをオフにする
        if (Instance != null)
        {
            WaveManager wave = FindObjectOfType<WaveManager>();
            if (wave != null)
            {
                wave.gameObject.SetActive(false);
            }
        }
        // ★ BossEventController を止める
        var bossEvent = FindObjectOfType<BossEventController>();
        if (bossEvent != null) bossEvent.enabled = false;

        StartCoroutine(ShowClearCoroutine());
    }

    private System.Collections.IEnumerator ShowClearCoroutine()
    {
        yield return new WaitForSeconds(3f); // 3秒待つ

        // プレイヤーだけ止める
        PlayerController player = FindObjectOfType<PlayerController>();
        FindObjectOfType<BGMManager>().PlayEndingBGM();

        if (player != null) player.enabled = false;

        // UIを表示
        if (clearUI != null) clearUI.SetActive(true);
        if (clearMessage != null)
            clearMessage.text = "\n\nCongratulations!!\nThank you for playing my game!";

        // タイトルボタン表示
        if (titleButton != null) titleButton.SetActive(true);
    }

    private IEnumerator ReturnToTitleCoroutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }
}

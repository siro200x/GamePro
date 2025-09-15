using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject gameOverUI;
    public GameObject clearUI;
    public TextMeshProUGUI clearMessage;

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
        if(isCleared) return;
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
        }
    }

    // クリア処理
    public void GameClear()
    {
        if(isCleared) return;
        isCleared = true;
        StartCoroutine(ShowClearCoroutine());
    }

    private System.Collections.IEnumerator ShowClearCoroutine()
    {
        yield return new WaitForSeconds(3f); // 3秒待つ

        // プレイヤーだけ止める
        PlayerController player = FindObjectOfType<PlayerController>();
        FindObjectOfType<BGMManager>().PlayEndingBGM();

        if (player != null) player.enabled = false;
        Debug.Log("ここまでキてる");

        // UIを表示
        if (clearUI != null) clearUI.SetActive(true);
        if (clearMessage != null)
            clearMessage.text = "\n\nCongratulations!!\nThank you for playing my game!";
    }
}

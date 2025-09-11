using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // canvas内のscoreTextを割り当て
    private int score = 0; // 現在のスコア

    void Start()
    {
        UpdateScoreText();
    }

    // スコア加算用メソッド
    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
    }

    // スコア表示更新
    private void UpdateScoreText()
    {
        scoreText.text = "SCORE:" + score.ToString();
    }
}

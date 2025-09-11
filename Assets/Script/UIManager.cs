using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText; //スコア表示用
    public TMP_Text livesText; //残機表示用

    private int score = 0;
    private int lives = 3;

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public void LoseLife()
    {
        lives--;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score:" + score;
        if (livesText != null) livesText.text = "Lives:" + lives;
    }
}

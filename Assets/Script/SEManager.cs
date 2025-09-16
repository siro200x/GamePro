using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance;
    public AudioSource seSource;
    public AudioClip playerDeathSE;
    public AudioClip playerRespawnSE;

    [Header("Common SE Clips")]
    public AudioClip enemyDestroySE; // 雑魚敵共通
    public AudioClip bossDestroySE; // ボス専用
    public float bossSEVolume = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip != null) seSource.PlayOneShot(clip);
    }
    public void PlayBossSE()
    {
        seSource.PlayOneShot(bossDestroySE, bossSEVolume);
    }
}

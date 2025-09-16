using System.Collections;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [Header("AudioSource & Clips")]
    public AudioSource audioSource;
    public AudioClip stageBGM;
    public AudioClip bossBGM;
    public AudioClip endingBGM;
    [Range(0f, 1f)] public float maxVolume = 0.5f;

    [Header("Fade Settings")]
    public float fadeDuration = 1f; // フェード時間（秒）

    void Start()
    {
        PlayStageBGM();
    }

    // ステージBGM再生
    public void PlayStageBGM()
    {
        StartCoroutine(FadeToClip(stageBGM, true));
    }

    // ボスBGM再生
    public void PlayBossBGM()
    {
        StartCoroutine(FadeToClip(bossBGM, true));
    }

    // エンディングBGM再生
    public void PlayEndingBGM()
    {
        StartCoroutine(FadeToClip(endingBGM, false));
    }

    // フェード処理
    private IEnumerator FadeToClip(AudioClip newClip, bool loop)
    {
        if (audioSource.isPlaying)
        {
            // フェードアウト
            float startVolume = audioSource.volume;
            float timer = 0f;
            while (timer < fadeDuration)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            audioSource.volume = 0f;
            audioSource.Stop();
        }

        // クリップ切替 & 再生
        audioSource.clip = newClip;
        audioSource.loop = loop;
        audioSource.Play();

        // フェードイン
        float t = 0f;
        while (t < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(0f, maxVolume, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = maxVolume;
    }
}

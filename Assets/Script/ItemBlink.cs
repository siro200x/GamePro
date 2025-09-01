using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlink : MonoBehaviour
{
    public float blinkSpeed = 3f; // 点滅の速さ
    public float minAlpha = 0.5f; // 最小の透明度(0=完全に消える)
    public float maxAlpha = 1f; // 最大の透明度(1=完全に不透明)

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // α値をサイン波で変化(0~1の間を往復)
        float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;

        // αをminAlpha~maxAlphaの間で変化
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}

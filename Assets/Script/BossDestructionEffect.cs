using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDestructionEffect : MonoBehaviour
{
    public GameObject smallExplosionPrefab;
    public float duration = 3f;
    public float explosionInterval = 0.3f;
    public float explosionRadius = 0.5f;

    public static void PlayAt(GameObject boss, GameObject effectPrefab)
    {
        if (boss == null || effectPrefab == null) return;

        // BossDestructionEffect プレハブから生成
        GameObject effectObj = Instantiate(effectPrefab);
        effectObj.SetActive(true);

        var effect = effectObj.GetComponent<BossDestructionEffect>();
        if (effect != null)
        {
            effect.StartCoroutine(effect.DestructionSequence(boss));
        }
    }

    // public void PlayDestruction(GameObject boss)
    // {
    //     StartCoroutine(DestructionSequence(boss));
    // }

    private IEnumerator DestructionSequence(GameObject boss)
    {
        // var sr = boss.GetComponent<SpriteRenderer>();
        if (boss == null) yield break;

        SpriteRenderer sr = boss.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Vector2 size = sr.bounds.size; // ボス画像の幅・高さ
        float t = 0f;
        float nextExplosion = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            if (boss == null) break;

            // ランダム位置に爆発
            if (t >= nextExplosion)
            {
                nextExplosion += explosionInterval;
                if (smallExplosionPrefab != null)
                {

                    Vector3 randomPos = boss.transform.position +
                                        new Vector3(
                                            Random.Range(-size.x / 2f, size.x / 2f),
                                            Random.Range(-size.y / 2f, size.y / 2f),
                                            0f
                                        );


                    GameObject explosion = Instantiate(smallExplosionPrefab, randomPos, Quaternion.identity);
                    Destroy(explosion, 2f);
                }
            }
            // 透明化
            if (sr != null)
            {
                float alpha = Mathf.Clamp01(1f - (t / duration));
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

                yield return null;
            }
        }
        if (boss != null) Destroy(boss);
        Destroy(gameObject);
    }
}

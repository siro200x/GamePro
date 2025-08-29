using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // 弾のスピード
    public float lifeTime = 3f; // 自動消滅時間

    void Start()
    {
        Destroy(gameObject, lifeTime); // 一定時間後に消す
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}

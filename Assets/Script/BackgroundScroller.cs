using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // 時間経過でUVをずらす
        float offset = Time.time * scrollSpeed;
        mat.mainTextureOffset = new Vector2(offset, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObject : MonoBehaviour
{
    public float speed = 1.0f;
    public float startPosition;
    public float endPosition;

    void Start()
    {

    }

    void Update()
    {
        // 毎フレームxポジションを少しずつ移動させる
        transform.Translate(-1 * speed * Time.deltaTime, 0, 0);

        // スクロールが目標ポイントまで到達したかをチェック
        if (transform.position.x <= endPosition) ScrollEnd();
    }

    void ScrollEnd()
    {
        // Debug.Log(transform.position.x);
        // 通り過ぎた分を加味してポジションを再設定
        float diff = transform.position.x - endPosition;
        Vector3 restartPosition = transform.position;
        restartPosition.x = startPosition + diff;
        // restartPosition.x = startPosition;
        transform.position = restartPosition;

        // 同じゲームオブジェクトにアタッチされているコンポーネントにメッセージを送る
        SendMessage("OnScrollEnd", SendMessageOptions.DontRequireReceiver);
    }
}

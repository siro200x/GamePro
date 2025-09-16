using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    public void GoToTitle()
    {
        SceneManager.LoadScene("TitleScene"); // タイトルシーン名を指定
    }
}


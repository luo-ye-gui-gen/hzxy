using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseButton : MonoBehaviour
{
    public static PauseButton instance;
    [Header("暂停相关按钮")]
    public Button pauseButton;     // 暂停游戏按钮（TMPro Button）

    void Awake()
    {
        if (instance == null && instance != this)
            instance = this;
        else
            Destroy(gameObject);

    }

    /// <summary>
    /// 暂停游戏按钮点击事件
    /// </summary>
    public void OnPauseButtonClick()
    {
        if (!GameManager.instance.isGamePaused)
        {
            GameManager.instance.Pause();
        }
    }
}

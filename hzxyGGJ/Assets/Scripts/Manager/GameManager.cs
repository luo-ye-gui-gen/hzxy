using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerHealth playerHealth;
    public bool isGamePaused = false;
    public bool isIEnumerator = false;

    void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
    }

    private void Update()
    {
        GamePause();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
    }

    public void GamePause()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !isIEnumerator)
        {
            Pause();
        }
    }

    public void Pause()
    {
        //AudioListener.pause = pause;后续用
        
        isGamePaused = !isGamePaused;
        //继续
        if (!isGamePaused) StartCoroutine(CountdownToResume());
        //暂停
        if (isGamePaused)
        {
            ResumeButton.instance.resumeButton.gameObject.SetActive(isGamePaused);
            PauseButton.instance.gameObject.SetActive(!isGamePaused);
            Time.timeScale = isGamePaused ? 0 : 1;
        }
    }

    /// <summary>
    /// 3秒倒计时协程
    /// </summary>
    IEnumerator CountdownToResume()
    {
        Debug.Log("进入携程");
        isIEnumerator = true;
        ResumeButton.instance.countdownText.gameObject.SetActive(true);
        ResumeButton.instance.resumeButton.gameObject.SetActive(isGamePaused);
        int countdown = 3;
        while (countdown > 0)
        {
            // 更新倒计时文本
            if (ResumeButton.instance.countdownText != null)
            {
                ResumeButton.instance.countdownText.text = countdown.ToString();
            }
            // 等待1秒
            yield return new WaitForSecondsRealtime(1);
            countdown--;
        }

        //倒计时结束，正式恢复游戏
        ResumeButton.instance.countdownText.text = "GO!";
        // 显示"GO!"后再隐藏文本
        yield return new WaitForSecondsRealtime(0.5f);
        ResumeButton.instance.countdownText.gameObject.SetActive(false);
        PauseButton.instance.gameObject.SetActive(!isGamePaused);
        Time.timeScale = isGamePaused ? 0 : 1;
        isIEnumerator = false;
    }
}

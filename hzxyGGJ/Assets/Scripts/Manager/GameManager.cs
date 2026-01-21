using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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

    private void OnEnable()
    {
        ResetGameState();
    }

    private void Update()
    {
        GamePause();
    }


    public void GameOver()
    {
        RestartButton.instance.restartButton.gameObject.SetActive(true);
        ResumeButton.instance.resumeButton.gameObject.SetActive(false);
        BackButton.instance.backButton.gameObject.SetActive(true);
        isGamePaused = true;
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
        isGamePaused = !isGamePaused;
        ScoreManager.Instance.isGameRunning = !ScoreManager.Instance.isGameRunning;
        //变继续
        if (!isGamePaused) StartCoroutine(CountdownToResume());
        //变暂停
        if (isGamePaused)
        {
            ResumeButton.instance.resumeButton.gameObject.SetActive(isGamePaused);
            RestartButton.instance.restartButton.gameObject.SetActive(isGamePaused);
            BackButton.instance.backButton.gameObject.SetActive(isGamePaused);
            PauseButton.instance.pauseButton.gameObject.SetActive(!isGamePaused);
            Time.timeScale = isGamePaused ? 0 : 1;
        }
    }

    /// <summary>
    /// 3秒倒计时协程
    /// </summary>
    IEnumerator CountdownToResume()
    {
        AudioManager.instance.PlaySFX(1,null);
        isIEnumerator = true;
        ResumeButton.instance.resumeButton.gameObject.SetActive(isGamePaused);
        ResumeButton.instance.countdownText.gameObject.SetActive(true);
        RestartButton.instance.restartButton.gameObject.SetActive(isGamePaused);
        BackButton.instance.backButton.gameObject.SetActive(isGamePaused);
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

    public void ResetGameState()
    {
        ScoreManager.Instance.ResetScore();
        playerHealth.ResetHealth();
        isGamePaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
    }
}

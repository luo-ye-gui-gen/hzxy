using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource[] sfx;
    public AudioSource[] bgm;
    
    [SerializeField] private float sfxMinimunDistance;

    public bool playBGM;
    private int bgmIndex;

    // 记录当前正在播放的BGM音量（用于渐变）
    private float currentBGMVolume = 0.2f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // 初始化BGM音量
        if (bgm.Length > 0)
        {
            foreach (var audio in bgm)
            {
                audio.volume = 0;
                audio.loop = true; // 设置BGM循环播放
            }
            // 默认播放bgm0并设置初始音量
            if (bgm.Length > 0)
            {
                bgm[0].volume = currentBGMVolume;
                bgm[0].Play();
                bgmIndex = 0;
            }
        }
    }

    void Update()
    {
    }

    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].Play();
        }
    }

    public void StopSFX(int index) => sfx[index].Stop();

    public void StopSFXWithTime(int _index) => StartCoroutine(DecreaseVolume(sfx[_index]));

    IEnumerator DecreaseVolume(AudioSource _audio)
    {
        float defaultVolume = _audio.volume;

        while (_audio.volume > .1f)
        {
            _audio.volume = _audio.volume * .2f;
            yield return new WaitForSeconds(.6f);

            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }

    // 新增：BGM音量渐小协程
    public IEnumerator FadeOutBGM(AudioSource audioSource, float fadeTime = 1f)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeTime);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    // 新增：BGM音量渐大协程
    public IEnumerator FadeInBGM(AudioSource audioSource, float targetVolume = 0.2f, float fadeTime = 1f)
    {
        audioSource.volume = 0f;
        audioSource.Play();
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsedTime / fadeTime);
            yield return null;
        }

        audioSource.volume = targetVolume;
        currentBGMVolume = targetVolume;
    }

    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;
        // 不再直接停止所有BGM，改为通过渐变控制
        if (bgmIndex < bgm.Length)
        {
            StartCoroutine(FadeInBGM(bgm[bgmIndex], 0.2f, 2f)); // 2秒渐大到0.2音量
        }
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
            bgm[i].volume = 0;
        }
    }

    // 新增：停止指定BGM（带渐隐效果）
    public void StopBGMWithFade(int index, float fadeTime = 1f)
    {
        if (index < bgm.Length)
        {
            StartCoroutine(FadeOutBGM(bgm[index], fadeTime));
        }
    }
}
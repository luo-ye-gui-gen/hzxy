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

    // 第一首BGM的初始音量（直接设置为0.8f）
    private float bgm0InitVolume = 1f;
    // 记录当前正在播放的BGM音量（用于渐变）
    private float currentBGMVolume;

    // 标记是否正在进行900-1000分的音量渐变
    private bool isInScoreVolumeFade = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        bgmIndex = 0;
        
        // 初始化BGM0的音量为0.8f（无渐变，直接固定）
        if (bgm.Length > 0)
        {
            bgm[0].volume = bgm0InitVolume;
            currentBGMVolume = bgm0InitVolume;
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

    // 900-1000分区间BGM0音量从0.8f渐变到0.4f的协程
    public IEnumerator FadeBGM0To04DuringScoreRange(float fadeTime = 1f)
    {
        isInScoreVolumeFade = true;
        AudioSource bgm0 = bgm[0];
        float startVolume = bgm0InitVolume; // 从初始0.8f开始
        float targetVolume = 0.4f;          // 渐变到0.4f
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            bgm0.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeTime);
            yield return null;
        }

        bgm0.volume = targetVolume;
        currentBGMVolume = targetVolume;
    }

    // BGM音量渐小协程（直接渐到0）
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
        isInScoreVolumeFade = false;
    }

    // BGM1专用渐入协程（从0.08f渐变到0.2f）
    public IEnumerator FadeInBGM1(float fadeTime = 2f)
    {
        AudioSource bgm1 = bgm[1];
        float startVolume = 0.2f;  
        float targetVolume = 0.9f; 
        bgm1.volume = startVolume;
        bgm1.Play();

        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            bgm1.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeTime);
            yield return null;
        }
        bgm1.volume = targetVolume;
        currentBGMVolume = targetVolume;
    }

    // 播放BGM方法（BGM0直接播放，BGM1渐变播放）
    public void PlayBGM(int _bgmIndex, bool isJianbian)
    {
        bgmIndex = _bgmIndex;

        if (_bgmIndex == 0)
        {
            // BGM0直接以0.8f音量播放（无渐变）
            bgm[0].volume = bgm0InitVolume;
            bgm[0].Play();
        }
        else if (isJianbian && _bgmIndex == 1)
        {
            // BGM1从0.08f渐变到0.2f
            StartCoroutine(FadeInBGM1(2f));
        }
        else
        {
            bgm[_bgmIndex].Play();
        }
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
            bgm[i].volume = 0;
        }
        isInScoreVolumeFade = false;
    }

    // 停止BGM0并切换到BGM1的方法
    public void SwitchFromBGM0ToBGM1(float fadeOutTime = 1f, float fadeInTime = 2f)
    {
        // 渐隐BGM0
        StartCoroutine(FadeOutBGM(bgm[0], fadeOutTime));
        // 渐入BGM1
        StartCoroutine(FadeInBGM1(fadeInTime));
    }

    public void StopBGMWithFade(int index, float fadeTime = 1f)
    {
        if (index < bgm.Length)
        {
            StartCoroutine(FadeOutBGM(bgm[index], fadeTime));
        }
    }

    public bool IsInScoreVolumeFade()
    {
        return isInScoreVolumeFade;
    }
}
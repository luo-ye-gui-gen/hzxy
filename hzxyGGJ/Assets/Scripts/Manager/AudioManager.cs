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

    // 第一首BGM的初始音量（直接设置为1f）
    private float bgm0InitVolume = 1f;
    // 记录当前正在播放的BGM音量（用于渐变）
    private float currentBGMVolume;

    // 标记是否正在进行900-1000分的音量渐变
    private bool isInScoreVolumeFade = false;
    // 新增：标记是否正在切换BGM（防止重复触发）
    private bool isSwitchingBGM = false;
    // 新增：帧率限制（每N帧更新一次音量，减少计算）
    [SerializeField] private int volumeUpdateFrameSkip = 2; // 每2帧更新一次

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        bgmIndex = 0;
        
        // 初始化BGM0的音量为1f（无渐变，直接固定）
        if (bgm.Length > 0)
        {
            bgm[0].volume = bgm0InitVolume;
            currentBGMVolume = bgm0InitVolume;
            
            // 提前加载BGM1到内存（避免首次播放卡顿）
            if (bgm.Length > 1 && bgm[1].clip != null)
            {
                bgm[1].clip.LoadAudioData();
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
        int frameCounter = 0;

        while (_audio.volume > .1f)
        {
            // 帧率限制：每N帧才更新一次
            frameCounter++;
            if (frameCounter % volumeUpdateFrameSkip != 0)
            {
                yield return null;
                continue;
            }

            _audio.volume = Mathf.Lerp(_audio.volume, 0f, 0.2f); // 用Lerp替代乘法，更平滑且性能更好
            yield return new WaitForSeconds(.05f); // 延长等待时间，减少循环次数

            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }

    // 900-1000分区间BGM0音量从1f渐变到0.4f的协程（优化版）
    public IEnumerator FadeBGM0To04DuringScoreRange(float fadeTime = 1f)
    {
        if (isInScoreVolumeFade) yield break; // 防止重复执行
        isInScoreVolumeFade = true;

        AudioSource bgm0 = bgm[0];
        if (bgm0 == null) yield break;

        float startVolume = bgm0InitVolume; 
        float targetVolume = 0.4f;          
        float elapsedTime = 0f;
        int frameCounter = 0;

        while (elapsedTime < fadeTime)
        {
            // 帧率限制：减少计算次数
            frameCounter++;
            if (frameCounter % volumeUpdateFrameSkip != 0)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeTime);
            bgm0.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        bgm0.volume = targetVolume;
        currentBGMVolume = targetVolume;
        isInScoreVolumeFade = false;
    }

    // BGM音量渐小协程（优化版：减少计算频率）
    public IEnumerator FadeOutBGM(AudioSource audioSource, float fadeTime = 1f)
    {
        if (audioSource == null || !audioSource.isPlaying) yield break;

        float startVolume = audioSource.volume;
        float elapsedTime = 0f;
        int frameCounter = 0;

        while (elapsedTime < fadeTime)
        {
            frameCounter++;
            if (frameCounter % volumeUpdateFrameSkip != 0)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeTime);
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        isInScoreVolumeFade = false;
    }

    // BGM1专用渐入协程（优化版：从0.2f渐变到0.9f）
    public IEnumerator FadeInBGM1(float fadeTime = 2f)
    {
        if (bgm.Length < 2 || bgm[1] == null) yield break;

        AudioSource bgm1 = bgm[1];
        float startVolume = 0.2f;  
        float targetVolume = 0.9f; 
        bgm1.volume = startVolume;
        
        // 提前检查是否已加载，避免播放时加载
        if (!bgm1.clip.isReadyToPlay)
        {
            yield return bgm1.clip.LoadAudioData();
        }
        
        bgm1.Play();

        float elapsedTime = 0f;
        int frameCounter = 0;

        while (elapsedTime < fadeTime)
        {
            frameCounter++;
            if (frameCounter % volumeUpdateFrameSkip != 0)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeTime);
            bgm1.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }
        bgm1.volume = targetVolume;
        currentBGMVolume = targetVolume;
    }

    // 播放BGM方法（优化版：增加状态判断）
    public void PlayBGM(int _bgmIndex, bool isJianbian)
    {
        if (_bgmIndex >= bgm.Length) return;

        bgmIndex = _bgmIndex;

        if (_bgmIndex == 0)
        {
            // BGM0直接以1f音量播放（无渐变）
            bgm[0].volume = bgm0InitVolume;
            bgm[0].Play();
        }
        else if (isJianbian && _bgmIndex == 1 && !isSwitchingBGM)
        {
            // BGM1渐变播放（防止重复触发）
            StartCoroutine(FadeInBGM1(2f));
        }
        else
        {
            bgm[_bgmIndex].Play();
        }
    }

    public void StopAllBGM()
    {
        isSwitchingBGM = false;
        isInScoreVolumeFade = false;

        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
            bgm[i].volume = 0;
        }
    }

    // 停止BGM0并切换到BGM1的方法（优化版：防止重复切换）
    public void SwitchFromBGM0ToBGM1(float fadeOutTime = 1f, float fadeInTime = 2f)
    {
        if (isSwitchingBGM || bgm.Length < 2) return;
        isSwitchingBGM = true;

        // 用顺序执行替代并发执行，减少性能压力
        StartCoroutine(SwitchBGMSequence(fadeOutTime, fadeInTime));
    }

    // 新增：顺序执行BGM切换（先渐隐BGM0，再渐入BGM1）
    private IEnumerator SwitchBGMSequence(float fadeOutTime, float fadeInTime)
    {
        // 第一步：渐隐BGM0
        yield return StartCoroutine(FadeOutBGM(bgm[0], fadeOutTime));
        
        // 第二步：等待一小段时间，再渐入BGM1（减少并发计算）
        yield return new WaitForSeconds(0.1f);
        
        // 第三步：渐入BGM1
        yield return StartCoroutine(FadeInBGM1(fadeInTime));
        
        isSwitchingBGM = false;
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
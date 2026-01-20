using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    [Header("默认震动参数")]
    public float defaultShakeIntensity = 2f;
    public float defaultShakeDuration = 0.4f;
    public float defaultShakeFrequency = 2f;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _cameraShake;
    private Coroutine _activeShakeCoroutine; // 新增：追踪当前震动协程

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        AutoFindCinemachineComponents();
        ForceShakeStop(); // 启动时强制归零
    }

    // 替换过时的OnLevelWasLoaded，使用SceneManager回调
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        AutoFindCinemachineComponents();
        ForceShakeStop(); // 场景加载后归零
    }

    private void AutoFindCinemachineComponents()
    {
        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (_virtualCamera != null)
        {
            _cameraShake = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            ForceShakeStop(); // 找到组件后立即归零
        }
        else
        {
            Debug.LogWarning("未找到CinemachineVirtualCamera组件，请检查场景");
        }
    }

    // 对外暴露的震动方法（修复防重复触发逻辑）
    public void TriggerShake(float intensity = 0, float duration = 0, float frequency = 0)
    {
        if (_cameraShake == null)
        {
            Debug.LogWarning("相机震动组件未初始化，震动失败");
            return;
        }

        // 若已有震动，先停止再重新开始
        if (_activeShakeCoroutine != null)
        {
            StopCoroutine(_activeShakeCoroutine);
        }

        float finalIntensity = intensity <= 0 ? defaultShakeIntensity : intensity;
        float finalDuration = duration <= 0 ? defaultShakeDuration : duration;
        float finalFrequency = frequency <= 0 ? defaultShakeFrequency : frequency;

        _activeShakeCoroutine = StartCoroutine(ShakeCoroutine(finalIntensity, finalDuration, finalFrequency));
    }

    // 震动协程（修复参数重置和渐变逻辑）
    private IEnumerator ShakeCoroutine(float intensity, float duration, float frequency)
    {
        float elapsedTime = 0;

        // 立即归零，不等待帧
        _cameraShake.m_AmplitudeGain = 0;
        _cameraShake.m_FrequencyGain = 0;

        // 设置初始震动参数
        _cameraShake.m_AmplitudeGain = intensity;
        _cameraShake.m_FrequencyGain = frequency;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // 渐变衰减振幅，频率保持不变
            _cameraShake.m_AmplitudeGain = intensity * (1 - elapsedTime / duration);
            yield return null;
        }

        // 强制归零，结束震动
        ForceShakeStop();
        _activeShakeCoroutine = null;
    }

    // 强制停止震动（确保参数完全归零）
    public void ForceShakeStop()
    {
        if (_cameraShake != null)
        {
            _cameraShake.m_AmplitudeGain = 0f;
            _cameraShake.m_FrequencyGain = 0f;
        }
        if (_activeShakeCoroutine != null)
        {
            StopCoroutine(_activeShakeCoroutine);
            _activeShakeCoroutine = null;
        }
    }
}
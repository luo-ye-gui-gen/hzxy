using UnityEngine;
using System.Collections;

public class BombExplosion : MonoBehaviour
{
    [Header("爆炸核心参数【面板可调节】")]
    public float bombCountDown = 5f; // 爆炸倒计时（秒）
    public float explosionRadius = 5f;
    public float explosionForce = 15f;

    [Header("红光特效参数")]
    public GameObject redLightPrefab; // 红光预制体（圆形Sprite）
    public float lightSpawnInterval = 0.5f; // 每层红光生成间隔
    public float lightLifeTime = 2f; // 单层红光生命周期
    public float lightMaxScale = 5f; // 红光最大缩放尺寸
    public float lightStartAlpha = 0.8f; // 红光初始透明度

    [Header("弹出文字相关")]
    private EntityFX entityFX;

    [Header("震动参数（传给全局管理器）")]
    public float shakeIntensity = 2f;
    public float shakeDuration = 0.4f;
    public float shakeFrequency = 2f;

    // 状态标记
    private bool hasExploded = false;
    private int lastCountNum;
    private bool isStarted = false; // 默认未激活
    private float initialCountDown; // 缓存初始倒计时
    private bool firstTextPop;
    private Coroutine lightSpawnCoroutine; // 红光生成协程引用

    void Start()
    {
        entityFX = GetComponent<EntityFX>();
        initialCountDown = bombCountDown; // 缓存初始值
        lastCountNum = Mathf.CeilToInt(initialCountDown);
        hasExploded = false; // 初始化爆炸状态
        firstTextPop = true;
    }

    void Update()
    {
        if (!isStarted || hasExploded) return; // 未激活或已爆炸则直接返回
        
        if (bombCountDown > 0)
        {
            bombCountDown -= Time.deltaTime;
            int currentCountNum = Mathf.CeilToInt(Mathf.Max(bombCountDown, 0)); // 防止负数
            // 倒计时数字变化时显示文字
            if(firstTextPop) 
            {
                entityFX?.CreatePopUpText(lastCountNum.ToString());
                firstTextPop = false;
            }

            if (currentCountNum != lastCountNum && entityFX != null)
            {
                lastCountNum = currentCountNum;
                entityFX.CreatePopUpText(lastCountNum.ToString());
                Debug.Log("倒计时：" + currentCountNum);
            }
        }
        else
        {
            Explode();
            hasExploded = true;
        }
    }

    // 激活炸弹（必须调用这个才开始倒计时）
    public void SetBombActive()
    {
        if (hasExploded) return; // 已爆炸则不重复激活
        bombCountDown = initialCountDown; // 重置倒计时
        lastCountNum = Mathf.CeilToInt(bombCountDown);
        isStarted = true;
        hasExploded = false;
        firstTextPop = true; // 重置文字弹出标记
        Debug.Log("炸弹激活，开始" + initialCountDown + "秒倒计时");

        // 启动红光特效生成协程
        if (redLightPrefab != null)
        {
            if (lightSpawnCoroutine != null)
                StopCoroutine(lightSpawnCoroutine);
            lightSpawnCoroutine = StartCoroutine(SpawnRedLightLayers());
        }
        else
        {
            Debug.LogWarning("未赋值红光预制体，特效无法生成！");
        }
    }

    // 协程：按间隔生成红光层
    private IEnumerator SpawnRedLightLayers()
    {
        while (isStarted && !hasExploded && bombCountDown > 0)
        {
            // 生成新的红光特效
            GameObject light = Instantiate(redLightPrefab, transform.position, Quaternion.identity);
            // 设置父物体（便于管理，销毁炸弹时可一并清理）
            light.transform.SetParent(transform);
            // 启动单层层红光的缩放+淡出动画
            StartCoroutine(AnimateLightLayer(light));
            // 等待指定间隔再生成下一层
            yield return new WaitForSeconds(lightSpawnInterval);
        }
    }

    // 单层层红光的动画：缩放+颜色渐变（淡出）
    private IEnumerator AnimateLightLayer(GameObject light)
    {
        SpriteRenderer sr = light.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("红光预制体缺少SpriteRenderer组件！");
            Destroy(light);
            yield break;
        }

        // 初始化红光状态
        light.transform.localScale = Vector3.zero; // 初始缩放到0
        Color startColor = new Color(1f, 0.2f, 0.2f, lightStartAlpha); // 初始红色（带透明度）
        sr.color = startColor;

        float elapsedTime = 0f;
        while (elapsedTime < lightLifeTime)
        {
            // 如果炸弹已爆炸，提前结束动画
            if (hasExploded) break;

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lightLifeTime; // 0~1的进度值

            // 1. 缩放：从0线性放大到maxScale
            light.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * lightMaxScale, t);

            // 2. 颜色渐变：透明度从startAlpha线性降到0
            Color currentColor = sr.color;
            currentColor.a = Mathf.Lerp(lightStartAlpha, 0f, t);
            sr.color = currentColor;

            yield return null;
        }

        // 动画结束后销毁该层红光
        Destroy(light);
    }

    void Explode()
    {
        // 停止红光生成协程
        if (lightSpawnCoroutine != null)
            StopCoroutine(lightSpawnCoroutine);

        // 调用全局震动
        if (CameraShakeManager.Instance != null)
        {
            CameraShakeManager.Instance.TriggerShake(shakeIntensity, shakeDuration, shakeFrequency);
            Debug.Log("爆炸触发相机震动");
        }
        else
        {
            Debug.LogWarning("CameraShakeManager实例未找到，震动失败");
        }

        // 击飞玩家逻辑
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var collider in collidersInRange)
        {
            if (collider.CompareTag("Player"))
            {
                Rigidbody2D playerRb = collider.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = Vector2.zero;
                    playerRb.AddForce(Vector2.up * explosionForce, ForceMode2D.Impulse);
                }
            }
        }

        // 延迟销毁炸弹
        Destroy(gameObject, 0.1f);
    }

    // 绘制爆炸范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // 防止协程泄漏，对象销毁时停止所有协程
    private void OnDestroy()
    {
        if (lightSpawnCoroutine != null)
            StopCoroutine(lightSpawnCoroutine);
    }
}
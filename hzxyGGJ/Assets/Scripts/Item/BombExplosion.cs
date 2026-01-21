using UnityEngine;
using System.Collections;

public class BombExplosion : MonoBehaviour
{
    [Header("爆炸核心参数【面板可调节】")]
    public float bombCountDown = 5f; // 爆炸倒计时（秒）
    public float explosionRadius = 5f;
    public float explosionForce = 15f;

    [Header("弹出文字相关")]
    private EntityFX entityFX;

    [Header("震动参数（传给全局管理器）")]
    public float shakeIntensity = 2f;
    public float shakeDuration = 0.4f;
    public float shakeFrequency = 2f;

    // 状态标记
    private bool hasExploded = false;
    private int lastCountNum;
    public bool isStarted = false; // 默认未激活
    private float initialCountDown; // 缓存初始倒计时
    private bool firstTextPop;
    private Coroutine lightSpawnCoroutine; // 红光生成协程引用
    private Animator anim;

    void Start()
    {
        entityFX = GetComponent<EntityFX>();
        initialCountDown = bombCountDown; // 缓存初始值
        lastCountNum = Mathf.CeilToInt(initialCountDown);
        hasExploded = false; // 初始化爆炸状态
        firstTextPop = true;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        
        if (!isStarted || hasExploded) return; // 未激活或已爆炸则直接返回

        anim.SetBool("Active",isStarted);
        
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
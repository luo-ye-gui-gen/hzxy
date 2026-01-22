using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FirePrefab : MonoBehaviour
{
    private float xYelocity;
    private Rigidbody2D rb;
    private float liveTime;
    // 新增：获取主摄像机，用于判断是否在屏幕内
    [SerializeField] private Camera mainCamera;
    // 可选：边界偏移值，避免火球刚出屏幕边缘还没完全离开就销毁
    [SerializeField] private float screenEdgeOffset = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        mainCamera = Camera.main;

        rb.velocity = new Vector2(xYelocity, 0);

        // 保留原有的超时销毁（双重保障，防止屏幕判断失效）
        Destroy(gameObject, liveTime);
    }

    public void SetupFirePrefab(float _xYelocity, float _liveTime)
    {
        xYelocity = _xYelocity;
        liveTime = _liveTime;
    }

    void Update()
    {
        // 每帧检查火球是否移出屏幕
        if (!IsObjectInScreen())
        {
            Destroy(gameObject);
        }
    }

    // 核心方法：判断物体是否在屏幕可视范围内
    private bool IsObjectInScreen()
    {
        if (mainCamera == null) return true; // 摄像机为空时不销毁

        // 将物体世界坐标转换为屏幕坐标（屏幕坐标以左下角为(0,0)，右上角为(Screen.width, Screen.height)）
        Vector2 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        // 判断是否超出屏幕范围（加上偏移值，避免临界值误判）
        bool isInX = screenPos.x > -screenEdgeOffset && screenPos.x < Screen.width + screenEdgeOffset;
        bool isInY = screenPos.y > -screenEdgeOffset && screenPos.y < Screen.height + screenEdgeOffset;

        return isInX && isInY;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ice")
        {
            AudioManager.instance.PlaySFX(5, null);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

        if (collision.tag == "Bomb")
        {
            if (collision.GetComponent<BombExplosion>().isStarted) return;
            collision.GetComponent<BombExplosion>().SetBombActive();
            Destroy(gameObject);
        }
    }
}
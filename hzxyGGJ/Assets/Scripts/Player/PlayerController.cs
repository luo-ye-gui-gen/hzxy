using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("移动设置")]
    [Tooltip("玩家向右的持续移动速度")]
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private Vector2 currentVelocity;
    [SerializeField] private float[] accelerateScore = { 250, 600, 1000 };//加速时的分数
    [SerializeField] private float[] accelerate = { 10, 15, 20 };
    private int accelerateCount = 0;
    private int i = 0;

    [Header("跳跃设置")]
    [Tooltip("跳跃力【初始值】")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Vector2 jumpVelocity;
    private float originJumpForce;

    [Header("二段跳设置")]
    [Tooltip("勾选=开启二段跳 | 取消=关闭二段跳（变为单段跳）")]
    [SerializeField] private bool isDoubleJumpOpen = true;  // 二段跳总开关【核心新增】
    [Tooltip("最大跳跃次数（2=二段跳，1=普通跳）")]
    [SerializeField] private int maxJumpCount = 2;
    
    [Header("地面检测设置")] // 单独分组，更清晰
    [Tooltip("地面检测圆形半径")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [Tooltip("地面检测位置偏移（相对于玩家锚点）")] // 新增可调参数
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.5f);
    [Tooltip("地面图层（需在Inspector指定）")]
    [SerializeField] private LayerMask groundLayer;
    
    [Header("二段跳力度设置")]
    [Tooltip("二段跳力度倍率(0~1)")]
    [SerializeField] private float secondJumpRate = 0.8f;

    private int currentJumpCount;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumpCount = maxJumpCount;
        originJumpForce = jumpForce;
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        // 持续向右移动 - 只修改X轴，保留Y轴重力/下落速度
        currentVelocity = rb.velocity;
        currentVelocity.x = runSpeed;
        rb.velocity = currentVelocity;
        
        CheckGrounded();
    }

    private void Update()
    {
        if(!GameManager.instance.playerHealth.isAlived) return;

        if(Time.timeScale == 0) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Accelerate();
    }

    public void Jump()
    {
        // 核心逻辑：判断二段跳开关 + 剩余跳跃次数
        if (currentJumpCount > 0)
        {
            jumpVelocity = rb.velocity;
            // 开启二段跳时：第一段满力跳，第二段按倍率跳
            // 关闭二段跳时：永远都是满力单段跳
            float currentJumpPower = (isDoubleJumpOpen && currentJumpCount == 1) ? jumpForce * secondJumpRate : jumpForce;
            jumpVelocity.y = currentJumpPower;
            rb.velocity = jumpVelocity;

            currentJumpCount--;

            AudioManager.instance.PlaySFX(2,null);
        }
    }

    private void CheckGrounded()
    {
        // 核心修改：使用可调的偏移量计算检测位置
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        
        // 圆形范围检测
        Collider2D hitCollider = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
        
        // 绘制调试圆形
        DrawDebugCircle(checkPos, groundCheckRadius, hitCollider ? Color.green : Color.red);

        isGrounded = hitCollider != null;

        if (isGrounded)
        {
            // 二段跳开关联动：关闭时，重置为1次跳跃机会(单段跳)；开启时，重置为设定的次数(二段跳)
            currentJumpCount = isDoubleJumpOpen ? maxJumpCount : 1;
            jumpForce = originJumpForce;
        }
    }

    // 自定义绘制圆形调试线的方法
    private void DrawDebugCircle(Vector2 center, float radius, Color color, int segments = 16)
    {
        // 计算每一段的角度增量
        float angleStep = 360f / segments;
        float radianStep = Mathf.Deg2Rad * angleStep;
        
        // 记录上一个点的位置
        Vector2 lastPoint = center + new Vector2(radius, 0);
        
        // 绘制圆形的每一段线段
        for (int i = 1; i <= segments; i++)
        {
            float currentRadian = radianStep * i;
            Vector2 currentPoint = center + new Vector2(Mathf.Cos(currentRadian) * radius, Mathf.Sin(currentRadian) * radius);
            Debug.DrawLine(lastPoint, currentPoint, color);
            lastPoint = currentPoint;
        }
    }

    #region 加速
    public void Accelerate()
    {
        if (i <= 2 && ScoreManager.Instance.CurrentScore >= accelerateScore[i] && accelerateCount == i)
        {
            accelerateCount++;
            runSpeed = accelerate[i];
            i++;
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        // 使用可调偏移量绘制Gizmos
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius); // 绘制检测圆形
        Gizmos.DrawWireSphere(checkPos, 0.05f); // 绘制检测中心点
        // 额外绘制偏移线：从玩家锚点到检测中心点，方便调整
        Gizmos.DrawLine(transform.position, checkPos);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (groundLayer == (groundLayer | (1 << other.gameObject.layer)))
        {
            // 碰撞落地也联动二段跳开关
            currentJumpCount = isDoubleJumpOpen ? maxJumpCount : 1;
            jumpForce = originJumpForce;
        }
    }
}
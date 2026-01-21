using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("移动设置")]
    [Tooltip("玩家向右的持续移动速度")]
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private Vector2 currentVelocity;

    [Header("跳跃设置")]
    [Tooltip("跳跃力【初始值】")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Vector2 jumpVelocity;
    private float originJumpForce;

    [Header("二段跳设置")]
    [Tooltip("勾选=开启二段跳 | 取消=关闭二段跳（变为单段跳）")]
    [SerializeField] private bool isDoubleJumpOpen = true;
    [Tooltip("最大跳跃次数（2=二段跳，1=普通跳）")]
    [SerializeField] private int maxJumpCount = 2;

    [Header("地面检测设置")]
    [Tooltip("地面检测圆形半径")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [Tooltip("地面检测位置偏移（相对于玩家锚点）")]
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.8f); // 已下调到角色底部
    [Tooltip("地面图层（需在Inspector指定）")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask trapLayer;

    [Header("AI自动跳跃检测设置")]
    [Tooltip("前方检测点的X轴距离（玩家右侧多远检测）")]
    [SerializeField] private float forwardCheckDistance = 1.2f;
    [SerializeField] private float forwardCheckTrapDistance = 1.2f;
    [Tooltip("前方检测点的Y轴偏移（玩家脚下多高检测）")]
    [SerializeField] private float forwardCheckYOffset = -0.8f; // 同步下调
    [SerializeField] private float forwardCheckTrapYOffset = -0.8f; // 同步下调
    [Tooltip("检测到前方无地面时，延迟多久跳跃（单位：秒）")]
    [SerializeField] private float jumpDelay = 0.1f;
    [Tooltip("二段跳延迟（一段跳后多久触发二段跳）")]
    [SerializeField] private float doubleJumpDelay = 0.2f; // 新增：二段跳触发延迟
    [Tooltip("是否自动触发二段跳（勾选=开启连续跳）")]
    [SerializeField] private bool autoDoubleJump = true; // 新增：自动二段跳开关
    private float jumpTimer;
    private float doubleJumpTimer;
    private bool shouldJump;
    private bool shouldDoubleJump; // 新增：是否需要触发二段跳

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
        // 持续向右移动
        currentVelocity = rb.velocity;
        currentVelocity.x = runSpeed;
        rb.velocity = currentVelocity;

        CheckGrounded();
        CheckForwardGround();
        CheckForwardTrap();
    }

    private void Update()
    {
        //if (!GameManager.instance.playerHealth.isAlived) return;
        if (Time.timeScale == 0) return;

        // AI自动一段跳逻辑
        if (shouldJump && Time.time >= jumpTimer)
        {
            Jump();
            shouldJump = false;

            // 如果开启自动二段跳，设置二段跳触发计时器
            if (autoDoubleJump && isDoubleJumpOpen && currentJumpCount > 0)
            {
                shouldDoubleJump = true;
                doubleJumpTimer = Time.time + doubleJumpDelay;
            }
        }

        // AI自动二段跳逻辑
        if (shouldDoubleJump && Time.time >= doubleJumpTimer)
        {
            Jump();
            shouldDoubleJump = false;
        }
    }

    public void Jump()
    {
        if (currentJumpCount > 0)
        {
            jumpVelocity = rb.velocity;
            float currentJumpPower = (isDoubleJumpOpen && currentJumpCount == 1) ? jumpForce * secondJumpRate : jumpForce;
            jumpVelocity.y = currentJumpPower;
            rb.velocity = jumpVelocity;

            currentJumpCount--;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(2, null);
            }
        }
    }

    private void CheckGrounded()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        Collider2D hitCollider = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
        DrawDebugCircle(checkPos, groundCheckRadius, hitCollider ? Color.green : Color.red);
        isGrounded = hitCollider != null;

        if (isGrounded)
        {
            currentJumpCount = isDoubleJumpOpen ? maxJumpCount : 1;
            jumpForce = originJumpForce;
            shouldDoubleJump = false; // 落地后重置二段跳标记
        }
    }

    // AI核心：检测前方是否有地面
    private void CheckForwardGround()
    {
        // 计算前方检测点的位置
        Vector2 forwardCheckPos = new Vector2(
            transform.position.x + forwardCheckDistance,
            transform.position.y + forwardCheckYOffset
        );

        // 检测前方是否有地面
        Collider2D forwardGround = Physics2D.OverlapCircle(forwardCheckPos, groundCheckRadius, groundLayer);

        // 绘制调试用的检测点
        DrawDebugCircle(forwardCheckPos, groundCheckRadius, forwardGround ? Color.cyan : Color.yellow);

        // 如果脚下有地面，但前方没有地面 → 准备跳跃
        if (isGrounded && forwardGround == null && !shouldJump)
        {
            shouldJump = true;
            jumpTimer = Time.time + jumpDelay;
        }
    }

    // AI核心：检测前方是否有陷阱
    private void CheckForwardTrap()
    {
        // 计算前方检测点的位置
        Vector2 forwardCheckPos = new Vector2(
            transform.position.x + forwardCheckTrapDistance,
            transform.position.y + forwardCheckTrapYOffset
        );

        // 检测前方是否有地面
        Collider2D forwardGround = Physics2D.OverlapCircle(forwardCheckPos, groundCheckRadius, trapLayer);

        // 绘制调试用的检测点
        DrawDebugCircle(forwardCheckPos, groundCheckRadius, forwardGround ? Color.cyan : Color.yellow);

        // 如果脚下有地面，但前方有陷阱 → 准备跳跃
        if (isGrounded && forwardGround != null && !shouldJump)
        {
            shouldJump = true;
            jumpTimer = Time.time + jumpDelay;
        }
    }

    private void DrawDebugCircle(Vector2 center, float radius, Color color, int segments = 16)
    {
        float angleStep = 360f / segments;
        float radianStep = Mathf.Deg2Rad * angleStep;
        Vector2 lastPoint = center + new Vector2(radius, 0);

        for (int i = 1; i <= segments; i++)
        {
            float currentRadian = radianStep * i;
            Vector2 currentPoint = center + new Vector2(Mathf.Cos(currentRadian) * radius, Mathf.Sin(currentRadian) * radius);
            Debug.DrawLine(lastPoint, currentPoint, color);
            lastPoint = currentPoint;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
        Gizmos.DrawWireSphere(checkPos, 0.05f);
        Gizmos.DrawLine(transform.position, checkPos);

        // 绘制AI前方检测点的Gizmos
        Vector2 forwardCheckPos = new Vector2(
            transform.position.x + forwardCheckDistance,
            transform.position.y + forwardCheckYOffset
        );
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(forwardCheckPos, groundCheckRadius);

        // 绘制AI前方Trap检测点的Gizmos
        Vector2 forwardTrapCheckPos = new Vector2(
            transform.position.x + forwardCheckTrapDistance,
            transform.position.y + forwardCheckTrapYOffset
        );
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(forwardTrapCheckPos, groundCheckRadius);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (groundLayer == (groundLayer | (1 << other.gameObject.layer)))
        {
            currentJumpCount = isDoubleJumpOpen ? maxJumpCount : 1;
            jumpForce = originJumpForce;
            shouldDoubleJump = false; // 碰撞落地后重置二段跳标记
        }
    }
}
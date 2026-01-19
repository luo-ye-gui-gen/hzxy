using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    [Tooltip("拖拽你的Player对象到这里")]
    [SerializeField] private Transform player;
    [Tooltip("摄像机X轴相对Player的偏移（可选，默认0）")]
    [SerializeField] private float xOffset = 0f;
    [Tooltip("摄像机固定的Y轴高度")]
    [SerializeField] private float fixedY = 4f;

    // 2D相机固定Z轴（不可修改）
    private readonly float fixedZ = -10f;

    private void LateUpdate()
    {
        // 安全校验：防止没赋值Player导致报错
        if (player == null)
        {
            Debug.LogError("请给CameraFollow脚本指定Player对象！");
            return;
        }

        // 核心逻辑：摄像机X坐标 = Player X坐标 + 偏移，Y/Z轴固定
        transform.position = new Vector3(
            player.position.x + xOffset,
            fixedY,
            fixedZ
        );
    }
}
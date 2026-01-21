using UnityEngine;
using Cinemachine;

/// <summary>
/// 控制玩家死亡时摄像机停止移动的核心脚本
/// 挂载到 Player 物体上，或挂载到专门的游戏管理器上
/// </summary>
public class CameraStopOnPlayerDeath : MonoBehaviour
{
    public static CameraStopOnPlayerDeath instance;
    // 拖拽赋值：你的玩家对象
    public GameObject player;
    // 拖拽赋值：场景中的 Cinemachine 虚拟摄像机
    public CinemachineVirtualCamera followCamera;
    
    // 记录摄像机停止前的状态，方便复活时恢复
    private CinemachineTransposer _originalTransposer;
    private Vector3 _cameraFinalPosition;
    private Quaternion _cameraFinalRotation;

    void Awake()
    {
        if(instance==null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 初始化：获取摄像机的跟随组件（Transposer）
        if (followCamera != null)
        {
            _originalTransposer = followCamera.GetCinemachineComponent<CinemachineTransposer>();
        }
    }

    /// <summary>
    /// 玩家死亡时调用此方法，停止摄像机移动
    /// </summary>
    public void StopCameraMovement()
    {
        
        if (GameManager.instance.playerHealth.isAlived || followCamera == null) return;
        
        // 1. 记录摄像机当前的最终位置和旋转（死亡瞬间的位置）
        _cameraFinalPosition = followCamera.transform.position;
        _cameraFinalRotation = followCamera.transform.rotation;
        
        // 2. 取消摄像机对玩家的跟随和看向，停止自动移动
        followCamera.Follow = null;
        followCamera.LookAt = null;
        
        // 3. 强制摄像机固定在死亡瞬间的位置和角度
        followCamera.transform.position = _cameraFinalPosition;
        followCamera.transform.rotation = _cameraFinalRotation;
        
        // 4. 禁用摄像机的自动更新（可选，确保完全不移动）
        followCamera.enabled = false;
        
        Debug.Log("玩家已死亡，摄像机已停止移动");
    }

    /// <summary>
    /// 可选：玩家复活时恢复摄像机移动
    /// </summary>
    public void ResumeCameraMovement()
    {
        if (GameManager.instance.playerHealth.isAlived || followCamera == null || player == null) return;

        
        // 恢复摄像机的跟随和看向
        followCamera.Follow = player.transform;
        followCamera.LookAt = player.transform;
        
        // 重新启用摄像机
        followCamera.enabled = true;
        
        Debug.Log("玩家已复活，摄像机恢复移动");
    }


}
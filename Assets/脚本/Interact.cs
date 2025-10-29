
using System;
using UnityEngine;

public class Interact : MonoBehaviour
{
    Transform cameraTransform;
    Transform targetTransform;
    bool isTriger = false;
    bool isFirstTriger = true;
    
    // 添加新变量来控制视角过渡
    bool isTransitioning = false; // 是否正在过渡视角
    float transitionProgress = 0f; // 过渡进度（0-1）
    [Header("视角过度速度")]
    [SerializeField] private float transitionSpeed = 10f; // 过渡速度
    Quaternion originalCameraRotation; // 摄像机原始旋转
    Vector3 originalCameraPosition; // 摄像机原始位置
    Quaternion targetCameraRotation; // 目标摄像机旋转
    Vector3 targetCameraPosition; // 目标摄像机位置
    
    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞物体是否是可交互物体
        if (other.CompareTag("Interactable"))
        {
            isTriger = true;
            targetTransform = other.transform;
            
            // 如果是第一次触发，开始视角过渡
            if (isFirstTriger)
            {
                StartCameraTransition();
                isFirstTriger = false;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // 当玩家离开交互区域时重置状态
        if (other.CompareTag("Interactable"))
        {
            isTriger = false;
            targetTransform = null;
            
            // 如果正在过渡，重置摄像机控制
            if (isTransitioning)
            {
                ResetCameraControl();
            }
        }
    }
    
    private void Start()
    {
        cameraTransform = transform.GetChild(0); // 获取摄像机变换组件
    }
    
    private void Update()
    {
        if (isTriger && isTransitioning)
        {
            // 执行视角过渡
            ExecuteCameraTransition();
        }
        
        // 检测交互输入（例如按下E键进行交互）
        if (isTriger && Input.GetKeyDown(KeyCode.E))
        {
            PerformInteraction();
        }
    }
    
    void StartCameraTransition()
    {
        isTransitioning = true;
        transitionProgress = 0f;
        
        // 保存摄像机原始状态
        originalCameraRotation = cameraTransform.rotation;
        originalCameraPosition = cameraTransform.localPosition;
        
        // 禁用玩家输入
        cameraTransform.GetComponent<CameraRotate>().EnableInput(false);
        
        // 计算目标摄像机位置和旋转
        CalculateTargetCameraTransform();
    }
    
    void CalculateTargetCameraTransform()
    {
        if (targetTransform == null) return;
        
        // 查找标记点
        Transform lookAtPoint = targetTransform.Find("LookPoint");
        
        if (lookAtPoint != null)
        {
            // 使用标记点的位置和旋转
            targetCameraPosition = transform.InverseTransformPoint(lookAtPoint.position);
            targetCameraRotation = lookAtPoint.rotation;
        }
        else
        {
            // 如果没有找到标记点，记录错误并返回
            Debug.LogError("未找到LookPoint标记点！");
            isTransitioning = false; // 停止视角过渡
        }
    }

    void ExecuteCameraTransition()
    {
        if (transitionProgress < 1f)
        {
            // 增加过渡进度
            transitionProgress += Time.deltaTime * transitionSpeed;

            // 使用平滑插值函数让过渡更自然
            float smoothProgress = Mathf.SmoothStep(0f, 1f, transitionProgress);

            // 插值摄像机位置和旋转
            cameraTransform.localPosition = Vector3.Lerp(
                originalCameraPosition,
                targetCameraPosition,
                smoothProgress
            );

            cameraTransform.rotation = Quaternion.Slerp(
                originalCameraRotation,
                targetCameraRotation,
                smoothProgress
            );
        }
        else
        {
            // 过渡完成
            transitionProgress = 1f;

            // 可以在这里添加过渡完成后的逻辑
            // 例如显示交互提示UI等
        }
    }
    
    
    void ResetCameraControl()
    {
        isTransitioning = false;
        transitionProgress = 0f;
        
        // 恢复玩家输入控制
        cameraTransform.GetComponent<CameraRotate>().EnableInput(true);
    }
    
    void PerformInteraction()
    {
        // 执行具体的交互逻辑
        Debug.Log("与 " + targetTransform.name + " 进行交互");
        
        // 这里可以添加具体的交互行为，例如：
        // - 打开宝箱
        // - 与NPC对话
        // - 拾取物品
        // - 触发机关等
        
        // 交互完成后，可以选择重置视角或保持当前视角
        // ResetCameraControl(); // 取消注释这行可以在交互后立即恢复控制
    }
}
using System.Collections;
using UnityEngine;

/// <summary>
/// 风互动涟漪控制器 - 使用Shader实现涟漪效果
/// 添加到带有RippleShader材质的物体上
/// </summary>
public class WindRippleController : WindInteractable
{
    [Header("渲染器")]
    public Renderer targetRenderer; // 需要显示涟漪的渲染器
    
    [Header("涟漪参数")]
    public float rippleStrength = 0.5f; // 涟漪强度
    public float rippleSpeed = 2.0f; // 涟漪扩散速度
    public float rippleFrequency = 30.0f; // 涟漪频率
    public float fadeOutTime = 1.5f; // 淡出时间
    [Range(0f, 1f)]
    public float maxRadius = 1.0f; // 涟漪最大半径（0-1，1为全屏）
    [Range(0f, 0.5f)]
    public float edgeFade = 0.2f; // 边缘渐隐范围（0.5为边缘完全透明）
    
    [Header("发射器设置")]
    public bool randomCenter = false; // 是否随机涟漪中心
    public Vector2 centerOffset = Vector2.zero; // 涟漪中心偏移
    
    [Header("广告牌设置")]
    public bool billboard = true; // 是否启用广告牌效果
    public Camera mainCamera; // 摄像机引用（为空则自动查找主摄像机）
    
    [Header("音量响应设置")]
    public bool useVolumeResponse = false; // 是否使用音量响应
    [Range(0f, 1f)]
    public float minVolume = 0.3f; // 最小触发音量
    [Range(0f, 1f)]
    public float maxVolume = 1.0f; // 最大触发音量
    public float minSpeed = 1.0f; // 音量最小时的速度
    public float maxSpeed = 5.0f; // 音量最大时的速度
    public AnimationCurve volumeToSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1); // 音量到速度的映射曲线
    
    private Material rippleMaterial;
    private float startTime;
    private bool isRippling = false;
    private GameObject rippleQuad; // Quad引用（自动创建时才有）
    private Transform billboardTarget; // 广告牌目标（可能是Quad或者当前物体）
    private float currentSpeed; // 当前速度
    private static int startTimeProperty = Shader.PropertyToID("_StartTime");
    
    void Start()
    {
        // 自动查找主摄像机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();
        
        if (targetRenderer != null)
        {
            // 获取材质实例（避免影响其他使用该材质的物体）
            rippleMaterial = targetRenderer.material;
            
            // 初始化为不可见
            InitializeMaterial();
            
            // 设置广告牌目标为当前物体
            billboardTarget = transform;
        }
        else
        {
            // 如果没有渲染器，创建一个Quad来显示涟漪效果
            CreateRippleQuad();
        }
    }
    
    void CreateRippleQuad()
    {
        // 创建一个简单的Quad来显示涟漪
        rippleQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        rippleQuad.name = "RippleQuad";
        rippleQuad.transform.SetParent(transform);
        rippleQuad.transform.localPosition = Vector3.zero;
        rippleQuad.transform.localRotation = Quaternion.identity;
        rippleQuad.transform.localScale = Vector3.one;
        
        targetRenderer = rippleQuad.GetComponent<Renderer>();
        
        // 创建材质
        Material mat = new Material(Shader.Find("Custom/RippleShader"));
        mat.color = new Color(0.5f, 0.7f, 1f, 0.8f); // 水蓝色涟漪
        
        targetRenderer.material = mat;
        rippleMaterial = mat;
        
        InitializeMaterial();
        
        // 设置广告牌目标为Quad
        billboardTarget = rippleQuad.transform;
    }
    
    protected override void Update()
    {
        // 调用父类方法保持原有触发逻辑
        base.Update();
        
        // 音量响应：根据麦克风音量动态调整涟漪速度
        if (useVolumeResponse && rippleMaterial != null && WindInput.Instance != null)
        {
            float volume = WindInput.Instance.Volume;
            
            // 将音量映射到速度
            float volumeNormalized = Mathf.InverseLerp(minVolume, maxVolume, volume);
            volumeNormalized = Mathf.Clamp01(volumeNormalized); // 限制在0-1
            
            // 使用曲线映射
            float curveValue = volumeToSpeedCurve.Evaluate(volumeNormalized);
            
            // 映射到速度范围
            currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, curveValue);
            
            // 如果在涟漪动画中，实时更新速度
            if (isRippling)
            {
                rippleMaterial.SetFloat("_RippleSpeed", currentSpeed);
                rippleMaterial.SetFloat("_RippleSpread", currentSpeed);
            }
        }
    }
    
    void LateUpdate()
    {
        // 广告牌效果：让Quad始终面向摄像机
        if (billboard && billboardTarget != null && mainCamera != null)
        {
            Vector3 direction = mainCamera.transform.position - billboardTarget.position;
            
            // 如果摄像机距离太近，跳过旋转避免抖动
            if (direction.magnitude > 0.1f)
            {
                billboardTarget.rotation = Quaternion.LookRotation(direction, mainCamera.transform.up);
            }
        }
    }
    
    void InitializeMaterial()
    {
        if (rippleMaterial == null) return;
        
        rippleMaterial.SetFloat("_RippleStrength", 0f);
        rippleMaterial.SetFloat("_RippleSpeed", 0f);
        rippleMaterial.SetFloat("_FadeOut", fadeOutTime);
    }
    
    protected override void OnBlow()
    {
        if (!isRippling)
        {
            StartRipple();
        }
    }
    
    void StartRipple()
    {
        if (rippleMaterial == null) return;
        
        isRippling = true;
        startTime = Time.time;
        
        // 确定初始速度（如果启用音量响应，会根据音量计算）
        if (useVolumeResponse && WindInput.Instance != null)
        {
            float volume = WindInput.Instance.Volume;
            float volumeNormalized = Mathf.InverseLerp(minVolume, maxVolume, volume);
            volumeNormalized = Mathf.Clamp01(volumeNormalized);
            float curveValue = volumeToSpeedCurve.Evaluate(volumeNormalized);
            currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, curveValue);
        }
        else
        {
            currentSpeed = rippleSpeed;
        }
        
        // 如果随机中心，生成随机偏移
        Vector2 offset = randomCenter ? 
            new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f)) : 
            centerOffset;
        
        // 设置Shader参数
        rippleMaterial.SetFloat("_RippleStrength", rippleStrength);
        rippleMaterial.SetFloat("_RippleSpeed", currentSpeed);
        rippleMaterial.SetFloat("_RippleFrequency", rippleFrequency);
        rippleMaterial.SetFloat("_RippleSpread", currentSpeed);
        rippleMaterial.SetFloat("_FadeOut", fadeOutTime);
        rippleMaterial.SetFloat("_MaxRadius", maxRadius);
        rippleMaterial.SetFloat("_EdgeFade", edgeFade);
        rippleMaterial.SetVector("_RippleCenter", new Vector4(0.5f + offset.x, 0.5f + offset.y, 0, 0));
        rippleMaterial.SetFloat(startTimeProperty, startTime);
        
        // 启动协程来重置状态
        StartCoroutine(ResetAfterFade());
    }
    
    IEnumerator ResetAfterFade()
    {
        // 等待淡出完成
        yield return new WaitForSeconds(fadeOutTime);
        
        isRippling = false;
        isTriggered = false; // 允许再次触发
        
        // 重置Shader参数
        InitializeMaterial();
    }
    
    void OnDestroy()
    {
        // 清理材质实例
        if (rippleMaterial != null && Application.isPlaying)
        {
            Destroy(rippleMaterial);
        }
    }
}


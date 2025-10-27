using UnityEngine;

/// <summary>
/// 涟漪触发区域 - 添加到任意GameObject上，检测玩家吹气后播放涟漪动画
/// </summary>
public class RippleTriggerZone : WindInteractable
{
    [Header("触发器设置")]
    public bool enableVisualization = true; // 显示触发器范围
    public float triggerRadius = 5f; // 触发半径
    
    [Header("涟漪特效设置")]
    public GameObject ripplePrefab; // 涟漪特效预制体（可选）
    public bool spawnRippleEffect = true; // 是否生成涟漪效果
    
    protected override void OnBlow()
    {
        // 播放涟漪效果
        if (spawnRippleEffect)
        {
            SpawnRippleEffect();
        }
        
        // 可以在这里添加其他触发效果，比如播放音效、触发事件等
    }
    
    void SpawnRippleEffect()
    {
        if (ripplePrefab != null)
        {
            // 如果有预制体，实例化它
            Instantiate(ripplePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            // 否则创建一个简单的粒子效果
            CreateSimpleRippleEffect();
        }
    }
    
    void CreateSimpleRippleEffect()
    {
        // 创建一个临时的GameObject来播放涟漪
        GameObject ripple = new GameObject("RippleEffect");
        ripple.transform.position = transform.position;
        
        // 添加粒子系统（如果需要简单的视觉效果）
        // 或者你可以使用之前创建的Shader效果
    }
    
    // 在编辑器中显示触发器范围
    void OnDrawGizmos()
    {
        if (enableVisualization)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }
    }
}


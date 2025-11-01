using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline; // 关联Timeline的PlayableDirector
    [SerializeField] private CinemachineVirtualCamera[] timelineVcams; // Timeline中使用的Cinemachine虚拟相机
    [SerializeField] private Camera targetCamera; // 运镜结束后要启用的原生Camera
    [SerializeField] private Camera cinemachineBrainCamera; // 挂载Cinemachine Brain的相机（通常是Main Camera）

    void Start()
    {
        // 初始时禁用目标原生相机（可选，根据需求决定）
        if (targetCamera != null)
            targetCamera.gameObject.SetActive(false);
        
        // 监听Timeline结束事件
        timeline.stopped += OnTimelineStopped;
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        // 1. 禁用Timeline用的虚拟相机（避免继续影响画面）
        foreach (CinemachineVirtualCamera timelineVcam in timelineVcams)
        {
            if (timelineVcam != null)
                timelineVcam.enabled = false;
        }
    
        // 2. 启用目标原生生长相机
        if (targetCamera != null)
        {
            targetCamera.gameObject.SetActive(true);
            // 3. 提高原生相机的渲染优先级（depth值越高，越晚渲染，覆盖之前的画面）
            targetCamera.depth = cinemachineBrainCamera.depth + 1;
            cinemachineBrainCamera.gameObject.SetActive(false);
        }
    }
}

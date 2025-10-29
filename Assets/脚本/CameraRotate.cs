using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotate : MonoBehaviour
{
    [Header("x轴灵敏度")]
    [SerializeField] private float sensX = 100f;
    [Header("y轴灵敏度")]
    [SerializeField] private float sensY = 100f;
    [SerializeField] Transform cameraTransform;
    private float xRotation;
    private float yRotation;
    private NewActions actions;
    private Vector2 _look;
    
    [Header("误触保护")]
    [SerializeField] private float deadZone = 15f; // 小于这个值的移动被忽略
    [SerializeField] private float minSwipeDistance = 25f; // 最小滑动距离
    
    // 用于误触检测的变量
    private Vector2[] touchStartPositions = new Vector2[10];
    private bool inputEnabled = true;

    void Awake()
    {
        actions = new NewActions();

        actions.Player.Look.performed += ctx => _look = ctx.ReadValue<Vector2>();
        actions.Player.Look.canceled += ctx => _look = Vector2.zero;
    }

    private void OnEnable()
    {
        actions.Player.Enable();
    }

    private void OnDisable()
    {
        actions.Player.Disable();
    }

    void Update()
    {
        if (!inputEnabled) return;

        if (Touchscreen.current.touches.Count > 0)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                // 只处理右半屏的触摸
                if (touch.position.ReadValue().x > Screen.width / 2)
                {
                    Vector2 delta = touch.delta.ReadValue();
                    
                    // 死区检查 - 忽略小的移动
                    if (delta.magnitude < deadZone)
                        continue;
                    
                    // 记录触摸开始位置
                    if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        int touchId = touch.touchId.ReadValue();
                        if (touchId < touchStartPositions.Length)
                            touchStartPositions[touchId] = touch.position.ReadValue();
                    }
                    
                    // 检查滑动距离是否足够
                    if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
                    {
                        int touchId = touch.touchId.ReadValue();
                        if (touchId < touchStartPositions.Length)
                        {
                            float swipeDistance = Vector2.Distance(touch.position.ReadValue(), touchStartPositions[touchId]);
                            if (swipeDistance < minSwipeDistance)
                                continue; // 滑动距离不够，忽略
                        }
                    }

                    float mouseX = delta.x * sensX * Time.deltaTime;
                    float mouseY = delta.y * sensY * Time.deltaTime;

                    yRotation += mouseX;
                    xRotation -= mouseY;
                    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                    cameraTransform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
                }
            }
        }
    }
    
    // 用于其他脚本控制相机输入
    public void EnableInput(bool enable)
    {
        inputEnabled = enable;
    }
}
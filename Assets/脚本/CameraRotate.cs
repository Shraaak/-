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
        if (Touchscreen.current.touches.Count > 0)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                // 只处理右半屏的触摸
                if (touch.position.ReadValue().x > Screen.width / 2)
                {
                    Vector2 delta = touch.delta.ReadValue();
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
}

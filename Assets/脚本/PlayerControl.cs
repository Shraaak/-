using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionToUse;
    [Header("角色移动的速度")]
    [SerializeField] private float speed = 5;
    [SerializeField] Transform cameraTransform;

    void Update()
    {
        Vector2 _move = moveActionToUse.action.ReadValue<Vector2>();

        Vector3 moveDir = cameraTransform.forward * _move.y + cameraTransform.right * _move.x;
        moveDir.y = 0f;
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
        transform.Translate(moveDir*speed*Time.deltaTime);
    }
}

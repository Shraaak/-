using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionToUse;
    [SerializeField] private float speed = 5;

    void Update()
    {
        Vector2 _move = moveActionToUse.action.ReadValue<Vector2>();

        Vector3 moveDir = new Vector3(_move.x, 0f, _move.y);
        transform.Translate(moveDir*speed*Time.deltaTime);
    }
}

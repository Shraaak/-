using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WindInteractable : MonoBehaviour
{
    [SerializeField] protected float threshold = 0.3f;
    protected bool isTriggered = false;

    protected virtual void Update()
    {
        if (!isTriggered && WindInput.Instance.Volume > threshold)
        {
            isTriggered = true;
            OnBlow();
        }
    }

    protected abstract void OnBlow();
}

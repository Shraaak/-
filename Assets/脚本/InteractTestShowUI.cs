using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractTestShowUI : MonoBehaviour
{
    public static InteractTestShowUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Hide();
    }

    [SerializeField] private TextMeshProUGUI text;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

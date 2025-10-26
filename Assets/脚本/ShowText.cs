using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private string[] sentences;
    private int currentIndex = 0;

    void Start()
    {
        if (sentences.Length > 0)
        {
            displayText.text = sentences[0];
        }

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextSentence();
        }
    }

    void NextSentence()
    {
        currentIndex++;
        if (currentIndex >= sentences.Length)
        {
            displayText.text = "";
        }
        else
        {
            displayText.text = sentences[currentIndex];
        }
        
    }
}

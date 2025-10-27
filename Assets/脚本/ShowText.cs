using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        TextEffect();
    }

    void NextSentence()
    {
        currentIndex++;
        if (currentIndex >= sentences.Length)
        {
            displayText.text = "";
            //改变场景
            StartCoroutine(ChangePlayerScence());
        }
        else
        {
            
            displayText.text = sentences[currentIndex];
            
        }
    }

    void TextEffect()
    {
        // 所有句子都先变为灰色
        Color baseGray = new Color(0.6f, 0.6f, 0.6f);
        string baseHex = ColorUtility.ToHtmlStringRGB(baseGray);

        if (currentIndex >= sentences.Length) return;

        string sentence = sentences[currentIndex];

        // 默认整句浅灰色
        string fullText = $"<color=#{baseHex}>{sentence}</color>";

        // 针对不同句子加特殊效果
        if (currentIndex == 3)
        {
            // “风”字闪烁：黑灰 ↔ 浅灰
            float t = (Mathf.Sin(Time.time * 4f) + 1f) / 2f;
            Color flashColor = Color.Lerp(new Color(0.15f, 0.15f, 0.15f), new Color(0.4f, 0.4f, 0.4f), t);
            string flashHex = ColorUtility.ToHtmlStringRGB(flashColor);


            string before = sentence.Substring(0, 1);
            string target = sentence.Substring(1, 1);
            string after = sentence.Substring(2);

            fullText = $"<color=#{baseHex}>{before}</color><color=#{flashHex}>{target}</color><color=#{baseHex}>{after}</color>";
        }
        else if (currentIndex == 2)
        {
            // 某些特殊字（例如前4个字）是深灰
            string darkHex = ColorUtility.ToHtmlStringRGB(new Color(0.25f, 0.25f, 0.25f));
            string target = sentence.Substring(0, 4);
            string after = sentence.Substring(4);
            fullText = $"<color=#{darkHex}>{target}</color><color=#{baseHex}>{after}</color>";
        }

        displayText.text = fullText;
    }

    IEnumerator ChangePlayerScence()
    {
        yield return new WaitForSeconds(2f);
        //等待两秒进行动画转场
        SceneManager.LoadScene(2);
    }
}

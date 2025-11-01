using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class ShowText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private string[] sentences;
    [Tooltip("淡入/淡出时长（秒））")]
    [SerializeField] private float fadeDuration = 0.6f;

    private CanvasGroup canvasGroup;
    private int currentIndex = 0;
    private bool isFading = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        if (sentences == null || sentences.Length == 0)
        {
            displayText.text = "";
            return;
        }

        currentIndex = 0;
        canvasGroup.alpha = 0f; // 从透明开始
        UpdateDisplayedText();  // 显示第一句文字内容

        // 第一行自动淡入
        StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFading)
        {
            // 每次点击开始一次淡出→切换→淡入流程
            StartCoroutine(FadeOutThenNext());
        }

        // 每帧更新文字效果（比如“风”字闪烁）
        // 注意：TextEffect 会在每帧写入 displayText.text
        if (sentences != null && currentIndex < sentences.Length)
            TextEffect();
    }

    IEnumerator FadeOutThenNext()
    {
        if (isFading) yield break;
        isFading = true;

        // Fade out
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));

        // 当淡出完成后，前进到下一句或处理结束逻辑
        currentIndex++;
        if (currentIndex >= sentences.Length)
        {
            // 所有句子读完：保持短暂淡出，之后切场景
            yield return new WaitForSeconds(0.2f);
            // 你可以在这里做更长的过渡
            SceneManager.LoadScene(2);
            yield break;
        }
        else
        {
            // 更新立即显示下一句的文字内容（TextEffect 会处理实际 text）
            UpdateDisplayedText();

            // Fade in
            yield return StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));
        }

        isFading = false;
    }

    IEnumerator FadeCanvasGroup(float from, float to, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    void UpdateDisplayedText()
    {
        // 不直接 set displayText.text，这里只是确保 TextEffect 会根据 currentIndex 写入文本。
        // 为了马上看到下一句，调用一次 TextEffect。
        TextEffect();
    }

    void TextEffect()
    {
        if (sentences == null || sentences.Length == 0) return;
        if (currentIndex < 0 || currentIndex >= sentences.Length) return;

        // 所有句子都先变为灰色（可调整深浅）
        Color baseGray = new Color(0.6f, 0.6f, 0.6f);
        string baseHex = ColorUtility.ToHtmlStringRGB(baseGray);

        string sentence = sentences[currentIndex];
        string fullText = $"<color=#{baseHex}>{sentence}</color>";

        // 针对不同句子加特殊效果
        if (currentIndex == 3 && sentence.Length >= 3)
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
        else if (currentIndex == 2 && sentence.Length >= 4)
        {
            // 前4个字是深灰
            string darkHex = ColorUtility.ToHtmlStringRGB(new Color(0.25f, 0.25f, 0.25f));
            string target = sentence.Substring(0, 4);
            string after = sentence.Substring(4);
            fullText = $"<color=#{darkHex}>{target}</color><color=#{baseHex}>{after}</color>";
        }

        // 将 rich-text 写回 TMP
        displayText.text = fullText;
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindInput : MonoBehaviour
{
    public static WindInput Instance { get; private set; }

    [Header("麦克风参数")]
    private AudioClip micClip;//存储音频
    private string micName;//麦克风设备名
    private const int sampleWindow = 128;
    private float volume;//当前音量
    public bool IsBlowing = false;

    [Header("调试UI")]
    public Slider volumeSlider;   
    public TextMeshProUGUI volumeText;
    void Awake()
    {
        Instance = this;
        //获取第一个麦克风设备名
        micName = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;

        if (micName != null)
        {
            micClip = Microphone.Start(micName, true, 1, 44100);
            print("开启麦克风");
        }
        else
        {
            print("麦克风开启失败");
        }
    }

    void Update()
    {
        volume = GetAveragedVolume();
        IsBlowing = volume > 0.3f;

        //更新ui
        if (volumeSlider)
            volumeSlider.value = volume;
        if (volumeText)
            volumeText.text = $"Volume: {volume:F2}";
    }

    float GetAveragedVolume()
    {
        if (micClip == null) return 0f;

        int micPos = Microphone.GetPosition(micName) - sampleWindow + 1;
        if (micPos < 0) return 0f;

        float[] waveData = new float[sampleWindow];
        micClip.GetData(waveData, micPos);

        float sum = 0f;
        for (int i = 0; i < sampleWindow; i++)
            sum += Mathf.Abs(waveData[i]);

        return Mathf.Clamp01(sum / sampleWindow * 100f);
    }
    
    public float Volume => volume;
}

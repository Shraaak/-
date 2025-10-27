using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindInput : MonoBehaviour
{
    public static WindInput Instance { get; private set; }

    [Header("麦克风参数")]
    private AudioClip micClip;//存储音频
    private string micName;//麦克风设备名
    private const int sampleRate = 44100;
    private const int sampleWindow = 1024; // 增大采样窗口以进行频谱分析
    private float volume;//当前音量
    public bool IsBlowing = false;
    
    [Header("吹气检测参数")]
    [Tooltip("音量阈值")]
    [Range(0f, 1f)]
    public float volumeThreshold = 0.15f;
    [Tooltip("高频能量阈值（用于识别风声）")]
    [Range(0f, 1f)]
    public float highFreqThreshold = 0.2f;
    [Tooltip("启用高频检测（更精准识别吹气）")]
    public bool useHighFreqDetection = true;
    [Tooltip("噪音抑制强度")]
    [Range(0f, 0.5f)]
    public float noiseFloor = 0.05f;
    [Tooltip("平滑系数")]
    [Range(0f, 0.9f)]
    public float smoothing = 0.3f;
    
    // 频谱分析
    private float[] spectrumData = new float[512];
    private float highFreqEnergy = 0f;
    private float smoothedVolume = 0f;
    private float baselineNoise = 0f;
    private int noiseCalibrationFrames = 0;
    
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
            micClip = Microphone.Start(micName, true, 1, sampleRate);
            print("开启麦克风: " + micName);
            print("提示：前2秒会校准噪音基线，请保持安静");
        }
        else
        {
            print("麦克风开启失败");
        }
    }

    void Update()
    {
        // 前2秒校准噪音基线
        if (noiseCalibrationFrames < 120)
        {
            noiseCalibrationFrames++;
            float rawVolume = GetRawVolume();
            baselineNoise = Mathf.Max(baselineNoise, rawVolume);
            return;
        }
        
        // 获取音量和频谱数据
        volume = GetAveragedVolume();
        highFreqEnergy = GetHighFrequencyEnergy();
        
        // 判断是否在吹气
        if (useHighFreqDetection)
        {
            // 组合判断：音量 + 高频能量
            IsBlowing = (volume > volumeThreshold) && (highFreqEnergy > highFreqThreshold);
        }
        else
        {
            // 仅音量判断
            IsBlowing = volume > volumeThreshold;
        }

        // 更新UI
        if (volumeSlider)
            volumeSlider.value = volume;
        if (volumeText)
            volumeText.text = $"Volume: {volume:F2}";
    }

    float GetRawVolume()
    {
        if (micClip == null) return 0f;

        int micPos = Microphone.GetPosition(micName) - sampleWindow + 1;
        if (micPos < 0) return 0f;

        float[] waveData = new float[sampleWindow];
        micClip.GetData(waveData, micPos);

        float sum = 0f;
        for (int i = 0; i < sampleWindow; i++)
            sum += Mathf.Abs(waveData[i]);

        return sum / sampleWindow;
    }
    
    float GetAveragedVolume()
    {
        float rawVol = GetRawVolume();
        
        // 减去噪音基线
        rawVol = Mathf.Max(0f, rawVol - baselineNoise - noiseFloor);
        
        // 平滑处理
        smoothedVolume = Mathf.Lerp(smoothedVolume, rawVol, 1f - smoothing);
        
        // 放大并限制范围
        return Mathf.Clamp01(smoothedVolume * 50f);
    }
    
    float GetHighFrequencyEnergy()
    {
        if (micClip == null) return 0f;
        
        // 获取频谱数据
        AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        
        // 计算高频段能量（2000Hz - 10000Hz 范围，风声主要在这个频段）
        // 假设采样率44100Hz，FFT大小512，每个bin约86Hz
        // 2000Hz约在bin 23，10000Hz约在bin 116
        int startBin = 23;
        int endBin = 116;
        
        float highFreqSum = 0f;
        for (int i = startBin; i < endBin && i < spectrumData.Length; i++)
        {
            highFreqSum += spectrumData[i];
        }
        
        // 归一化
        float energy = highFreqSum / (endBin - startBin);
        
        // 放大并限制范围
        return Mathf.Clamp01(energy * 100f);
    }
    
    // 公开接口
    public float Volume => volume;
    public float HighFreqEnergy => highFreqEnergy;
    
    // 手动校准噪音
    public void RecalibrateNoise()
    {
        noiseCalibrationFrames = 0;
        baselineNoise = 0f;
        print("重新校准噪音基线...");
    }
}

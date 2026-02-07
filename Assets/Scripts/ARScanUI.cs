using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AR扫描UI - 专门管理AR扫描相关的用户界面
/// 基于Pixso设计稿的现代扫描界面
/// </summary>
public class ARScanUI : MonoBehaviour
{
    [Header("扫描界面组件")]
    public GameObject scanPanel;               // 扫描主面板
    public RectTransform scanFrame;            // 扫描框
    public Image scanLine;                     // 扫描线
    public Text scanHintText;                  // 扫描提示文本
    public Text animalDetectedText;            // 动物检测文本
    public Image animalDetectedIcon;           // 动物检测图标
    
    [Header("控制按钮")]
    public Button startScanButton;             // 开始扫描按钮
    public Button stopScanButton;              // 停止扫描按钮
    public Button toggleFlashButton;           // 切换闪光灯按钮
    public Button showEncyclopediaButton;      // 显示图鉴按钮
    
    [Header("状态指示器")]
    public GameObject connectingIndicator;     // 连接中指示器
    public Image connectionStatusIcon;         // 连接状态图标
    public Text connectionStatusText;          // 连接状态文本
    
    [Header("动物预览")]
    public GameObject animalPreviewPanel;      // 动物预览面板
    public Text previewAnimalName;             // 预览动物名称
    public Text previewAnimalStatus;           // 预览动物状态
    public Image previewAnimalImage;           // 预览动物图片
    
    [Header("动画设置")]
    public float scanLineSpeed = 100f;         // 扫描线移动速度
    public float scanFramePulseRate = 1f;      // 扫描框脉动频率
    public Color scanFrameActiveColor = Color.green;   // 激活时的颜色
    public Color scanFrameInactiveColor = Color.white; // 非激活时的颜色
    
    [Header("音效")]
    public AudioClip scanStartSound;           // 扫描开始音效
    public AudioClip scanDetectSound;          // 检测到动物音效
    public AudioClip scanCompleteSound;        // 扫描完成音效
    
    // 状态变量
    private bool isScanning = false;
    private bool isConnected = false;
    private float scanLinePosition = 0f;
    private AudioSource audioSource;
    
    void Start()
    {
        InitializeComponents();
        SetupEventHandlers();
        ShowIdleState();
    }
    
    void Update()
    {
        if (isScanning)
        {
            UpdateScanAnimation();
        }
    }
    
    /// <summary>
    /// 初始化组件
    /// </summary>
    private void InitializeComponents()
    {
        // 获取或创建AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // 初始状态
        if (scanPanel != null) scanPanel.SetActive(false);
        if (animalPreviewPanel != null) animalPreviewPanel.SetActive(false);
        if (connectingIndicator != null) connectingIndicator.SetActive(false);
        
        UpdateConnectionStatus(false);
    }
    
    /// <summary>
    /// 设置事件处理器
    /// </summary>
    private void SetupEventHandlers()
    {
        if (startScanButton != null)
            startScanButton.onClick.AddListener(StartScanning);
        
        if (stopScanButton != null)
            stopScanButton.onClick.AddListener(StopScanning);
        
        if (toggleFlashButton != null)
            toggleFlashButton.onClick.AddListener(ToggleFlash);
        
        if (showEncyclopediaButton != null)
            showEncyclopediaButton.onClick.AddListener(ShowEncyclopedia);
    }
    
    /// <summary>
    /// 开始扫描
    /// </summary>
    public void StartScanning()
    {
        if (isScanning) return;
        
        isScanning = true;
        
        // 显示扫描界面
        if (scanPanel != null) scanPanel.SetActive(true);
        if (startScanButton != null) startScanButton.gameObject.SetActive(false);
        if (stopScanButton != null) stopScanButton.gameObject.SetActive(true);
        
        // 重置扫描动画
        scanLinePosition = 0f;
        if (scanLine != null) scanLine.gameObject.SetActive(true);
        
        // 更新提示文本
        if (scanHintText != null)
            scanHintText.text = "正在扫描...请将摄像头对准识别图";
        
        // 播放音效
        PlaySound(scanStartSound);
        
        // 开始扫描动画
        StartCoroutine(ScanFramePulse());
        
        Debug.Log("[ARScanUI] 开始AR扫描");
    }
    
    /// <summary>
    /// 停止扫描
    /// </summary>
    public void StopScanning()
    {
        if (!isScanning) return;
        
        isScanning = false;
        
        // 隐藏扫描界面
        if (scanPanel != null) scanPanel.SetActive(false);
        if (startScanButton != null) startScanButton.gameObject.SetActive(true);
        if (stopScanButton != null) stopScanButton.gameObject.SetActive(false);
        if (scanLine != null) scanLine.gameObject.SetActive(false);
        
        // 显示空闲状态
        ShowIdleState();
        
        Debug.Log("[ARScanUI] 停止AR扫描");
    }
    
    /// <summary>
    /// 切换闪光灯
    /// </summary>
    public void ToggleFlash()
    {
        // 这里应该控制设备的闪光灯
        // 暂时只是UI反馈
        Debug.Log("[ARScanUI] 切换闪光灯");
        
        // 可以添加闪光灯图标的状态切换
    }
    
    /// <summary>
    /// 显示动物图鉴
    /// </summary>
    public void ShowEncyclopedia()
    {
        Debug.Log("[ARScanUI] 显示动物图鉴");
        
        // 这里应该打开动物图鉴界面
        // 可以发送事件给其他管理器
    }
    
    /// <summary>
    /// 更新扫描动画
    /// </summary>
    private void UpdateScanAnimation()
    {
        if (scanLine == null) return;
        
        // 移动扫描线
        scanLinePosition += Time.deltaTime * scanLineSpeed;
        
        RectTransform lineRect = scanLine.GetComponent<RectTransform>();
        if (lineRect != null)
        {
            // 在扫描框内上下移动
            float scanFrameHeight = scanFrame != null ? scanFrame.rect.height : 300f;
            float yPos = Mathf.PingPong(scanLinePosition, scanFrameHeight) - scanFrameHeight / 2;
            lineRect.anchoredPosition = new Vector2(0, yPos);
        }
    }
    
    /// <summary>
    /// 扫描框脉动动画
    /// </summary>
    private IEnumerator ScanFramePulse()
    {
        Image frameImage = scanFrame != null ? scanFrame.GetComponent<Image>() : null;
        if (frameImage == null) yield break;
        
        while (isScanning)
        {
            // 脉动效果
            float pulse = (Mathf.Sin(Time.time * scanFramePulseRate * Mathf.PI * 2) + 1) * 0.5f;
            Color currentColor = Color.Lerp(scanFrameInactiveColor, scanFrameActiveColor, pulse);
            frameImage.color = currentColor;
            
            yield return null;
        }
        
        // 恢复原色
        if (frameImage != null)
            frameImage.color = scanFrameInactiveColor;
    }
    
    /// <summary>
    /// 当检测到动物时调用
    /// </summary>
    public void OnAnimalDetected(string animalName, string animalType, Sprite animalIcon = null)
    {
        if (!isScanning) return;
        
        // 显示检测成功UI
        if (animalDetectedText != null)
        {
            animalDetectedText.text = $"检测到: {animalName}";
            animalDetectedText.gameObject.SetActive(true);
        }
        
        if (animalDetectedIcon != null)
        {
            animalDetectedIcon.sprite = animalIcon;
            animalDetectedIcon.gameObject.SetActive(true);
        }
        
        // 播放检测音效
        PlaySound(scanDetectSound);
        
        // 显示动物预览
        ShowAnimalPreview(animalName, animalType, animalIcon);
        
        // 更新扫描提示
        if (scanHintText != null)
            scanHintText.text = $"成功检测到{animalName}！";
        
        Debug.Log($"[ARScanUI] 检测到动物: {animalName} ({animalType})");
    }
    
    /// <summary>
    /// 显示动物预览
    /// </summary>
    private void ShowAnimalPreview(string animalName, string animalType, Sprite animalIcon)
    {
        if (animalPreviewPanel == null) return;
        
        animalPreviewPanel.SetActive(true);
        
        if (previewAnimalName != null)
            previewAnimalName.text = animalName;
        
        if (previewAnimalStatus != null)
            previewAnimalStatus.text = GetConservationStatus(animalType);
        
        if (previewAnimalImage != null && animalIcon != null)
            previewAnimalImage.sprite = animalIcon;
        
        // 3秒后自动隐藏预览
        StartCoroutine(HidePreviewAfterDelay(3f));
    }
    
    /// <summary>
    /// 延迟隐藏预览
    /// </summary>
    private IEnumerator HidePreviewAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (animalPreviewPanel != null)
            animalPreviewPanel.SetActive(false);
    }
    
    /// <summary>
    /// 获取保护状态
    /// </summary>
    private string GetConservationStatus(string animalType)
    {
        switch (animalType.ToLower())
        {
            case "panda":
                return "易危 (Vulnerable)";
            case "tiger":
                return "濒危 (Endangered)";
            case "snow_leopard":
                return "易危 (Vulnerable)";
            case "porpoise":
                return "极危 (Critically Endangered)";
            default:
                return "保护状态未知";
        }
    }
    
    /// <summary>
    /// 显示空闲状态
    /// </summary>
    public void ShowIdleState()
    {
        if (scanHintText != null)
            scanHintText.text = "点击开始扫描按钮，探索濒危动物世界";
        
        if (animalDetectedText != null)
            animalDetectedText.gameObject.SetActive(false);
        
        if (animalDetectedIcon != null)
            animalDetectedIcon.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 更新连接状态
    /// </summary>
    public void UpdateConnectionStatus(bool connected)
    {
        isConnected = connected;
        
        if (connectingIndicator != null)
            connectingIndicator.SetActive(!connected);
        
        if (connectionStatusText != null)
            connectionStatusText.text = connected ? "AR系统就绪" : "连接AR系统...";
        
        if (connectionStatusIcon != null)
            connectionStatusIcon.color = connected ? Color.green : Color.yellow;
        
        // 更新开始扫描按钮状态
        if (startScanButton != null)
            startScanButton.interactable = connected;
    }
    
    /// <summary>
    /// 播放音效
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    /// <summary>
    /// 设置扫描提示
    /// </summary>
    public void SetScanHint(string hint)
    {
        if (scanHintText != null)
            scanHintText.text = hint;
    }
    
    /// <summary>
    /// 是否正在扫描
    /// </summary>
    public bool IsScanning()
    {
        return isScanning;
    }
    
    /// <summary>
    /// 是否已连接
    /// </summary>
    public bool IsConnected()
    {
        return isConnected;
    }
    
    /// <summary>
    /// 当扫描完成时调用
    /// </summary>
    public void OnScanComplete()
    {
        PlaySound(scanCompleteSound);
        
        // 显示完成提示
        if (scanHintText != null)
            scanHintText.text = "扫描完成！";
        
        // 停止扫描
        StartCoroutine(StopScanningAfterDelay(1.5f));
    }
    
    /// <summary>
    /// 延迟停止扫描
    /// </summary>
    private IEnumerator StopScanningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopScanning();
    }
}
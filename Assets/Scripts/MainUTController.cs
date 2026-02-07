using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// AR体验主控制器 - 管理整个濒危动物AR体验流程
/// </summary>
public class MainUTController : MonoBehaviour
{
    [Header("AR系统组件")]
    public ARSession arSession;
    public MonoBehaviour arSessionOrigin; // 改为MonoBehaviour避免类型引用问题
    public ARTrackedImageManager trackedImageManager;
    
    [Header("UI组件")]
    public GameObject scanningPanel;        // 扫描提示面板
    public GameObject interactionPanel;     // 互动面板
    public Text animalNameText;             // 动物名称显示
    public Text animalInfoText;             // 动物信息显示
    public Button chatButton;               // 聊天按钮
    public Button infoButton;               // 信息按钮
    public Button shareButton;              // 分享按钮
    
    [Header("动物对话系统")]
    public AnimalChatManager chatManager;
    
    [Header("特效和音频")]
    public ParticleSystem spawnEffect;      // 生成特效
    public AudioSource audioSource;         // 音频源
    public AudioClip spawnSound;            // 生成音效
    
    // 当前状态
    private bool isARActive = false;
    private GameObject currentAnimal;
    private string currentAnimalType = "";
    
    void Start()
    {
        // 检查API配置
        if (!ConfigLoader.IsAPIConfigValid())
        {
            Debug.LogWarning("[MainUTController] API配置无效，请检查api_config.json文件");
        }
        
        InitializeARSystem();
        SetupUI();
        SetupEventHandlers();
    }
    
    /// <summary>
    /// 初始化AR系统
    /// </summary>
    private void InitializeARSystem()
    {
        if (arSession == null)
        {
            arSession = FindObjectOfType(typeof(ARSession)) as ARSession;
        }
        
        // 注释掉ARSessionOrigin初始化，避免编译错误
        // if (arSessionOrigin == null)
        // {
        //     arSessionOrigin = FindObjectOfType(typeof(ARSessionOrigin)) as ARSessionOrigin;
        // }
        
        if (trackedImageManager == null)
        {
            trackedImageManager = FindObjectOfType(typeof(ARTrackedImageManager)) as ARTrackedImageManager;
        }
        
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
        
        // 启动AR会话
        if (arSession != null)
        {
            arSession.enabled = true;
        }
        
        isARActive = true;
        ShowScanningUI();
    }
    
    /// <summary>
    /// 设置UI组件
    /// </summary>
    private void SetupUI()
    {
        if (scanningPanel != null) scanningPanel.SetActive(true);
        if (interactionPanel != null) interactionPanel.SetActive(false);
        
        // 设置按钮事件
        if (chatButton != null)
        {
            chatButton.onClick.AddListener(OnChatButtonClicked);
        }
        
        if (infoButton != null)
        {
            infoButton.onClick.AddListener(OnInfoButtonClicked);
        }
        
        if (shareButton != null)
        {
            shareButton.onClick.AddListener(OnShareButtonClicked);
        }
    }
    
    /// <summary>
    /// 设置事件处理器
    /// </summary>
    private void SetupEventHandlers()
    {
        // 如果有聊天管理器，设置回调
        if (chatManager != null)
        {
            chatManager.OnResponseReceived += OnChatResponseReceived;
            chatManager.OnError += OnChatError;
        }
    }
    
    /// <summary>
    /// 当追踪图像状态改变时调用
    /// </summary>
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            OnImageDetected(trackedImage);
        }
        
        foreach (var trackedImage in eventArgs.updated)
        {
            OnImageUpdated(trackedImage);
        }
        
        foreach (var trackedImage in eventArgs.removed)
        {
            OnImageRemoved(trackedImage);
        }
    }
    
    /// <summary>
    /// 当检测到图像时
    /// </summary>
    private void OnImageDetected(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        Debug.Log($"[MainUT] 检测到濒危动物图像: {imageName}");
        
        // 隐藏扫描界面，显示互动界面
        ShowInteractionUI();
        
        // 设置当前动物类型
        currentAnimalType = GetAnimalTypeFromImageName(imageName);
        
        // 更新UI信息
        UpdateAnimalInfo(currentAnimalType);
        
        // 如果有聊天管理器，设置动物类型
        if (chatManager != null)
        {
            chatManager.SetAnimalType(currentAnimalType);
            
            // 显示欢迎消息
            string welcomeMsg = chatManager.GetWelcomeMessage();
            Debug.Log($"[MainUT] 欢迎消息: {welcomeMsg}");
        }
        
        // 播放特效和音效
        PlaySpawnEffects(trackedImage.transform.position);
    }
    
    /// <summary>
    /// 当图像更新时（位置变化等）
    /// </summary>
    private void OnImageUpdated(ARTrackedImage trackedImage)
    {
        // 可以在这里添加动画或其他更新逻辑
        if (currentAnimal != null)
        {
            // 确保动物跟随图像移动
            bool isTracking = trackedImage.trackingState == TrackingState.Tracking;
            currentAnimal.SetActive(isTracking);
        }
    }
    
    /// <summary>
    /// 当图像移除时
    /// </summary>
    private void OnImageRemoved(ARTrackedImage trackedImage)
    {
        Debug.Log($"[MainUT] 动物离开视野: {trackedImage.referenceImage.name}");
        
        // 隐藏互动界面，显示扫描界面
        ShowScanningUI();
        
        currentAnimalType = "";
    }
    
    /// <summary>
    /// 从图像名称解析动物类型
    /// </summary>
    private string GetAnimalTypeFromImageName(string imageName)
    {
        // 简单的映射逻辑，可以根据实际图像名称调整
        if (imageName.Contains("panda") || imageName.Contains("熊猫"))
            return "panda";
        else if (imageName.Contains("tiger") || imageName.Contains("老虎"))
            return "tiger";
        else if (imageName.Contains("leopard") || imageName.Contains("雪豹"))
            return "snow_leopard";
        else if (imageName.Contains("porpoise") || imageName.Contains("江豚"))
            return "yangtze_finless_porpoise";
        else
            return "default";
    }
    
    /// <summary>
    /// 更新动物信息显示
    /// </summary>
    private void UpdateAnimalInfo(string animalType)
    {
        if (animalNameText != null)
        {
            animalNameText.text = GetAnimalDisplayName(animalType);
        }
        
        if (animalInfoText != null)
        {
            animalInfoText.text = GetAnimalDescription(animalType);
        }
    }
    
    /// <summary>
    /// 获取动物显示名称
    /// </summary>
    private string GetAnimalDisplayName(string animalType)
    {
        switch (animalType.ToLower())
        {
            case "panda": return "大熊猫";
            case "tiger": return "东北虎";
            case "snow_leopard": return "雪豹";
            case "yangtze_finless_porpoise": return "长江江豚";
            default: return "濒危动物";
        }
    }
    
    /// <summary>
    /// 获取动物描述信息
    /// </summary>
    private string GetAnimalDescription(string animalType)
    {
        switch (animalType.ToLower())
        {
            case "panda":
                return "憨态可掬的大熊猫，中国的国宝，目前野生数量约1864只。主要生活在四川、陕西、甘肃的山区竹林中。";
            case "tiger":
                return "威武的东北虎，世界最大的猫科动物。目前野生数量不足500只，是中国一级保护动物。";
            case "snow_leopard":
                return "神秘的雪豹，被称为'雪山幽灵'，全球仅存约4000只。生活在海拔3000-6000米的高山地区。";
            case "yangtze_finless_porpoise":
                return "长江江豚，长江生态系统的旗舰物种。目前仅存约1012只，被誉为'水中大熊猫'。";
            default:
                return "这是一种珍稀的濒危动物，需要我们共同保护。";
        }
    }
    
    /// <summary>
    /// 播放生成特效
    /// </summary>
    private void PlaySpawnEffects(Vector3 position)
    {
        if (spawnEffect != null)
        {
            var effect = Instantiate(spawnEffect, position, Quaternion.identity);
            Destroy(effect.gameObject, 2f); // 2秒后销毁特效对象
        }
        
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }
    
    /// <summary>
    /// 显示扫描界面
    /// </summary>
    private void ShowScanningUI()
    {
        if (scanningPanel != null) scanningPanel.SetActive(true);
        if (interactionPanel != null) interactionPanel.SetActive(false);
    }
    
    /// <summary>
    /// 显示互动界面
    /// </summary>
    private void ShowInteractionUI()
    {
        if (scanningPanel != null) scanningPanel.SetActive(false);
        if (interactionPanel != null) interactionPanel.SetActive(true);
    }
    
    /// <summary>
    /// 聊天按钮点击事件
    /// </summary>
    private void OnChatButtonClicked()
    {
        if (chatManager != null && !string.IsNullOrEmpty(currentAnimalType))
        {
            // 这里可以弹出聊天界面或开始对话
            Debug.Log("[MainUT] 开始与动物对话");
            
            // 示例：发送一条默认消息
            string defaultMessage = "你好！你能告诉我关于你的故事吗？";
            chatManager.SendMessage(defaultMessage, OnChatSuccess, OnChatFailed);
        }
    }
    
    /// <summary>
    /// 信息按钮点击事件
    /// </summary>
    private void OnInfoButtonClicked()
    {
        if (!string.IsNullOrEmpty(currentAnimalType))
        {
            // 显示更多动物信息
            Debug.Log($"[MainUT] 显示{GetAnimalDisplayName(currentAnimalType)}的详细信息");
            
            // 这里可以弹出更多信息面板
            ShowAnimalDetails();
        }
    }
    
    /// <summary>
    /// 分享按钮点击事件
    /// </summary>
    private void OnShareButtonClicked()
    {
        // 实现分享功能
        Debug.Log("[MainUT] 准备分享当前体验");
        
        // 这里可以截屏并分享
        StartCoroutine(ShareCurrentExperience());
    }
    
    /// <summary>
    /// 显示动物详细信息
    /// </summary>
    private void ShowAnimalDetails()
    {
        string details = GetAnimalDetailedInfo(currentAnimalType);
        Debug.Log($"[MainUT] 动物详细信息: {details}");
        
        // 在实际项目中，这里会更新UI显示详细信息
    }
    
    /// <summary>
    /// 获取动物详细信息
    /// </summary>
    private string GetAnimalDetailedInfo(string animalType)
    {
        switch (animalType.ToLower())
        {
            case "panda":
                return "大熊猫 (Ailuropoda melanoleuca)\n\n" +
                       "栖息地: 中国四川、陕西、甘肃的山区\n" +
                       "食物: 99%为竹子\n" +
                       "保护状态: 易危 (Vulnerable)\n" +
                       "特征: 黑白相间的毛色，圆滚滚的身体";
            case "tiger":
                return "东北虎 (Panthera tigris altaica)\n\n" +
                       "栖息地: 中国东北、俄罗斯远东地区的森林\n" +
                       "食物: 鹿、野猪等大型哺乳动物\n" +
                       "保护状态: 濒危 (Endangered)\n" +
                       "特征: 体型最大的猫科动物，适应寒冷气候";
            case "snow_leopard":
                return "雪豹 (Panthera uncia)\n\n" +
                       "栖息地: 中亚高山地区，海拔3000-6000米\n" +
                       "食物: 岩羊、野兔、鸟类\n" +
                       "保护状态: 易危 (Vulnerable)\n" +
                       "特征: 灰白色毛皮带黑色斑点，尾巴粗长";
            case "yangtze_finless_porpoise":
                return "长江江豚 (Neophocaena asiaeorientalis)\n\n" +
                       "栖息地: 长江中下游干流及洞庭湖、鄱阳湖\n" +
                       "食物: 小鱼、虾类\n" +
                       "保护状态: 极危 (Critically Endangered)\n" +
                       "特征: 微笑的嘴型，无背鳍";
            default:
                return "暂无详细信息";
        }
    }
    
    /// <summary>
    /// 分享当前体验
    /// </summary>
    private IEnumerator ShareCurrentExperience()
    {
        yield return new WaitForEndOfFrame();
        
        // 截取当前屏幕
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();
        
        // 这里可以实现分享到社交媒体的功能
        Debug.Log("[MainUT] 屏幕截图已捕获，准备分享");
        
        // 保存截图到本地（实际项目中）
        // byte[] data = screenshot.EncodeToPNG();
        // string filename = Application.persistentDataPath + "/ar_experience_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        // System.IO.File.WriteAllBytes(filename, data);
        
        Destroy(screenshot);
    }
    
    /// <summary>
    /// 聊天成功回调
    /// </summary>
    private void OnChatSuccess(string response)
    {
        Debug.Log($"[MainUT] 聊天成功: {response}");
        // 这里可以更新UI显示聊天内容
    }
    
    /// <summary>
    /// 聊天失败回调
    /// </summary>
    private void OnChatFailed(string error)
    {
        Debug.LogError($"[MainUT] 聊天失败: {error}");
        // 这里可以显示错误信息给用户
    }
    
    void OnDestroy()
    {
        // 清理事件监听器
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
        
        if (chatManager != null)
        {
            chatManager.OnResponseReceived -= OnChatResponseReceived;
            chatManager.OnError -= OnChatError;
        }
    }
    
    /// <summary>
    /// 聊天响应接收回调
    /// </summary>
    private void OnChatResponseReceived(string response)
    {
        Debug.Log($"[MainUT] 收到聊天响应: {response}");
    }
    
    /// <summary>
    /// 聊天错误回调
    /// </summary>
    private void OnChatError(string error)
    {
        Debug.LogError($"[MainUT] 聊天错误: {error}");
    }
}
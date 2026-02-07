using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 动物UI管理器 - 管理所有动物相关的用户界面
/// 基于Pixso设计稿实现统一的UI体验
/// </summary>
public class AnimalUIManager : MonoBehaviour
{
    [Header("主界面组件")]
    public GameObject mainMenuPanel;           // 主菜单面板
    public GameObject arScanPanel;             // AR扫描面板
    public GameObject animalInfoPanel;         // 动物信息面板
    public GameObject dialoguePanel;           // 对话面板
    public GameObject encyclopediaPanel;       // 动物图鉴面板
    public GameObject settingsPanel;           // 设置面板
    
    [Header("顶部导航栏")]
    public Button homeButton;                  // 首页按钮
    public Button scanButton;                  // 扫描按钮
    public Button encyclopediaButton;          // 图鉴按钮
    public Button settingsButton;              // 设置按钮
    
    [Header("动物信息面板组件")]
    public Text animalNameText;                // 动物名称
    public Text scientificNameText;            // 学名
    public Text conservationStatusText;        // 保护状态
    public Text habitatText;                   // 栖息地
    public Text dietText;                      // 食性
    public Text funFactText;                   // 趣味事实
    public Image animalImage;                  // 动物图片
    public Text protectionTipsText;            // 保护建议
    
    [Header("对话面板组件")]
    public ScrollRect dialogueScrollView;      // 对话滚动视图
    public GameObject messagePrefab;           // 消息预制件
    public InputField messageInputField;       // 消息输入框
    public Button sendButton;                  // 发送按钮
    public Button closeDialogueButton;         // 关闭对话按钮
    
    [Header("动物图鉴组件")]
    public Transform animalCardContainer;      // 动物卡片容器
    public GameObject animalCardPrefab;        // 动物卡片预制件
    public Text encyclopediaTitleText;         // 图鉴标题
    public Button closeEncyclopediaButton;     // 关闭图鉴按钮
    
    [Header("设置面板组件")]
    public Toggle soundToggle;                 // 音效开关
    public Toggle vibrationToggle;             // 震动开关
    public Slider volumeSlider;                // 音量滑块
    public Dropdown languageDropdown;          // 语言下拉框
    public Button closeSettingsButton;         // 关闭设置按钮
    public Button saveSettingsButton;          // 保存设置按钮
    
    [Header("AR扫描提示")]
    public Text scanHintText;                  // 扫描提示文本
    public GameObject scanAnimation;           // 扫描动画
    public Button cancelScanButton;            // 取消扫描按钮
    
    [Header("状态指示器")]
    public GameObject connectionStatusIndicator; // 连接状态指示器
    public Text connectionStatusText;          // 连接状态文本
    public Image connectionStatusIcon;         // 连接状态图标
    
    // 当前状态
    private UIState currentState = UIState.MainMenu;
    private string currentAnimalId = "";
    private List<AnimalCardUI> animalCards = new List<AnimalCardUI>();
    
    // 颜色配置
    private Color normalButtonColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private Color selectedButtonColor = new Color(0f, 0.5f, 0.8f, 1f);
    
    /// <summary>
    /// UI状态枚举
    /// </summary>
    public enum UIState
    {
        MainMenu,           // 主菜单
        ARScan,             // AR扫描
        AnimalInfo,         // 动物信息
        Dialogue,           // 对话
        Encyclopedia,       // 动物图鉴
        Settings            // 设置
    }
    
    void Start()
    {
        // 自动查找关键UI组件（如果未在编辑器中设置）
        AutoFindCriticalComponents();
        
        InitializeUI();
        SetupEventHandlers();
        LoadAnimalData();
        ShowMainMenu();
    }
    
    /// <summary>
    /// 自动查找关键UI组件（备用方案）
    /// </summary>
    private void AutoFindCriticalComponents()
    {
        Debug.Log("[AnimalUIManager] 开始自动查找关键UI组件...");
        
        // 如果主菜单面板未设置，尝试通过名称查找
        if (mainMenuPanel == null)
        {
            mainMenuPanel = GameObject.Find("MainMenuPanel");
            if (mainMenuPanel == null) mainMenuPanel = GameObject.Find("MainMenu");
            if (mainMenuPanel == null) mainMenuPanel = GameObject.Find("MainMenuUI");
            
            if (mainMenuPanel != null)
            {
                Debug.Log($"[AnimalUIManager] 自动查找到主菜单面板: {mainMenuPanel.name}");
            }
            else
            {
                Debug.LogWarning("[AnimalUIManager] 警告：未找到主菜单面板！初始界面可能无法显示。");
                Debug.LogWarning("请在Unity编辑器中检查以下内容：");
                Debug.LogWarning("1. 确保场景中有名为 MainMenuPanel 或类似名称的GameObject");
                Debug.LogWarning("2. 或者将主菜单面板拖拽到AnimalUIManager脚本的mainMenuPanel字段");
            }
        }
        
        // 尝试查找导航按钮（如果为null）
        if (homeButton == null) homeButton = FindButton("HomeButton", "HomeBtn", "Home");
        if (scanButton == null) scanButton = FindButton("ScanButton", "ScanBtn", "Scan");
        if (encyclopediaButton == null) encyclopediaButton = FindButton("EncyclopediaButton", "EncyclopediaBtn", "Encyclopedia");
        if (settingsButton == null) settingsButton = FindButton("SettingsButton", "SettingsBtn", "Settings");
        
        // 检查关键组件
        if (mainMenuPanel == null)
        {
            Debug.LogError("[AnimalUIManager] 严重错误：主菜单面板未找到！");
            Debug.LogError("初始界面将无法显示，请检查UI配置。");
        }
        else
        {
            Debug.Log("[AnimalUIManager] 关键UI组件检查完成");
        }
    }
    
    /// <summary>
    /// 查找按钮的辅助方法
    /// </summary>
    private Button FindButton(params string[] possibleNames)
    {
        foreach (var name in possibleNames)
        {
            GameObject go = GameObject.Find(name);
            if (go != null)
            {
                Button btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    Debug.Log($"[AnimalUIManager] 找到按钮: {name}");
                    return btn;
                }
            }
        }
        return null;
    }
    
    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitializeUI()
    {
        // 隐藏所有面板，只显示主菜单
        SetPanelActive(mainMenuPanel, false);
        SetPanelActive(arScanPanel, false);
        SetPanelActive(animalInfoPanel, false);
        SetPanelActive(dialoguePanel, false);
        SetPanelActive(encyclopediaPanel, false);
        SetPanelActive(settingsPanel, false);
        
        // 设置初始连接状态
        UpdateConnectionStatus(false);
    }
    
    /// <summary>
    /// 设置事件处理器
    /// </summary>
    private void SetupEventHandlers()
    {
        // 导航栏按钮
        if (homeButton != null) homeButton.onClick.AddListener(ShowMainMenu);
        if (scanButton != null) scanButton.onClick.AddListener(ShowARScan);
        if (encyclopediaButton != null) encyclopediaButton.onClick.AddListener(ShowEncyclopedia);
        if (settingsButton != null) settingsButton.onClick.AddListener(ShowSettings);
        
        // 对话相关
        if (sendButton != null) sendButton.onClick.AddListener(OnSendMessage);
        if (closeDialogueButton != null) closeDialogueButton.onClick.AddListener(ShowMainMenu);
        
        // 图鉴相关
        if (closeEncyclopediaButton != null) closeEncyclopediaButton.onClick.AddListener(ShowMainMenu);
        
        // 设置相关
        if (closeSettingsButton != null) closeSettingsButton.onClick.AddListener(ShowMainMenu);
        if (saveSettingsButton != null) saveSettingsButton.onClick.AddListener(SaveSettings);
        
        // AR扫描相关
        if (cancelScanButton != null) cancelScanButton.onClick.AddListener(ShowMainMenu);
        
        // 输入框回车发送
        if (messageInputField != null)
        {
            messageInputField.onEndEdit.AddListener((text) => {
                // 检查是否按下了回车键（新Input System兼容方案）
                // 当文本不为空且非空白字符时，认为用户按下了回车
                if (!string.IsNullOrWhiteSpace(text))
                {
                    OnSendMessage();
                }
            });
        }
    }
    
    /// <summary>
    /// 加载动物数据
    /// </summary>
    private void LoadAnimalData()
    {
        // 这里可以连接AnimalConfigManager或其他数据源
        // 暂时使用示例数据
        CreateSampleAnimalCards();
    }
    
    /// <summary>
    /// 创建示例动物卡片
    /// </summary>
    private void CreateSampleAnimalCards()
    {
        // 清空现有卡片
        foreach (var card in animalCards)
        {
            if (card != null && card.gameObject != null)
                Destroy(card.gameObject);
        }
        animalCards.Clear();
        
        // 示例动物数据
        var sampleAnimals = new List<AnimalCardData>
        {
            new AnimalCardData
            {
                animalId = "panda",
                displayName = "大熊猫",
                scientificName = "Ailuropoda melanoleuca",
                conservationStatus = "易危 (VU)",
                habitat = "中国四川、陕西、甘肃的竹林中",
                thumbnailPath = "Animals/Panda_Thumbnail"
            },
            new AnimalCardData
            {
                animalId = "tiger",
                displayName = "东北虎",
                scientificName = "Panthera tigris altaica",
                conservationStatus = "濒危 (EN)",
                habitat = "俄罗斯远东地区和中国东北",
                thumbnailPath = "Animals/Tiger_Thumbnail"
            },
            new AnimalCardData
            {
                animalId = "snow_leopard",
                displayName = "雪豹",
                scientificName = "Panthera uncia",
                conservationStatus = "易危 (VU)",
                habitat = "中亚高山地区",
                thumbnailPath = "Animals/SnowLeopard_Thumbnail"
            },
            new AnimalCardData
            {
                animalId = "porpoise",
                displayName = "长江江豚",
                scientificName = "Neophocaena asiaeorientalis",
                conservationStatus = "极危 (CR)",
                habitat = "长江中下游流域",
                thumbnailPath = "Animals/Porpoise_Thumbnail"
            }
        };
        
        // 创建卡片
        foreach (var animalData in sampleAnimals)
        {
            CreateAnimalCard(animalData);
        }
    }
    
    /// <summary>
    /// 创建动物卡片
    /// </summary>
    private void CreateAnimalCard(AnimalCardData data)
    {
        if (animalCardPrefab == null || animalCardContainer == null) return;
        
        GameObject cardObject = Instantiate(animalCardPrefab, animalCardContainer);
        AnimalCardUI cardUI = cardObject.GetComponent<AnimalCardUI>();
        
        if (cardUI != null)
        {
            cardUI.Initialize(data, this);
            animalCards.Add(cardUI);
        }
    }
    
    /// <summary>
    /// 显示主菜单
    /// </summary>
    public void ShowMainMenu()
    {
        SwitchState(UIState.MainMenu);
        UpdateNavigationButtonColors(0); // 首页按钮高亮
    }
    
    /// <summary>
    /// 显示AR扫描界面
    /// </summary>
    public void ShowARScan()
    {
        SwitchState(UIState.ARScan);
        UpdateNavigationButtonColors(1); // 扫描按钮高亮
        
        // 开始扫描动画
        if (scanAnimation != null)
            scanAnimation.SetActive(true);
        
        // 显示扫描提示
        if (scanHintText != null)
            scanHintText.text = "请将摄像头对准濒危动物识别图...";
    }
    
    /// <summary>
    /// 显示动物信息
    /// </summary>
    public void ShowAnimalInfo(string animalId)
    {
        currentAnimalId = animalId;
        SwitchState(UIState.AnimalInfo);
        UpdateNavigationButtonColors(-1); // 无高亮
        
        // 加载并显示动物信息
        LoadAndDisplayAnimalInfo(animalId);
    }
    
    /// <summary>
    /// 显示对话界面
    /// </summary>
    public void ShowDialogue(string animalId)
    {
        currentAnimalId = animalId;
        SwitchState(UIState.Dialogue);
        UpdateNavigationButtonColors(-1); // 无高亮
        
        // 清空对话历史
        ClearDialogueHistory();
        
        // 显示欢迎消息
        AddMessageToDialogue("系统", $"你好！我是{GetAnimalName(animalId)}，很高兴与你对话！", true);
    }
    
    /// <summary>
    /// 显示动物图鉴
    /// </summary>
    public void ShowEncyclopedia()
    {
        SwitchState(UIState.Encyclopedia);
        UpdateNavigationButtonColors(2); // 图鉴按钮高亮
        
        if (encyclopediaTitleText != null)
            encyclopediaTitleText.text = "濒危动物图鉴";
    }
    
    /// <summary>
    /// 显示设置界面
    /// </summary>
    public void ShowSettings()
    {
        SwitchState(UIState.Settings);
        UpdateNavigationButtonColors(3); // 设置按钮高亮
        
        // 加载当前设置
        LoadCurrentSettings();
    }
    
    /// <summary>
    /// 切换UI状态
    /// </summary>
    private void SwitchState(UIState newState)
    {
        // 隐藏当前面板
        SetPanelActive(mainMenuPanel, false);
        SetPanelActive(arScanPanel, false);
        SetPanelActive(animalInfoPanel, false);
        SetPanelActive(dialoguePanel, false);
        SetPanelActive(encyclopediaPanel, false);
        SetPanelActive(settingsPanel, false);
        
        // 显示新面板
        switch (newState)
        {
            case UIState.MainMenu:
                if (mainMenuPanel != null)
                {
                    SetPanelActive(mainMenuPanel, true);
                    Debug.Log($"[AnimalUIManager] 显示主菜单面板: {mainMenuPanel.name}");
                }
                else
                {
                    Debug.LogError("[AnimalUIManager] 错误：无法显示主菜单 - mainMenuPanel为null！");
                    // 尝试再次自动查找
                    AutoFindCriticalComponents();
                    
                    if (mainMenuPanel != null)
                    {
                        SetPanelActive(mainMenuPanel, true);
                        Debug.Log($"[AnimalUIManager] 重新查找后显示主菜单面板");
                    }
                    else
                    {
                        Debug.LogError("[AnimalUIManager] 严重错误：主菜单面板仍未找到！");
                        // 尝试显示其他面板作为备用方案
                        if (arScanPanel != null)
                        {
                            SetPanelActive(arScanPanel, true);
                            Debug.LogWarning("[AnimalUIManager] 显示AR扫描面板作为备用方案");
                            newState = UIState.ARScan;
                        }
                    }
                }
                break;
            case UIState.ARScan:
                SetPanelActive(arScanPanel, true);
                break;
            case UIState.AnimalInfo:
                SetPanelActive(animalInfoPanel, true);
                break;
            case UIState.Dialogue:
                SetPanelActive(dialoguePanel, true);
                break;
            case UIState.Encyclopedia:
                SetPanelActive(encyclopediaPanel, true);
                break;
            case UIState.Settings:
                SetPanelActive(settingsPanel, true);
                break;
        }
        
        currentState = newState;
        Debug.Log($"[AnimalUIManager] 切换到状态: {newState}");
    }
    
    /// <summary>
    /// 更新导航按钮颜色
    /// </summary>
    private void UpdateNavigationButtonColors(int selectedIndex)
    {
        // 0=首页, 1=扫描, 2=图鉴, 3=设置
        ColorBlock colors;
        
        // 首页按钮
        colors = homeButton != null ? homeButton.colors : default;
        colors.normalColor = (selectedIndex == 0) ? selectedButtonColor : normalButtonColor;
        if (homeButton != null) homeButton.colors = colors;
        
        // 扫描按钮
        colors = scanButton != null ? scanButton.colors : default;
        colors.normalColor = (selectedIndex == 1) ? selectedButtonColor : normalButtonColor;
        if (scanButton != null) scanButton.colors = colors;
        
        // 图鉴按钮
        colors = encyclopediaButton != null ? encyclopediaButton.colors : default;
        colors.normalColor = (selectedIndex == 2) ? selectedButtonColor : normalButtonColor;
        if (encyclopediaButton != null) encyclopediaButton.colors = colors;
        
        // 设置按钮
        colors = settingsButton != null ? settingsButton.colors : default;
        colors.normalColor = (selectedIndex == 3) ? selectedButtonColor : normalButtonColor;
        if (settingsButton != null) settingsButton.colors = colors;
    }
    
    /// <summary>
    /// 加载并显示动物信息
    /// </summary>
    private void LoadAndDisplayAnimalInfo(string animalId)
    {
        // 这里应该从AnimalConfigManager或AnimalPrefabManager获取数据
        // 暂时使用示例数据
        
        switch (animalId.ToLower())
        {
            case "panda":
                SetAnimalInfo(
                    "大熊猫",
                    "Ailuropoda melanoleuca",
                    "易危 (VU)",
                    "中国四川、陕西、甘肃的竹林中",
                    "99%竹子，偶尔吃昆虫或小动物",
                    "大熊猫每天要花12-16小时吃竹子",
                    "保护竹林生态系统，支持大熊猫保护区建设"
                );
                break;
                
            case "tiger":
                SetAnimalInfo(
                    "东北虎",
                    "Panthera tigris altaica",
                    "濒危 (EN)",
                    "俄罗斯远东地区和中国东北",
                    "大型有蹄类动物（鹿、野猪等）",
                    "东北虎是现存最大的猫科动物",
                    "打击非法盗猎，保护森林栖息地"
                );
                break;
                
            default:
                SetAnimalInfo(
                    "未知动物",
                    "Unknown",
                    "未知",
                    "未知",
                    "未知",
                    "暂无趣味事实",
                    "请保护野生动物"
                );
                break;
        }
    }
    
    /// <summary>
    /// 设置动物信息
    /// </summary>
    private void SetAnimalInfo(string name, string scientific, string status, 
                              string habitat, string diet, string funFact, string tips)
    {
        if (animalNameText != null) animalNameText.text = name;
        if (scientificNameText != null) scientificNameText.text = scientific;
        if (conservationStatusText != null) conservationStatusText.text = status;
        if (habitatText != null) habitatText.text = habitat;
        if (dietText != null) dietText.text = diet;
        if (funFactText != null) funFactText.text = funFact;
        if (protectionTipsText != null) protectionTipsText.text = tips;
    }
    
    /// <summary>
    /// 发送消息
    /// </summary>
    private void OnSendMessage()
    {
        if (messageInputField == null || string.IsNullOrWhiteSpace(messageInputField.text))
            return;
        
        string message = messageInputField.text.Trim();
        
        // 添加用户消息
        AddMessageToDialogue("你", message, false);
        
        // 清空输入框
        messageInputField.text = "";
        
        // 模拟AI回复（实际应该调用AnimalChatManager）
        StartCoroutine(SendMockAIResponse(message));
    }
    
    /// <summary>
    /// 发送模拟AI回复
    /// </summary>
    private IEnumerator SendMockAIResponse(string userMessage)
    {
        // 模拟网络延迟
        yield return new WaitForSeconds(1f);
        
        string animalName = GetAnimalName(currentAnimalId);
        string response = GenerateMockResponse(userMessage, animalName);
        
        AddMessageToDialogue(animalName, response, true);
    }
    
    /// <summary>
    /// 添加消息到对话
    /// </summary>
    private void AddMessageToDialogue(string sender, string message, bool isAI)
    {
        if (messagePrefab == null || dialogueScrollView == null || 
            dialogueScrollView.content == null) return;
        
        GameObject messageObject = Instantiate(messagePrefab, dialogueScrollView.content);
        MessageUI messageUI = messageObject.GetComponent<MessageUI>();
        
        if (messageUI != null)
        {
            messageUI.SetMessage(sender, message, isAI);
        }
        
        // 滚动到底部
        Canvas.ForceUpdateCanvases();
        dialogueScrollView.verticalNormalizedPosition = 0f;
    }
    
    /// <summary>
    /// 清空对话历史
    /// </summary>
    private void ClearDialogueHistory()
    {
        if (dialogueScrollView == null || dialogueScrollView.content == null) return;
        
        foreach (Transform child in dialogueScrollView.content)
        {
            Destroy(child.gameObject);
        }
    }
    
    /// <summary>
    /// 生成模拟回复
    /// </summary>
    private string GenerateMockResponse(string userMessage, string animalName)
    {
        // 简单的关键词匹配
        userMessage = userMessage.ToLower();
        
        if (userMessage.Contains("你好") || userMessage.Contains("hi") || userMessage.Contains("hello"))
            return $"你好！我是{animalName}，很高兴认识你！";
        
        if (userMessage.Contains("吃什么") || userMessage.Contains("食物"))
            return "我主要以竹子为食，每天要吃很多竹子呢！";
        
        if (userMessage.Contains("住哪里") || userMessage.Contains("栖息地"))
            return "我生活在中国西南部的竹林里，那里是我的家园。";
        
        if (userMessage.Contains("保护") || userMessage.Contains("濒危"))
            return "是的，我们是濒危物种，需要大家的保护。请不要破坏我们的栖息地！";
        
        if (userMessage.Contains("谢谢") || userMessage.Contains("感谢"))
            return "不客气！很高兴能和你交流！";
        
        // 默认回复
        return $"这个问题很有意思！作为{animalName}，我很乐意和你聊天。关于这个话题，你有什么具体想了解的吗？";
    }
    
    /// <summary>
    /// 获取动物名称
    /// </summary>
    private string GetAnimalName(string animalId)
    {
        switch (animalId.ToLower())
        {
            case "panda": return "大熊猫";
            case "tiger": return "东北虎";
            case "snow_leopard": return "雪豹";
            case "porpoise": return "长江江豚";
            default: return "动物朋友";
        }
    }
    
    /// <summary>
    /// 加载当前设置
    /// </summary>
    private void LoadCurrentSettings()
    {
        // 这里应该从PlayerPrefs或配置文件加载
        // 暂时使用默认值
        
        if (soundToggle != null) soundToggle.isOn = true;
        if (vibrationToggle != null) vibrationToggle.isOn = true;
        if (volumeSlider != null) volumeSlider.value = 0.8f;
        
        // 设置语言选项
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(new List<string> { "简体中文", "English", "日本語" });
            languageDropdown.value = 0;
        }
    }
    
    /// <summary>
    /// 保存设置
    /// </summary>
    private void SaveSettings()
    {
        // 保存到PlayerPrefs
        if (soundToggle != null) PlayerPrefs.SetInt("SoundEnabled", soundToggle.isOn ? 1 : 0);
        if (vibrationToggle != null) PlayerPrefs.SetInt("VibrationEnabled", vibrationToggle.isOn ? 1 : 0);
        if (volumeSlider != null) PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        if (languageDropdown != null) PlayerPrefs.SetInt("Language", languageDropdown.value);
        
        PlayerPrefs.Save();
        
        // 显示保存成功提示
        Debug.Log("[AnimalUIManager] 设置已保存");
        
        // 返回主菜单
        ShowMainMenu();
    }
    
    /// <summary>
    /// 更新连接状态
    /// </summary>
    public void UpdateConnectionStatus(bool isConnected)
    {
        if (connectionStatusIndicator == null) return;
        
        connectionStatusIndicator.SetActive(!isConnected);
        
        if (connectionStatusText != null)
            connectionStatusText.text = isConnected ? "已连接" : "连接中...";
        
        if (connectionStatusIcon != null)
            connectionStatusIcon.color = isConnected ? Color.green : Color.yellow;
    }
    
    /// <summary>
    /// 当检测到动物图像时调用
    /// </summary>
    public void OnAnimalDetected(string animalId, string animalName)
    {
        // 停止扫描动画
        if (scanAnimation != null)
            scanAnimation.SetActive(false);
        
        // 显示动物信息
        ShowAnimalInfo(animalId);
        
        // 显示检测成功提示
        if (scanHintText != null)
            scanHintText.text = $"检测到{animalName}！";
    }
    
    /// <summary>
    /// 设置面板激活状态
    /// </summary>
    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }
    
    /// <summary>
    /// 获取当前UI状态
    /// </summary>
    public UIState GetCurrentState()
    {
        return currentState;
    }
}

/// <summary>
/// 动物卡片数据
/// </summary>
public class AnimalCardData
{
    public string animalId;
    public string displayName;
    public string scientificName;
    public string conservationStatus;
    public string habitat;
    public string thumbnailPath;
}

/// <summary>
/// 动物卡片UI组件
/// </summary>
public class AnimalCardUI : MonoBehaviour
{
    public Text animalNameText;
    public Text scientificNameText;
    public Text statusText;
    public Image animalImage;
    public Button cardButton;
    
    private AnimalCardData data;
    private AnimalUIManager uiManager;
    
    public void Initialize(AnimalCardData cardData, AnimalUIManager manager)
    {
        data = cardData;
        uiManager = manager;
        
        if (animalNameText != null) animalNameText.text = cardData.displayName;
        if (scientificNameText != null) scientificNameText.text = cardData.scientificName;
        if (statusText != null) statusText.text = cardData.conservationStatus;
        
        // 加载图片（示例，实际需要资源加载）
        // if (animalImage != null) animalImage.sprite = Resources.Load<Sprite>(cardData.thumbnailPath);
        
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnCardClicked);
        }
    }
    
    private void OnCardClicked()
    {
        if (uiManager != null && data != null)
        {
            uiManager.ShowAnimalInfo(data.animalId);
        }
    }
}

/// <summary>
/// 消息UI组件
/// </summary>
public class MessageUI : MonoBehaviour
{
    public Text senderText;
    public Text messageText;
    public Image backgroundImage;
    public RectTransform messageContainer;
    
    // AI消息和用户消息的不同样式
    public Color aiMessageColor = new Color(0.9f, 0.95f, 1f, 1f);
    public Color userMessageColor = new Color(0.8f, 1f, 0.8f, 1f);
    
    public void SetMessage(string sender, string message, bool isAI)
    {
        if (senderText != null) senderText.text = sender;
        if (messageText != null) messageText.text = message;
        
        if (backgroundImage != null)
            backgroundImage.color = isAI ? aiMessageColor : userMessageColor;
        
        // 自动调整大小
        if (messageContainer != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageContainer);
        }
    }
}
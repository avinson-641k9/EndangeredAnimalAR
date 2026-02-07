using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 主菜单管理器 - 解决白屏问题，提供清晰的主界面架构
/// 1. 确保Canvas和EventSystem正确设置
/// 2. 提供四个核心功能入口
/// 3. 实现模块化跳转框架
/// 4. 解决UI显示和交互的基础问题
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Canvas配置 - 必须正确设置以解决白屏问题")]
    [Tooltip("Canvas组件引用，用于设置渲染模式")]
    public Canvas mainCanvas;
    
    [Header("UI组件引用")]
    public Button startARButton;
    public Button aiChatButton;
    public Button tasksButton;
    public Button encyclopediaButton;
    
    [Header("UI状态显示")]
    public Text statusText;
    public GameObject loadingIndicator;
    
    [Header("视觉风格配置")]
    [Tooltip("主色调 - 生态保护绿色主题")]
    public Color primaryColor = new Color(0.2f, 0.6f, 0.3f, 1f); // 清新绿色
    public Color buttonNormalColor = new Color(0.9f, 0.95f, 0.9f, 1f);
    public Color buttonHighlightColor = new Color(0.4f, 0.8f, 0.5f, 1f);
    
    // 单例实例
    private static MainMenuManager _instance;
    public static MainMenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainMenuManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("MainMenuManager");
                    _instance = obj.AddComponent<MainMenuManager>();
                }
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        // 确保单例
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 第一步：确保Canvas正确配置（解决白屏的核心）
        EnsureCanvasConfiguration();
        
        // 第二步：确保EventSystem存在（解决按钮无法点击）
        EnsureEventSystem();
    }
    
    void Start()
    {
        // 初始化UI
        InitializeUI();
        
        // 设置按钮事件
        SetupButtonEvents();
        
        // 更新状态显示
        UpdateStatus("主菜单已就绪");
        
        Debug.Log("[MainMenuManager] 主菜单系统初始化完成，白屏问题已解决");
    }
    
    /// <summary>
    /// 确保Canvas正确配置 - 这是解决白屏问题的关键
    /// </summary>
    private void EnsureCanvasConfiguration()
    {
        // 查找或创建Canvas
        if (mainCanvas == null)
        {
            mainCanvas = GetComponent<Canvas>();
            if (mainCanvas == null)
            {
                mainCanvas = gameObject.AddComponent<Canvas>();
                Debug.LogWarning("[MainMenuManager] Canvas未找到，已自动创建");
            }
        }
        
        // 必须设置Canvas的Render Mode为Screen Space - Overlay（确保UI显示）
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // 添加Canvas Scaler确保UI适配不同分辨率
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            Debug.Log("[MainMenuManager] 已添加CanvasScaler以确保UI适配");
        }
        
        // 添加Graphic Raycaster确保UI可交互
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
            Debug.Log("[MainMenuManager] 已添加GraphicRaycaster以确保UI交互");
        }
        
        Debug.Log($"[MainMenuManager] Canvas配置完成: RenderMode={mainCanvas.renderMode}, PixelPerfect={mainCanvas.pixelPerfect}");
    }
    
    /// <summary>
    /// 确保EventSystem存在 - 解决按钮无法点击的问题
    /// </summary>
    private void EnsureEventSystem()
    {
        // 检查场景中是否存在EventSystem
        UnityEngine.EventSystems.EventSystem eventSystem = 
            FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        
        if (eventSystem == null)
        {
            // 创建EventSystem对象
            GameObject eventSystemObj = new GameObject("EventSystem");
            
            // 添加EventSystem组件
            eventSystem = eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            
            // 添加StandaloneInputModule组件（标准输入模块）
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            Debug.LogWarning("[MainMenuManager] EventSystem未找到，已自动创建");
        }
        else
        {
            // 确保有StandaloneInputModule
            if (eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("[MainMenuManager] 已添加StandaloneInputModule到现有EventSystem");
            }
        }
        
        Debug.Log($"[MainMenuManager] EventSystem状态: {(eventSystem != null ? "正常" : "异常")}");
    }
    
    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitializeUI()
    {
        // 自动查找按钮（备用方案）
        if (startARButton == null) startARButton = FindButton("StartARButton", "开始探索");
        if (aiChatButton == null) aiChatButton = FindButton("AIChatButton", "智能助手");
        if (tasksButton == null) tasksButton = FindButton("TasksButton", "科普任务");
        if (encyclopediaButton == null) encyclopediaButton = FindButton("EncyclopediaButton", "物种图鉴");
        
        // 检查UI组件状态
        CheckUIComponents();
        
        // 应用视觉风格
        ApplyVisualStyle();
    }
    
    /// <summary>
    /// 查找按钮的辅助方法
    /// </summary>
    private Button FindButton(string buttonName, string displayName)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj != null)
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                Debug.Log($"[MainMenuManager] 找到按钮: {displayName}");
                return button;
            }
        }
        
        Debug.LogWarning($"[MainMenuManager] 未找到按钮: {displayName}，请检查UI层级");
        return null;
    }
    
    /// <summary>
    /// 检查UI组件状态
    /// </summary>
    private void CheckUIComponents()
    {
        int missingCount = 0;
        
        if (startARButton == null)
        {
            Debug.LogError("[MainMenuManager] 错误：开始探索按钮未找到！");
            missingCount++;
        }
        
        if (aiChatButton == null)
        {
            Debug.LogError("[MainMenuManager] 错误：智能助手按钮未找到！");
            missingCount++;
        }
        
        if (tasksButton == null)
        {
            Debug.LogError("[MainMenuManager] 错误：科普任务按钮未找到！");
            missingCount++;
        }
        
        if (encyclopediaButton == null)
        {
            Debug.LogError("[MainMenuManager] 错误：物种图鉴按钮未找到！");
            missingCount++;
        }
        
        if (missingCount > 0)
        {
            Debug.LogError($"[MainMenuManager] 共缺少 {missingCount} 个关键UI组件，可能导致功能不完整");
            Debug.LogError("请检查以下UI组件是否存在于场景中：");
            Debug.LogError("1. 开始探索按钮 (StartARButton)");
            Debug.LogError("2. 智能助手按钮 (AIChatButton)");
            Debug.LogError("3. 科普任务按钮 (TasksButton)");
            Debug.LogError("4. 物种图鉴按钮 (EncyclopediaButton)");
        }
        else
        {
            Debug.Log("[MainMenuManager] 所有关键UI组件检查正常");
        }
    }
    
    /// <summary>
    /// 应用视觉风格
    /// </summary>
    private void ApplyVisualStyle()
    {
        // 设置按钮颜色（生态保护主题）
        ColorBlock colors = new ColorBlock
        {
            normalColor = buttonNormalColor,
            highlightedColor = buttonHighlightColor,
            pressedColor = primaryColor,
            disabledColor = Color.gray,
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };
        
        if (startARButton != null) startARButton.colors = colors;
        if (aiChatButton != null) aiChatButton.colors = colors;
        if (tasksButton != null) tasksButton.colors = colors;
        if (encyclopediaButton != null) encyclopediaButton.colors = colors;
        
        Debug.Log("[MainMenuManager] 视觉风格已应用（生态保护绿色主题）");
    }
    
    /// <summary>
    /// 设置按钮事件
    /// </summary>
    private void SetupButtonEvents()
    {
        // 清除现有事件监听器（避免重复绑定）
        if (startARButton != null)
        {
            startARButton.onClick.RemoveAllListeners();
            startARButton.onClick.AddListener(LoadARScene);
        }
        
        if (aiChatButton != null)
        {
            aiChatButton.onClick.RemoveAllListeners();
            aiChatButton.onClick.AddListener(OpenChatPanel);
        }
        
        if (tasksButton != null)
        {
            tasksButton.onClick.RemoveAllListeners();
            tasksButton.onClick.AddListener(OpenTasksPanel);
        }
        
        if (encyclopediaButton != null)
        {
            encyclopediaButton.onClick.RemoveAllListeners();
            encyclopediaButton.onClick.AddListener(OpenEncyclopediaPanel);
        }
        
        Debug.Log("[MainMenuManager] 按钮事件绑定完成");
    }
    
    /// <summary>
    /// 更新状态显示
    /// </summary>
    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        
        Debug.Log($"[MainMenuManager] 状态更新: {message}");
    }
    
    /// <summary>
    /// 显示/隐藏加载指示器
    /// </summary>
    private void SetLoading(bool isLoading)
    {
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(isLoading);
        }
        
        if (isLoading)
        {
            UpdateStatus("加载中...");
        }
    }
    
    // ========================
    // 核心功能入口（模块化框架）
    // ========================
    
    /// <summary>
    /// 进入AR探索场景
    /// </summary>
    public void LoadARScene()
    {
        Debug.Log("进入AR探索场景...");
        UpdateStatus("准备进入AR探索...");
        SetLoading(true);
        
        // 记录用户行为
        LogUserAction("进入AR探索");
        
        // 这里应该是场景加载逻辑
        // SceneManager.LoadScene("ARScene");
        
        // 模拟加载延迟
        StartCoroutine(SimulateLoading(() => {
            Debug.Log("[MainMenuManager] AR场景加载完成（占位实现）");
            UpdateStatus("AR场景已就绪");
            SetLoading(false);
            
            // 实际项目中这里会跳转到AR场景
            // 现在用调试消息代替
            Debug.LogWarning("[MainMenuManager] 注意：AR场景跳转功能待实现");
        }));
    }
    
    /// <summary>
    /// 打开智能助手面板
    /// </summary>
    public void OpenChatPanel()
    {
        Debug.Log("打开智能助手面板...");
        UpdateStatus("启动AI对话系统...");
        
        // 记录用户行为
        LogUserAction("打开智能助手");
        
        // 这里应该是打开聊天界面的逻辑
        // 现在用调试消息代替具体实现
        
        // 模拟打开面板
        StartCoroutine(SimulateAction(() => {
            Debug.Log("[MainMenuManager] AI聊天面板已打开（占位实现）");
            UpdateStatus("可以开始与动物对话了");
            
            // 实际项目中这里会显示聊天UI
            Debug.LogWarning("[MainMenuManager] 注意：AI聊天功能待实现");
        }));
    }
    
    /// <summary>
    /// 打开科普任务面板
    /// </summary>
    public void OpenTasksPanel()
    {
        Debug.Log("打开科普任务面板...");
        UpdateStatus("加载任务系统...");
        
        // 记录用户行为
        LogUserAction("打开科普任务");
        
        // 这里应该是打开任务界面的逻辑
        
        StartCoroutine(SimulateAction(() => {
            Debug.Log("[MainMenuManager] 任务系统已加载（占位实现）");
            UpdateStatus("查看你的任务和积分");
            
            Debug.LogWarning("[MainMenuManager] 注意：任务系统功能待实现");
        }));
    }
    
    /// <summary>
    /// 打开物种图鉴面板
    /// </summary>
    public void OpenEncyclopediaPanel()
    {
        Debug.Log("打开物种图鉴面板...");
        UpdateStatus("加载动物图鉴...");
        
        // 记录用户行为
        LogUserAction("打开物种图鉴");
        
        // 这里应该是打开图鉴界面的逻辑
        
        StartCoroutine(SimulateAction(() => {
            Debug.Log("[MainMenuManager] 动物图鉴已加载（占位实现）");
            UpdateStatus("探索濒危动物的奇妙世界");
            
            Debug.LogWarning("[MainMenuManager] 注意：图鉴系统功能待实现");
        }));
    }
    
    /// <summary>
    /// 模拟加载过程的协程
    /// </summary>
    private IEnumerator SimulateLoading(System.Action onComplete)
    {
        // 模拟加载时间
        yield return new WaitForSeconds(1.5f);
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 模拟操作的协程
    /// </summary>
    private IEnumerator SimulateAction(System.Action onComplete)
    {
        yield return new WaitForSeconds(0.5f);
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 记录用户行为（用于分析）
    /// </summary>
    private void LogUserAction(string action)
    {
        // 实际项目中可以发送到分析服务器
        Debug.Log($"[用户行为] {action} - 时间: {System.DateTime.Now}");
    }
    
    /// <summary>
    /// 公共方法：重新检查UI状态（用于调试）
    /// </summary>
    public void RecheckUIState()
    {
        Debug.Log("[MainMenuManager] 重新检查UI状态...");
        EnsureCanvasConfiguration();
        EnsureEventSystem();
        CheckUIComponents();
        UpdateStatus("UI状态重新检查完成");
    }
    
    /// <summary>
    /// 公共方法：显示调试信息
    /// </summary>
    public void ShowDebugInfo()
    {
        string debugInfo = $"主菜单系统状态报告:\n" +
                          $"Canvas: {mainCanvas.name} (RenderMode: {mainCanvas.renderMode})\n" +
                          $"按钮状态: AR[{startARButton != null}] AI[{aiChatButton != null}] " +
                          $"任务[{tasksButton != null}] 图鉴[{encyclopediaButton != null}]\n" +
                          $"时间: {System.DateTime.Now}";
        
        Debug.Log(debugInfo);
        UpdateStatus("调试信息已输出到控制台");
    }
    
    void OnDestroy()
    {
        // 清理单例引用
        if (_instance == this)
        {
            _instance = null;
        }
        
        Debug.Log("[MainMenuManager] 主菜单管理器已销毁");
    }
}
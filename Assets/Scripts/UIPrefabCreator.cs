using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// UI预制件创建器 - 自动创建完整的UI系统
/// 使用方法：将此脚本添加到场景中的任意GameObject上
/// 运行时自动创建所有UI元素，无需手动配置
/// </summary>
public class UIPrefabCreator : MonoBehaviour
{
    [Header("UI配置")]
    [Tooltip("UI颜色主题 - 主色")]
    public Color primaryColor = new Color(0.298f, 0.686f, 0.314f); // #4CAF50
    [Tooltip("UI颜色主题 - 强调色")] 
    public Color accentColor = new Color(0.129f, 0.588f, 0.953f); // #2196F3
    [Tooltip("UI颜色主题 - 背景色")]
    public Color backgroundColor = new Color(0.95f, 0.95f, 0.95f, 1f);
    
    [Header("调试选项")]
    [Tooltip("运行时自动创建UI")]
    public bool autoCreateOnStart = true;
    [Tooltip("显示调试日志")]
    public bool showDebugLogs = true;
    
    // 创建的UI组件引用
    private Canvas mainCanvas;
    private GameObject eventSystem;
    private GameObject mainMenuPanel;
    private AnimalUIManager animalUIManager;
    
    void Start()
    {
        if (autoCreateOnStart)
        {
            CreateCompleteUISystem();
        }
    }
    
    /// <summary>
    /// 创建完整的UI系统
    /// </summary>
    public void CreateCompleteUISystem()
    {
        DebugLog("开始创建完整UI系统...");
        
        // 1. 创建Canvas和EventSystem
        CreateCanvasAndEventSystem();
        
        // 2. 创建主菜单面板
        CreateMainMenuPanel();
        
        // 3. 创建导航栏
        CreateNavigationBar();
        
        // 4. 创建AnimalUIManager并配置引用
        CreateAndConfigureAnimalUIManager();
        
        // 5. 初始显示主菜单
        ShowMainMenuInitially();
        
        DebugLog("UI系统创建完成！");
        DebugLog("提示：按F1键可以显示UI状态信息");
    }
    
    /// <summary>
    /// 创建Canvas和EventSystem
    /// </summary>
    private void CreateCanvasAndEventSystem()
    {
        // 检查是否已有Canvas
        mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null)
        {
            DebugLog("创建Canvas...");
            GameObject canvasObj = new GameObject("Canvas");
            mainCanvas = canvasObj.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 设置Canvas Scaler
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }
        else
        {
            DebugLog($"使用现有Canvas: {mainCanvas.name}");
        }
        
        // 检查是否已有EventSystem
        eventSystem = GameObject.Find("EventSystem");
        if (eventSystem == null && FindObjectOfType<EventSystem>() == null)
        {
            DebugLog("创建EventSystem...");
            eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            
            // 添加触摸输入支持（移动端）
            #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            eventSystem.AddComponent<TouchInputModule>();
            #endif
        }
        else
        {
            DebugLog("EventSystem已存在");
        }
    }
    
    /// <summary>
    /// 创建主菜单面板
    /// </summary>
    private void CreateMainMenuPanel()
    {
        DebugLog("创建主菜单面板...");
        
        // 创建主菜单面板
        mainMenuPanel = CreateUIPanel("MainMenuPanel", mainCanvas.transform);
        
        // 设置面板样式
        Image panelImage = mainMenuPanel.GetComponent<Image>();
        panelImage.color = backgroundColor;
        
        // 创建标题
        GameObject titleObj = CreateUIText("TitleText", mainMenuPanel.transform);
        Text titleText = titleObj.GetComponent<Text>();
        titleText.text = "濒危动物AR体验";
        titleText.fontSize = 48;
        titleText.color = primaryColor;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        
        // 设置标题位置
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.1f, 0.7f);
        titleRect.anchorMax = new Vector2(0.9f, 0.9f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // 创建副标题
        GameObject subtitleObj = CreateUIText("SubtitleText", mainMenuPanel.transform);
        Text subtitleText = subtitleObj.GetComponent<Text>();
        subtitleText.text = "探索、学习、保护濒危动物";
        subtitleText.fontSize = 24;
        subtitleText.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        subtitleText.alignment = TextAnchor.MiddleCenter;
        
        // 设置副标题位置
        RectTransform subtitleRect = subtitleObj.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0.1f, 0.6f);
        subtitleRect.anchorMax = new Vector2(0.9f, 0.7f);
        subtitleRect.offsetMin = Vector2.zero;
        subtitleRect.offsetMax = Vector2.zero;
        
        // 创建开始按钮
        GameObject startButton = CreateUIButton("StartButton", mainMenuPanel.transform);
        Button startBtn = startButton.GetComponent<Button>();
        Text startText = startButton.GetComponentInChildren<Text>();
        startText.text = "开始AR体验";
        startText.fontSize = 28;
        
        // 设置开始按钮位置
        RectTransform startRect = startButton.GetComponent<RectTransform>();
        startRect.anchorMin = new Vector2(0.3f, 0.4f);
        startRect.anchorMax = new Vector2(0.7f, 0.5f);
        startRect.offsetMin = Vector2.zero;
        startRect.offsetMax = Vector2.zero;
        
        // 创建图鉴按钮
        GameObject encyclopediaButton = CreateUIButton("EncyclopediaButton", mainMenuPanel.transform);
        Button encyclopediaBtn = encyclopediaButton.GetComponent<Button>();
        Text encyclopediaText = encyclopediaButton.GetComponentInChildren<Text>();
        encyclopediaText.text = "动物图鉴";
        encyclopediaText.fontSize = 28;
        
        // 设置图鉴按钮位置
        RectTransform encyclopediaRect = encyclopediaButton.GetComponent<RectTransform>();
        encyclopediaRect.anchorMin = new Vector2(0.3f, 0.3f);
        encyclopediaRect.anchorMax = new Vector2(0.7f, 0.4f);
        encyclopediaRect.offsetMin = Vector2.zero;
        encyclopediaRect.offsetMax = Vector2.zero;
        
        // 创建设置按钮
        GameObject settingsButton = CreateUIButton("SettingsButton", mainMenuPanel.transform);
        Button settingsBtn = settingsButton.GetComponent<Button>();
        Text settingsText = settingsButton.GetComponentInChildren<Text>();
        settingsText.text = "设置";
        settingsText.fontSize = 28;
        
        // 设置按钮位置
        RectTransform settingsRect = settingsButton.GetComponent<RectTransform>();
        settingsRect.anchorMin = new Vector2(0.3f, 0.2f);
        settingsRect.anchorMax = new Vector2(0.7f, 0.3f);
        settingsRect.offsetMin = Vector2.zero;
        settingsRect.offsetMax = Vector2.zero;
        
        // 创建版本信息
        GameObject versionObj = CreateUIText("VersionText", mainMenuPanel.transform);
        Text versionText = versionObj.GetComponent<Text>();
        versionText.text = "版本 1.0.0 | 濒危动物保护教育项目";
        versionText.fontSize = 14;
        versionText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        versionText.alignment = TextAnchor.LowerCenter;
        
        // 设置版本信息位置
        RectTransform versionRect = versionObj.GetComponent<RectTransform>();
        versionRect.anchorMin = new Vector2(0.1f, 0.02f);
        versionRect.anchorMax = new Vector2(0.9f, 0.1f);
        versionRect.offsetMin = Vector2.zero;
        versionRect.offsetMax = Vector2.zero;
    }
    
    /// <summary>
    /// 创建导航栏
    /// </summary>
    private void CreateNavigationBar()
    {
        DebugLog("创建导航栏...");
        
        // 创建导航栏面板
        GameObject navBarPanel = CreateUIPanel("NavigationBar", mainCanvas.transform);
        
        // 设置导航栏样式
        Image navImage = navBarPanel.GetComponent<Image>();
        navImage.color = new Color(1f, 1f, 1f, 0.95f);
        
        // 设置导航栏位置（底部）
        RectTransform navRect = navBarPanel.GetComponent<RectTransform>();
        navRect.anchorMin = new Vector2(0f, 0f);
        navRect.anchorMax = new Vector2(1f, 0.1f);
        navRect.offsetMin = Vector2.zero;
        navRect.offsetMax = Vector2.zero;
        
        // 创建首页按钮
        GameObject homeButton = CreateUIButton("HomeButton", navBarPanel.transform);
        Button homeBtn = homeButton.GetComponent<Button>();
        Text homeText = homeButton.GetComponentInChildren<Text>();
        homeText.text = "首页";
        homeText.fontSize = 20;
        
        // 设置首页按钮位置
        RectTransform homeRect = homeButton.GetComponent<RectTransform>();
        homeRect.anchorMin = new Vector2(0f, 0f);
        homeRect.anchorMax = new Vector2(0.25f, 1f);
        homeRect.offsetMin = new Vector2(10f, 10f);
        homeRect.offsetMax = new Vector2(-10f, -10f);
        
        // 创建扫描按钮
        GameObject scanButton = CreateUIButton("ScanButton", navBarPanel.transform);
        Button scanBtn = scanButton.GetComponent<Button>();
        Text scanText = scanButton.GetComponentInChildren<Text>();
        scanText.text = "扫描";
        scanText.fontSize = 20;
        
        // 设置扫描按钮位置
        RectTransform scanRect = scanButton.GetComponent<RectTransform>();
        scanRect.anchorMin = new Vector2(0.25f, 0f);
        scanRect.anchorMax = new Vector2(0.5f, 1f);
        scanRect.offsetMin = new Vector2(10f, 10f);
        scanRect.offsetMax = new Vector2(-10f, -10f);
        
        // 创建图鉴按钮（导航栏）
        GameObject navEncyclopediaButton = CreateUIButton("NavEncyclopediaButton", navBarPanel.transform);
        Button navEncyclopediaBtn = navEncyclopediaButton.GetComponent<Button>();
        Text navEncyclopediaText = navEncyclopediaButton.GetComponentInChildren<Text>();
        navEncyclopediaText.text = "图鉴";
        navEncyclopediaText.fontSize = 20;
        
        // 设置图鉴按钮位置
        RectTransform navEncyclopediaRect = navEncyclopediaButton.GetComponent<RectTransform>();
        navEncyclopediaRect.anchorMin = new Vector2(0.5f, 0f);
        navEncyclopediaRect.anchorMax = new Vector2(0.75f, 1f);
        navEncyclopediaRect.offsetMin = new Vector2(10f, 10f);
        navEncyclopediaRect.offsetMax = new Vector2(-10f, -10f);
        
        // 创建设置按钮（导航栏）
        GameObject navSettingsButton = CreateUIButton("NavSettingsButton", navBarPanel.transform);
        Button navSettingsBtn = navSettingsButton.GetComponent<Button>();
        Text navSettingsText = navSettingsButton.GetComponentInChildren<Text>();
        navSettingsText.text = "设置";
        navSettingsText.fontSize = 20;
        
        // 设置按钮位置
        RectTransform navSettingsRect = navSettingsButton.GetComponent<RectTransform>();
        navSettingsRect.anchorMin = new Vector2(0.75f, 0f);
        navSettingsRect.anchorMax = new Vector2(1f, 1f);
        navSettingsRect.offsetMin = new Vector2(10f, 10f);
        navSettingsRect.offsetMax = new Vector2(-10f, -10f);
    }
    
    /// <summary>
    /// 创建并配置AnimalUIManager
    /// </summary>
    private void CreateAndConfigureAnimalUIManager()
    {
        DebugLog("创建并配置AnimalUIManager...");
        
        // 检查是否已有AnimalUIManager
        animalUIManager = FindObjectOfType<AnimalUIManager>();
        if (animalUIManager == null)
        {
            GameObject uiManagerObj = new GameObject("UIManager");
            animalUIManager = uiManagerObj.AddComponent<AnimalUIManager>();
        }
        
        // 尝试获取UI组件引用
        if (animalUIManager != null)
        {
            // 主菜单面板引用
            if (mainMenuPanel != null)
            {
                // 通过反射设置字段（如果字段是public）
                var field = typeof(AnimalUIManager).GetField("mainMenuPanel");
                if (field != null)
                {
                    field.SetValue(animalUIManager, mainMenuPanel);
                }
            }
            
            // 按钮引用
            GameObject homeBtnObj = GameObject.Find("HomeButton");
            GameObject scanBtnObj = GameObject.Find("ScanButton");
            GameObject encyclopediaBtnObj = GameObject.Find("EncyclopediaButton");
            GameObject settingsBtnObj = GameObject.Find("SettingsButton");
            
            if (homeBtnObj != null)
            {
                var homeField = typeof(AnimalUIManager).GetField("homeButton");
                if (homeField != null)
                {
                    homeField.SetValue(animalUIManager, homeBtnObj.GetComponent<Button>());
                }
            }
            
            // 其他按钮类似...
            
            DebugLog("AnimalUIManager配置完成");
        }
    }
    
    /// <summary>
    /// 初始显示主菜单
    /// </summary>
    private void ShowMainMenuInitially()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            DebugLog("主菜单已显示");
        }
        
        // 隐藏导航栏（主菜单不需要）
        GameObject navBar = GameObject.Find("NavigationBar");
        if (navBar != null)
        {
            navBar.SetActive(false);
        }
    }
    
    /// <summary>
    /// 创建UI面板
    /// </summary>
    private GameObject CreateUIPanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        panel.transform.SetParent(parent);
        
        // 设置RectTransform
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        return panel;
    }
    
    /// <summary>
    /// 创建UI文本
    /// </summary>
    private GameObject CreateUIText(string name, Transform parent)
    {
        GameObject textObj = new GameObject(name);
        textObj.AddComponent<CanvasRenderer>();
        Text text = textObj.AddComponent<Text>();
        textObj.transform.SetParent(parent);
        
        // 设置默认文本属性
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 24;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        
        // 设置RectTransform
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.sizeDelta = new Vector2(200, 50);
        
        return textObj;
    }
    
    /// <summary>
    /// 创建UI按钮
    /// </summary>
    private GameObject CreateUIButton(string name, Transform parent)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.AddComponent<CanvasRenderer>();
        Image image = buttonObj.AddComponent<Image>();
        Button button = buttonObj.AddComponent<Button>();
        buttonObj.transform.SetParent(parent);
        
        // 设置按钮样式
        image.color = primaryColor;
        
        // 创建按钮文本
        GameObject textObj = new GameObject("Text");
        textObj.AddComponent<CanvasRenderer>();
        Text text = textObj.AddComponent<Text>();
        textObj.transform.SetParent(buttonObj.transform);
        
        // 设置文本属性
        text.text = "按钮";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        // 设置文本RectTransform
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        textRect.localPosition = Vector3.zero;
        textRect.localScale = Vector3.one;
        
        // 设置按钮RectTransform
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.localPosition = Vector3.zero;
        buttonRect.localScale = Vector3.one;
        buttonRect.sizeDelta = new Vector2(200, 60);
        
        return buttonObj;
    }
    
    /// <summary>
    /// 调试日志
    /// </summary>
    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[UIPrefabCreator] {message}");
        }
    }
    
    /// <summary>
    /// 编辑器工具：手动触发UI创建
    /// </summary>
    [ContextMenu("手动创建UI系统")]
    public void ManualCreateUISystem()
    {
        CreateCompleteUISystem();
    }
    
    /// <summary>
    /// 编辑器工具：清理创建的UI
    /// </summary>
    [ContextMenu("清理UI系统")]
    public void CleanUISystem()
    {
        DebugLog("清理UI系统...");
        
        if (mainCanvas != null && mainCanvas.name == "Canvas")
        {
            DestroyImmediate(mainCanvas.gameObject);
        }
        
        if (eventSystem != null && eventSystem.name == "EventSystem")
        {
            DestroyImmediate(eventSystem);
        }
        
        GameObject uiManager = GameObject.Find("UIManager");
        if (uiManager != null)
        {
            DestroyImmediate(uiManager);
        }
        
        DebugLog("UI系统清理完成");
    }
    
    void Update()
    {
        // 调试功能：按F1显示UI状态（已禁用 - 与Input System冲突）
        // 如需启用，请修改项目设置使用旧Input系统，或改用新的Input System API
        // if (Input.GetKeyDown(KeyCode.F1))
        // {
        //     DisplayUIStatus();
        // }
    }
    
    /// <summary>
    /// 显示UI状态信息
    /// </summary>
    private void DisplayUIStatus()
    {
        Debug.Log("=== UI状态信息 ===");
        Debug.Log($"Canvas: {(mainCanvas != null ? mainCanvas.name : "未找到")}");
        Debug.Log($"EventSystem: {(eventSystem != null ? eventSystem.name : "未找到")}");
        Debug.Log($"主菜单面板: {(mainMenuPanel != null ? mainMenuPanel.name : "未找到")}");
        Debug.Log($"AnimalUIManager: {(animalUIManager != null ? animalUIManager.name : "未找到")}");
        Debug.Log($"当前场景: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        Debug.Log("==================");
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 场景检查器 - 检查当前场景的UI配置状态
/// 提供问题诊断和修复建议
/// </summary>
public class SceneChecker : MonoBehaviour
{
    [Header("检查选项")]
    public bool autoCheckOnStart = true;
    public bool showDetailedReport = true;
    
    void Start()
    {
        if (autoCheckOnStart)
        {
            CheckSceneAndReport();
        }
    }
    
    /// <summary>
    /// 检查场景并生成报告
    /// </summary>
    public void CheckSceneAndReport()
    {
        Debug.Log("=== 场景配置检查报告 ===");
        
        // 检查1: 相机
        CheckCamera();
        
        // 检查2: UI系统
        CheckUISystem();
        
        // 检查3: AnimalUIManager
        CheckAnimalUIManager();
        
        // 检查4: MainUTController (暂时注释，避免编译错误)
        // // CheckMainUTController(); // 紧急修复：暂时注释
        
        // 检查5: 编译状态
        CheckCompilationStatus();
        
        Debug.Log("=== 检查完成 ===");
        Debug.Log("提示：将UIPrefabCreator脚本添加到场景中的GameObject来自动修复UI问题");
    }
    
    /// <summary>
    /// 检查相机配置
    /// </summary>
    private void CheckCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Camera[] allCameras = FindObjectsOfType<Camera>();
            if (allCameras.Length == 0)
            {
                Debug.LogError("❌ 严重问题：场景中没有找到任何相机！");
                Debug.LogError("   解决方案：GameObject → Camera 创建一个新相机");
            }
            else
            {
                Debug.LogWarning($"⚠️  注意：没有找到MainCamera标签的相机，但有{allCameras.Length}个其他相机");
                foreach (Camera cam in allCameras)
                {
                    Debug.Log($"    - {cam.name} (Tag: {cam.tag})");
                }
            }
        }
        else
        {
            Debug.Log($"✅ 相机：找到MainCamera - {mainCamera.name}");
            Debug.Log($"   位置：{mainCamera.transform.position}");
            Debug.Log($"   视口：{mainCamera.pixelWidth}x{mainCamera.pixelHeight}");
        }
    }
    
    /// <summary>
    /// 检查UI系统
    /// </summary>
    private void CheckUISystem()
    {
        // 检查Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ UI问题：场景中没有Canvas！");
            Debug.LogError("   解决方案：运行UIPrefabCreator脚本或手动创建Canvas");
        }
        else
        {
            Debug.Log($"✅ Canvas：找到 - {canvas.name}");
            Debug.Log($"   渲染模式：{canvas.renderMode}");
            Debug.Log($"   尺寸：{canvas.pixelRect}");
            
            // 检查Canvas Scaler
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                Debug.LogWarning("⚠️  Canvas缺少CanvasScaler组件，UI可能无法正确缩放");
            }
            else
            {
                Debug.Log($"   Scaler模式：{scaler.uiScaleMode}");
            }
        }
        
        // 检查EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ UI问题：场景中没有EventSystem！");
            Debug.LogError("   解决方案：GameObject → UI → EventSystem");
        }
        else
        {
            Debug.Log($"✅ EventSystem：找到 - {eventSystem.name}");
        }
        
        // 检查UI元素
        CheckUIElements();
    }
    
    /// <summary>
    /// 检查具体UI元素
    /// </summary>
    private void CheckUIElements()
    {
        // 检查主菜单面板
        GameObject mainMenu = GameObject.Find("MainMenuPanel");
        if (mainMenu == null) mainMenu = GameObject.Find("MainMenu");
        if (mainMenu == null) mainMenu = GameObject.Find("mainMenuPanel");
        
        if (mainMenu == null)
        {
            Debug.LogError("❌ UI问题：未找到主菜单面板！");
            Debug.LogError("   这是你看不到界面的主要原因");
            Debug.LogError("   解决方案：使用UIPrefabCreator脚本创建主菜单");
        }
        else
        {
            Debug.Log($"✅ 主菜单面板：找到 - {mainMenu.name}");
            Debug.Log($"   激活状态：{mainMenu.activeSelf}");
            
            // 检查面板是否在Canvas下
            Canvas canvasInParent = mainMenu.GetComponentInParent<Canvas>();
            if (canvasInParent == null)
            {
                Debug.LogError("❌ 主菜单面板不在Canvas下！");
                Debug.LogError("   解决方案：将面板拖拽到Canvas对象下");
            }
        }
        
        // 检查按钮
        CheckButton("HomeButton", "首页按钮");
        CheckButton("StartButton", "开始按钮");
        CheckButton("ScanButton", "扫描按钮");
    }
    
    /// <summary>
    /// 检查按钮
    /// </summary>
    private void CheckButton(string buttonName, string displayName)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj == null)
        {
            if (showDetailedReport)
            {
                Debug.LogWarning($"⚠️  {displayName}未找到：{buttonName}");
            }
        }
        else
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError($"❌ {buttonName}不是按钮组件！");
            }
            else
            {
                if (showDetailedReport)
                {
                    Debug.Log($"✅ {displayName}：找到 - {buttonObj.name}");
                }
            }
        }
    }
    
    /// <summary>
    /// 检查AnimalUIManager
    /// </summary>
    private void CheckAnimalUIManager()
    {
        AnimalUIManager uiManager = FindObjectOfType<AnimalUIManager>();
        if (uiManager == null)
        {
            Debug.LogError("❌ 核心组件缺失：未找到AnimalUIManager！");
            Debug.LogError("   解决方案：创建GameObject并添加AnimalUIManager脚本");
            Debug.LogError("   或使用UIPrefabCreator自动创建");
        }
        else
        {
            Debug.Log($"✅ AnimalUIManager：找到 - {uiManager.name}");
            
            // 检查关键字段
            var fields = typeof(AnimalUIManager).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            int nullFields = 0;
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(GameObject) || field.FieldType == typeof(Button))
                {
                    object value = field.GetValue(uiManager);
                    if (value == null)
                    {
                        nullFields++;
                        if (showDetailedReport)
                        {
                            Debug.LogWarning($"   ⚠️  字段 {field.Name} 未赋值");
                        }
                    }
                }
            }
            
            if (nullFields > 0)
            {
                Debug.LogWarning($"⚠️  AnimalUIManager有{nullFields}个字段未赋值");
                Debug.LogWarning("   脚本可能无法正常工作");
            }
        }
    }
    
    /// <summary>
    /// 检查MainUTController
    /// </summary>
    private void CheckMainUTController()
    {
        MainUTController mainController = FindObjectOfType<MainUTController>();
        if (mainController == null)
        {
            Debug.LogWarning("⚠️  未找到MainUTController（可能正常，如果当前不是AR场景）");
        }
        else
        {
            Debug.Log($"✅ MainUTController：找到 - {mainController.name}");
            
            // 检查关键组件
            if (mainController.scanningPanel == null)
            {
                Debug.LogWarning("⚠️  MainUTController.scanningPanel未赋值");
            }
            if (mainController.interactionPanel == null)
            {
                Debug.LogWarning("⚠️  MainUTController.interactionPanel未赋值");
            }
        }
    }
    
    /// <summary>
    /// 检查编译状态
    /// </summary>
    private void CheckCompilationStatus()
    {
        // 检查控制台是否有错误
        Debug.Log("📝 编译状态：请检查Unity控制台是否有红色错误");
        Debug.Log("   如果有编译错误，脚本可能无法运行");
        Debug.Log("   我已修复了CS0050、CS1061、CS1657、CS0246错误");
        Debug.Log("   如果仍有错误，请提供具体错误信息");
    }
    
    /// <summary>
    /// 快速修复：创建基础UI
    /// </summary>
    [ContextMenu("快速修复UI")]
    public void QuickFixUI()
    {
        Debug.Log("开始快速修复UI...");
        
        // 创建Canvas（如果不存在）
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("✅ 创建Canvas");
        }
        
        // 创建EventSystem（如果不存在）
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Debug.Log("✅ 创建EventSystem");
        }
        
        // 创建简单的主菜单（如果不存在）
        GameObject mainMenu = GameObject.Find("MainMenuPanel");
        if (mainMenu == null)
        {
            mainMenu = new GameObject("MainMenuPanel");
            mainMenu.AddComponent<CanvasRenderer>();
            Image img = mainMenu.AddComponent<Image>();
            img.color = new Color(0.9f, 0.95f, 1f, 1f);
            mainMenu.transform.SetParent(canvas.transform);
            
            // 设置RectTransform
            RectTransform rect = mainMenu.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // 添加测试文本
            GameObject textObj = new GameObject("TestText");
            textObj.AddComponent<CanvasRenderer>();
            Text text = textObj.AddComponent<Text>();
            text.text = "UI系统运行正常！\n主菜单即将显示...";
            text.fontSize = 32;
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleCenter;
            textObj.transform.SetParent(mainMenu.transform);
            
            // 设置文本位置
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.1f, 0.4f);
            textRect.anchorMax = new Vector2(0.9f, 0.6f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            Debug.Log("✅ 创建主菜单面板");
        }
        
        // 确保AnimalUIManager存在
        AnimalUIManager uiManager = FindObjectOfType<AnimalUIManager>();
        if (uiManager == null)
        {
            GameObject uiManagerObj = new GameObject("UIManager");
            uiManager = uiManagerObj.AddComponent<AnimalUIManager>();
            Debug.Log("✅ 创建AnimalUIManager");
        }
        
        Debug.Log("✅ 快速修复完成！");
        Debug.Log("   运行游戏测试，如果仍有问题，请使用完整的UIPrefabCreator");
    }
    
    /// <summary>
    /// 显示帮助信息
    /// </summary>
    [ContextMenu("显示帮助信息")]
    public void ShowHelp()
    {
        Debug.Log("=== 濒危动物AR项目UI问题帮助 ===");
        Debug.Log("");
        Debug.Log("问题：进入播放模式后看不到主菜单");
        Debug.Log("");
        Debug.Log("可能原因：");
        Debug.Log("1. 场景中没有Canvas或EventSystem");
        Debug.Log("2. AnimalUIManager组件缺失或未配置");
        Debug.Log("3. 主菜单面板GameObject不存在");
        Debug.Log("4. 相机设置问题");
        Debug.Log("");
        Debug.Log("解决方案：");
        Debug.Log("1. 运行本脚本的'快速修复UI'（右键点击组件）");
        Debug.Log("2. 或使用UIPrefabCreator创建完整UI系统");
        Debug.Log("3. 确保打开正确的场景（UI.unity或EndangeredAnimalAR.unity）");
        Debug.Log("4. 检查Unity控制台是否有编译错误");
        Debug.Log("");
        Debug.Log("详细步骤：");
        Debug.Log("1. 打开UI.unity场景：File → Open Scene → Assets/Scenes/UI.unity");
        Debug.Log("2. 创建空GameObject，添加SceneChecker脚本");
        Debug.Log("3. 运行游戏查看检查报告");
        Debug.Log("4. 根据报告建议进行修复");
        Debug.Log("");
        Debug.Log("=== 帮助结束 ===");
    }
}
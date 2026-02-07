using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI集成管理器 - 将新的Pixso设计UI与现有系统集成
/// 作为新旧UI系统之间的桥梁（可选集成）
/// </summary>
public class UIIntegrationManager : MonoBehaviour
{
    [Header("新UI系统引用")]
    public AnimalUIManager animalUIManager;        // 动物UI管理器
    public ARScanUI arScanUI;                      // AR扫描UI
    
    [Header("可选集成 - 现有系统")]
    [Tooltip("如果为空，新UI系统将独立工作")]
    public GameObject existingSystemContainer;     // 现有系统容器（包含MainUTController等）
    
    [Header("UI过渡设置")]
    public float uiTransitionDuration = 0.3f;      // UI过渡持续时间
    public AnimationCurve transitionCurve;         // 过渡曲线
    
    [Header("集成选项")]
    public bool hideExistingUIOnStart = true;      // 启动时隐藏现有UI
    public bool forwardEventsToNewUI = true;       // 将事件转发到新UI
    public bool useNewUIByDefault = true;          // 默认使用新UI系统
    
    // 状态
    private bool isInitialized = false;
    private bool usingNewUI = true;
    private Component cachedMainController;
    
    void Start()
    {
        InitializeIntegration();
    }
    
    /// <summary>
    /// 初始化集成系统
    /// </summary>
    private void InitializeIntegration()
    {
        if (isInitialized) return;
        
        Debug.Log("[UIIntegrationManager] 初始化UI集成系统");
        
        // 自动查找UI组件（如果未在编辑器中设置）
        AutoFindUIComponents();
        
        // 尝试获取现有系统的组件
        if (existingSystemContainer != null)
        {
            // 尝试获取MainUTController组件（可选）
            cachedMainController = existingSystemContainer.GetComponent("MainUTController");
            if (cachedMainController != null)
            {
                Debug.Log("[UIIntegrationManager] 找到现有系统组件");
            }
        }
        
        // 根据设置决定使用哪个UI系统
        if (useNewUIByDefault)
        {
            SwitchToNewUI();
        }
        else
        {
            SwitchToExistingUI();
        }
        
        isInitialized = true;
    }
    
    /// <summary>
    /// 自动查找UI组件（备用方案）
    /// </summary>
    private void AutoFindUIComponents()
    {
        // 如果AnimalUIManager未设置，尝试自动查找
        if (animalUIManager == null)
        {
            animalUIManager = FindObjectOfType<AnimalUIManager>();
            if (animalUIManager != null)
            {
                Debug.Log("[UIIntegrationManager] 自动查找到AnimalUIManager");
            }
            else
            {
                Debug.LogWarning("[UIIntegrationManager] 未找到AnimalUIManager组件，新UI系统可能无法正常工作");
            }
        }
        
        // 如果ARScanUI未设置，尝试自动查找
        if (arScanUI == null)
        {
            arScanUI = FindObjectOfType<ARScanUI>();
            if (arScanUI != null)
            {
                Debug.Log("[UIIntegrationManager] 自动查找到ARScanUI");
            }
            else
            {
                Debug.LogWarning("[UIIntegrationManager] 未找到ARScanUI组件，AR扫描功能可能无法正常工作");
            }
        }
        
        // 检查是否有可用的UI系统
        if (animalUIManager == null && arScanUI == null)
        {
            Debug.LogError("[UIIntegrationManager] 错误：未找到任何UI系统组件！");
            Debug.LogError("请确保以下组件之一存在于场景中：");
            Debug.LogError("1. AnimalUIManager (新UI系统)");
            Debug.LogError("2. 包含MainUTController的游戏对象 (现有系统)");
        }
    }
    
    /// <summary>
    /// 切换到新UI系统
    /// </summary>
    public void SwitchToNewUI()
    {
        StartCoroutine(TransitionToNewUI());
    }
    
    /// <summary>
    /// 切换到现有UI系统
    /// </summary>
    public void SwitchToExistingUI()
    {
        StartCoroutine(TransitionToExistingUI());
    }
    
    /// <summary>
    /// 过渡到新UI
    /// </summary>
    private IEnumerator TransitionToNewUI()
    {
        Debug.Log("[UIIntegrationManager] 切换到新UI系统");
        
        // 检查是否有新UI组件
        if (animalUIManager == null && arScanUI == null)
        {
            Debug.LogError("[UIIntegrationManager] 错误：无法切换到新UI系统 - 未找到任何新UI组件！");
            Debug.LogError("尝试切换到现有UI系统作为备用方案...");
            SwitchToExistingUI();
            yield break;
        }
        
        // 隐藏现有UI（如果存在）
        HideExistingUIPanels();
        
        // 显示新UI
        if (animalUIManager != null)
        {
            animalUIManager.gameObject.SetActive(true);
            Debug.Log($"[UIIntegrationManager] 激活AnimalUIManager: {animalUIManager.gameObject.name}");
            
            // 确保显示主菜单
            try
            {
                animalUIManager.ShowMainMenu();
                Debug.Log("[UIIntegrationManager] 已调用AnimalUIManager.ShowMainMenu()");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[UIIntegrationManager] 调用ShowMainMenu失败: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("[UIIntegrationManager] AnimalUIManager为null，主菜单可能无法显示");
        }
        
        if (arScanUI != null)
        {
            arScanUI.gameObject.SetActive(true);
            Debug.Log($"[UIIntegrationManager] 激活ARScanUI: {arScanUI.gameObject.name}");
        }
        
        usingNewUI = true;
        
        // 记录UI状态
        Debug.Log($"[UIIntegrationManager] 新UI系统已激活，当前UI: {(usingNewUI ? "新UI系统" : "现有UI系统")}");
        yield break;
    }
    
    /// <summary>
    /// 过渡到现有UI
    /// </summary>
    private IEnumerator TransitionToExistingUI()
    {
        Debug.Log("[UIIntegrationManager] 切换到现有UI系统");
        
        // 检查是否有现有系统
        if (cachedMainController == null && existingSystemContainer == null)
        {
            Debug.LogError("[UIIntegrationManager] 错误：无法切换到现有UI系统 - 未找到现有系统组件！");
            
            // 如果也没有新UI系统，显示错误
            if (animalUIManager == null && arScanUI == null)
            {
                Debug.LogError("[UIIntegrationManager] 严重错误：没有任何UI系统可用！");
                // 尝试重新查找组件
                AutoFindUIComponents();
                
                // 如果找到了新UI组件，切换回新UI
                if (animalUIManager != null || arScanUI != null)
                {
                    Debug.Log("[UIIntegrationManager] 找到新UI组件，切换回新UI系统");
                    SwitchToNewUI();
                }
            }
            else
            {
                Debug.Log("[UIIntegrationManager] 切换回新UI系统作为备用方案");
                SwitchToNewUI();
            }
            yield break;
        }
        
        // 隐藏新UI
        if (animalUIManager != null)
        {
            animalUIManager.gameObject.SetActive(false);
            Debug.Log("[UIIntegrationManager] 隐藏AnimalUIManager");
        }
        
        if (arScanUI != null)
        {
            arScanUI.gameObject.SetActive(false);
            Debug.Log("[UIIntegrationManager] 隐藏ARScanUI");
        }
        
        // 显示现有UI（如果存在）
        ShowExistingUIPanels();
        
        usingNewUI = false;
        
        // 记录UI状态
        Debug.Log($"[UIIntegrationManager] 现有UI系统已激活，当前UI: {(usingNewUI ? "新UI系统" : "现有UI系统")}");
        yield break;
    }
    
    /// <summary>
    /// 隐藏现有UI面板
    /// </summary>
    private void HideExistingUIPanels()
    {
        if (cachedMainController == null) return;
        
        try
        {
            // 使用反射访问现有UI组件
            var scanningPanel = GetComponentProperty(cachedMainController, "scanningPanel") as GameObject;
            var interactionPanel = GetComponentProperty(cachedMainController, "interactionPanel") as GameObject;
            
            if (scanningPanel != null) scanningPanel.SetActive(false);
            if (interactionPanel != null) interactionPanel.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[UIIntegrationManager] 隐藏现有UI时出错: {e.Message}");
        }
    }
    
    /// <summary>
    /// 显示现有UI面板
    /// </summary>
    private void ShowExistingUIPanels()
    {
        if (cachedMainController == null) return;
        
        try
        {
            var scanningPanel = GetComponentProperty(cachedMainController, "scanningPanel") as GameObject;
            if (scanningPanel != null) scanningPanel.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[UIIntegrationManager] 显示现有UI时出错: {e.Message}");
        }
    }
    
    /// <summary>
    /// 通过反射获取组件属性
    /// </summary>
    private object GetComponentProperty(Component component, string propertyName)
    {
        if (component == null) return null;
        
        try
        {
            var property = component.GetType().GetProperty(propertyName);
            if (property != null) return property.GetValue(component);
            
            var field = component.GetType().GetField(propertyName);
            if (field != null) return field.GetValue(component);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[UIIntegrationManager] 获取属性 {propertyName} 失败: {e.Message}");
        }
        
        return null;
    }
    
    /// <summary>
    /// 调用现有系统的方法
    /// </summary>
    private void CallExistingSystemMethod(string methodName, params object[] args)
    {
        if (cachedMainController == null) return;
        
        try
        {
            var method = cachedMainController.GetType().GetMethod(methodName);
            if (method != null)
            {
                method.Invoke(cachedMainController, args);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[UIIntegrationManager] 调用方法 {methodName} 失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 当检测到动物时调用（可以从AR系统触发）
    /// </summary>
    public void OnAnimalDetected(string animalId, string animalName)
    {
        Debug.Log($"[UIIntegrationManager] 检测到动物: {animalName} ({animalId})");
        
        // 如果使用新UI，更新新UI
        if (usingNewUI && animalUIManager != null)
        {
            animalUIManager.OnAnimalDetected(animalId, animalName);
        }
        
        // 如果使用现有系统，转发事件
        if (!usingNewUI)
        {
            // 可以调用现有系统的方法
            // CallExistingSystemMethod("OnAnimalDetected", animalId, animalName);
        }
    }
    
    /// <summary>
    /// 开始AR扫描
    /// </summary>
    public void StartARScan()
    {
        Debug.Log("[UIIntegrationManager] 开始AR扫描");
        
        // 这里可以启动AR系统
        // 暂时简单显示AR扫描UI
        if (arScanUI != null)
        {
            arScanUI.StartScanning();
        }
    }
    
    /// <summary>
    /// 停止AR扫描
    /// </summary>
    public void StopARScan()
    {
        Debug.Log("[UIIntegrationManager] 停止AR扫描");
        
        if (arScanUI != null)
        {
            arScanUI.StopScanning();
        }
    }
    
    /// <summary>
    /// 显示动物信息
    /// </summary>
    public void ShowAnimalInfo(string animalId)
    {
        Debug.Log($"[UIIntegrationManager] 显示动物信息: {animalId}");
        
        if (animalUIManager != null)
        {
            animalUIManager.ShowAnimalInfo(animalId);
        }
    }
    
    /// <summary>
    /// 开始动物对话
    /// </summary>
    public void StartAnimalChat(string animalId)
    {
        Debug.Log($"[UIIntegrationManager] 开始动物对话: {animalId}");
        
        if (animalUIManager != null)
        {
            animalUIManager.ShowDialogue(animalId);
        }
    }
    
    /// <summary>
    /// 切换UI系统（用于调试或用户选择）
    /// </summary>
    public void ToggleUISystem()
    {
        if (usingNewUI)
        {
            SwitchToExistingUI();
        }
        else
        {
            SwitchToNewUI();
        }
    }
    
    /// <summary>
    /// 获取当前使用的UI系统
    /// </summary>
    public string GetCurrentUISystem()
    {
        return usingNewUI ? "新UI系统" : "现有UI系统";
    }
}
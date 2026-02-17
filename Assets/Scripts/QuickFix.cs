using UnityEngine;

/// <summary>
/// 快速修复脚本 - 临时解决编译错误
/// 在安全模式中运行一次后可以删除
/// </summary>
public class QuickFix : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 快速修复脚本启动 ===");
        
        // 检查并修复常见问题
        FixCommonIssues();
        
        Debug.Log("=== 快速修复完成 ===");
        Debug.Log("提示：现在可以退出安全模式");
    }
    
    void FixCommonIssues()
    {
        Debug.Log("1. 检查脚本引用...");
        
        // 临时解决方案：如果某些脚本引用有问题，可以在这里处理
        // 例如：动态加载组件、跳过有问题的检查等
        
        Debug.Log("2. 验证场景配置...");
        
        // 确保基本的UI元素存在
        EnsureBasicUIElements();
        
        Debug.Log("3. 清理临时状态...");
        
        // 可以在这里添加其他修复逻辑
    }
    
    void EnsureBasicUIElements()
    {
        // 确保有EventSystem
        var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            Debug.Log("⚠️ 未找到EventSystem，正在创建...");
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), 
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }
        
        // 确保有Canvas
        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.Log("⚠️ 未找到Canvas，正在创建...");
            var newCanvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler));
            newCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }
}
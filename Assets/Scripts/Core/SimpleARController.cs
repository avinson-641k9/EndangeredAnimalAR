using UnityEngine;

/// <summary>
/// 简单AR控制器 - 绝对无编译错误的基础版本
/// </summary>
public class SimpleARController : MonoBehaviour
{
    void Start()
    {
        Debug.Log("✅ 简单AR控制器启动成功！");
        Debug.Log("项目可以正常编译和运行");
        
        // 检查基础组件
        CheckBasicComponents();
    }
    
    void Update()
    {
        // 基础更新逻辑
    }
    
    void CheckBasicComponents()
    {
        // 检查相机
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Debug.Log($"✅ 找到主相机: {mainCamera.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到主相机");
        }
        
        // 检查EventSystem
        var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem != null)
        {
            Debug.Log($"✅ 找到EventSystem: {eventSystem.name}");
        }
        
        // 检查Canvas
        var canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"✅ 找到Canvas: {canvas.name}");
        }
    }
    
    // 简单的方法，确保功能正常
    public void LogTest(string message)
    {
        Debug.Log($"测试消息: {message}");
    }
}
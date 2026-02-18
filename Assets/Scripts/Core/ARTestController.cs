using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// AR 测试控制器 - 验证 AR Foundation 包是否正确安装
/// </summary>
public class ARTestController : MonoBehaviour
{
    private ARSession arSession;
    private ARCameraManager arCameraManager;
    
    void Start()
    {
        Debug.Log("🔧 AR 测试控制器启动");
        
        // 检查 AR 组件
        CheckARComponents();
        
        // 测试 AR Foundation 功能
        TestARFoundation();
    }
    
    void CheckARComponents()
    {
        // 检查 AR Session
        arSession = FindObjectOfType<ARSession>();
        if (arSession != null)
        {
            Debug.Log($"✅ 找到 AR Session: {arSession.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到 AR Session - 需要在场景中添加 AR Session 组件");
        }
        
        // 检查 AR Camera Manager
        arCameraManager = FindObjectOfType<ARCameraManager>();
        if (arCameraManager != null)
        {
            Debug.Log($"✅ 找到 AR Camera Manager: {arCameraManager.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到 AR Camera Manager");
        }
        
        // 检查主相机
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Debug.Log($"✅ 找到主相机: {mainCamera.name}");
            
            // 检查相机是否有 AR 相关组件
            // 注意：在 AR Foundation 5.0.7 中，ARCamera 可能已更名为 ARCameraManager
            // 或者使用 ARSessionOrigin/ARSession 来检查
            var arSession = FindObjectOfType<ARSession>();
            if (arSession != null)
            {
                Debug.Log("✅ 找到 AR Session 组件");
            }
            else
            {
                Debug.Log("⚠️ 未找到 AR Session 组件，但相机存在");
            }
        }
    }
    
    void TestARFoundation()
    {
        Debug.Log("🧪 测试 AR Foundation 功能...");
        
        // 检查 AR Foundation 命名空间是否可用
        bool arFoundationAvailable = true;
        
        try
        {
            // 测试 AR Foundation 类型
            System.Type arSessionType = typeof(ARSession);
            System.Type arCameraType = typeof(ARCameraManager);
            
            Debug.Log($"✅ AR Foundation 类型加载成功:");
            Debug.Log($"   - ARSession: {arSessionType.FullName}");
            Debug.Log($"   - ARCameraManager: {arCameraType.FullName}");
        }
        catch (System.Exception e)
        {
            arFoundationAvailable = false;
            Debug.LogError($"❌ AR Foundation 类型加载失败: {e.Message}");
        }
        
        if (arFoundationAvailable)
        {
            Debug.Log("🎉 AR Foundation 包安装成功！");
            Debug.Log("✅ 项目应该可以正常编译和运行 AR 功能");
        }
        else
        {
            Debug.LogError("❌ AR Foundation 包可能未正确安装");
            Debug.Log("请检查 Package Manager 中的包导入状态");
        }
    }
    
    void Update()
    {
        // 简单的帧率显示
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"📊 帧率: {1.0f / Time.deltaTime:F1} FPS");
        }
    }
    
    // 公共方法供 UI 调用
    public void RunARTest()
    {
        Debug.Log("🚀 手动运行 AR 测试...");
        CheckARComponents();
        TestARFoundation();
    }
    
    public void CheckCompilationStatus()
    {
        Debug.Log("🔍 检查编译状态...");
        Debug.Log($"脚本位置: {this.GetType().FullName}");
        Debug.Log("如果看到此消息，说明脚本编译成功！");
    }
}
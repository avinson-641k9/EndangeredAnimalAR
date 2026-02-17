using UnityEngine;

namespace EndangeredAnimalAR.Test
{
    /// <summary>
    /// 项目完整性测试脚本
    /// </summary>
    public class ProjectTest : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("=== 项目完整性测试开始 ===");
            
            // 测试1: 检查核心脚本是否存在
            TestScriptReferences();
            
            // 测试2: 检查场景文件
            TestSceneFiles();
            
            // 测试3: 检查配置
            TestConfigurations();
            
            Debug.Log("=== 项目完整性测试完成 ===");
        }
        
        void TestScriptReferences()
        {
            Debug.Log("测试1: 检查核心脚本...");
            
            // AR 相关脚本
            if (System.Type.GetType("EndangeredAnimalAR.AR.ARAnimalController") != null)
                Debug.Log("✅ ARAnimalController 脚本存在");
            else
                Debug.LogWarning("⚠️ ARAnimalController 脚本可能有问题");
            
            // UI 相关脚本
            if (System.Type.GetType("MainMenuManager") != null)
                Debug.Log("✅ MainMenuManager 脚本存在");
            else
                Debug.LogWarning("⚠️ MainMenuManager 脚本可能有问题");
            
            // LLM 相关脚本
            if (System.Type.GetType("EndangeredAnimalAR.LLM.LLMClient") != null)
                Debug.Log("✅ LLMClient 脚本存在");
            else
                Debug.LogWarning("⚠️ LLMClient 脚本可能有问题");
            
            // 配置脚本
            if (System.Type.GetType("ConfigLoader") != null)
                Debug.Log("✅ ConfigLoader 脚本存在");
            else
                Debug.LogWarning("⚠️ ConfigLoader 脚本可能有问题");
        }
        
        void TestSceneFiles()
        {
            Debug.Log("测试2: 检查场景文件...");
            
            // 检查场景文件是否存在
            string[] expectedScenes = {
                "Assets/Scenes/EnddangeredAnimalAR1.unity",
                "Assets/Scenes/UI.unity",
                "Assets/Scenes/SampleScene.unity"
            };
            
            foreach (var scene in expectedScenes)
            {
                if (System.IO.File.Exists(scene))
                    Debug.Log($"✅ 场景文件存在: {scene}");
                else
                    Debug.LogWarning($"⚠️ 场景文件缺失: {scene}");
            }
        }
        
        void TestConfigurations()
        {
            Debug.Log("测试3: 检查配置...");
            
            // 检查包配置
            string manifestPath = "Packages/manifest.json";
            if (System.IO.File.Exists(manifestPath))
            {
                Debug.Log("✅ manifest.json 存在");
                // 可以进一步检查关键包
            }
            else
            {
                Debug.LogError("❌ manifest.json 缺失");
            }
            
            // 检查 LLM 服务器
            string llmServerPath = "LLM_Server/app.py";
            if (System.IO.File.Exists(llmServerPath))
                Debug.Log("✅ LLM 服务器代码存在");
            else
                Debug.LogWarning("⚠️ LLM 服务器代码缺失");
        }
    }
}
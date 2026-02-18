using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
public class CheckPackageCompatibility : EditorWindow
{
    [MenuItem("Tools/检查包兼容性")]
    public static void ShowWindow()
    {
        GetWindow<CheckPackageCompatibility>("包兼容性检查");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Unity 包兼容性检查工具", EditorStyles.boldLabel);
        GUILayout.Space(20);
        
        if (GUILayout.Button("1. 检查当前包配置", GUILayout.Height(40)))
        {
            CheckCurrentPackages();
        }
        
        if (GUILayout.Button("2. 检查版本兼容性", GUILayout.Height(40)))
        {
            CheckVersionCompatibility();
        }
        
        if (GUILayout.Button("3. 生成推荐配置", GUILayout.Height(40)))
        {
            GenerateRecommendedConfig();
        }
        
        if (GUILayout.Button("4. 清理示例文件夹", GUILayout.Height(40)))
        {
            CleanupSampleFolders();
        }
        
        GUILayout.Space(20);
        EditorGUILayout.HelpBox("Unity 版本: 2022.3.62f3c1\n建议使用兼容的包版本", MessageType.Info);
    }
    
    static void CheckCurrentPackages()
    {
        string manifestPath = "Packages/manifest.json";
        if (File.Exists(manifestPath))
        {
            string content = File.ReadAllText(manifestPath);
            EditorUtility.DisplayDialog("当前包配置", 
                $"已分析 manifest.json\n包含 {CountLines(content)} 行配置", 
                "确定");
                
            // 检查关键包
            CheckKeyPackages(content);
        }
    }
    
    static void CheckVersionCompatibility()
    {
        Dictionary<string, string> recommendedVersions = new Dictionary<string, string>()
        {
            // Unity 2022.3 LTS 推荐版本
            { "com.unity.xr.arfoundation", "5.1.0" },
            { "com.unity.xr.arcore", "5.1.0" },
            { "com.unity.xr.arkit", "5.1.0" },
            { "com.unity.xr.interaction.toolkit", "3.1.2" },
            { "com.unity.xr.management", "4.4.0" },
            { "com.unity.xr.core-utils", "2.2.3" },
            { "com.unity.textmeshpro", "3.0.6" },
            { "com.unity.timeline", "1.7.5" }
        };
        
        string manifestPath = "Packages/manifest.json";
        if (File.Exists(manifestPath))
        {
            string content = File.ReadAllText(manifestPath);
            string report = "包版本兼容性检查:\n\n";
            
            foreach (var kvp in recommendedVersions)
            {
                if (content.Contains($"\"{kvp.Key}\":"))
                {
                    // 提取当前版本
                    int startIndex = content.IndexOf($"\"{kvp.Key}\":") + kvp.Key.Length + 3;
                    int endIndex = content.IndexOf("\"", startIndex + 1);
                    string currentVersion = content.Substring(startIndex, endIndex - startIndex);
                    
                    if (currentVersion == kvp.Value)
                    {
                        report += $"✅ {kvp.Key}: {currentVersion} (推荐)\n";
                    }
                    else
                    {
                        report += $"⚠️ {kvp.Key}: {currentVersion} (推荐: {kvp.Value})\n";
                    }
                }
                else
                {
                    report += $"❌ {kvp.Key}: 未找到\n";
                }
            }
            
            EditorUtility.DisplayDialog("版本兼容性报告", report, "确定");
        }
    }
    
    static void GenerateRecommendedConfig()
    {
        // Unity 2022.3 LTS 的推荐配置
        string recommendedManifest = @"{
  ""dependencies"": {
    // 核心包
    ""com.unity.collab-proxy"": ""2.0.5"",
    ""com.unity.ide.rider"": ""3.0.24"",
    ""com.unity.ide.visualstudio"": ""2.0.18"",
    ""com.unity.ide.vscode"": ""1.2.5"",
    ""com.unity.test-framework"": ""1.1.33"",
    ""com.unity.textmeshpro"": ""3.0.6"",
    ""com.unity.timeline"": ""1.7.5"",
    ""com.unity.ugui"": ""1.0.0"",
    
    // AR Foundation 5.x (Unity 2022.3 兼容)
    ""com.unity.xr.arfoundation"": ""5.1.0"",
    ""com.unity.xr.arcore"": ""5.1.0"",
    ""com.unity.xr.arkit"": ""5.1.0"",
    ""com.unity.xr.arkit-face-tracking"": ""5.1.0"",
    
    // XR Interaction Toolkit 3.x
    ""com.unity.xr.interaction.toolkit"": ""3.1.2"",
    ""com.unity.xr.management"": ""4.4.0"",
    ""com.unity.xr.core-utils"": ""2.2.3"",
    
    // Unity 核心模块
    ""com.unity.modules.ai"": ""1.0.0"",
    ""com.unity.modules.androidjni"": ""1.0.0"",
    ""com.unity.modules.animation"": ""1.0.0"",
    ""com.unity.modules.assetbundle"": ""1.0.0"",
    ""com.unity.modules.audio"": ""1.0.0"",
    ""com.unity.modules.cloth"": ""1.0.0"",
    ""com.unity.modules.director"": ""1.0.0"",
    ""com.unity.modules.imageconversion"": ""1.0.0"",
    ""com.unity.modules.imgui"": ""1.0.0"",
    ""com.unity.modules.jsonserialize"": ""1.0.0"",
    ""com.unity.modules.particlesystem"": ""1.0.0"",
    ""com.unity.modules.physics"": ""1.0.0"",
    ""com.unity.modules.physics2d"": ""1.0.0"",
    ""com.unity.modules.screencapture"": ""1.0.0"",
    ""com.unity.modules.terrain"": ""1.0.0"",
    ""com.unity.modules.terrainphysics"": ""1.0.0"",
    ""com.unity.modules.tilemap"": ""1.0.0"",
    ""com.unity.modules.ui"": ""1.0.0"",
    ""com.unity.modules.uielements"": ""1.0.0"",
    ""com.unity.modules.umbra"": ""1.0.0"",
    ""com.unity.modules.unityanalytics"": ""1.0.0"",
    ""com.unity.modules.unitywebrequest"": ""1.0.0"",
    ""com.unity.modules.unitywebrequestassetbundle"": ""1.0.0"",
    ""com.unity.modules.unitywebrequestaudio"": ""1.0.0"",
    ""com.unity.modules.unitywebrequesttexture"": ""1.0.0"",
    ""com.unity.modules.unitywebrequestwww"": ""1.0.0"",
    ""com.unity.modules.vehicles"": ""1.0.0"",
    ""com.unity.modules.video"": ""1.0.0"",
    ""com.unity.modules.vr"": ""1.0.0"",
    ""com.unity.modules.wind"": ""1.0.0"",
    ""com.unity.modules.xr"": ""1.0.0""
  }
}";
        
        string backupPath = "Packages/manifest.json.backup";
        if (File.Exists("Packages/manifest.json") && !File.Exists(backupPath))
        {
            File.Copy("Packages/manifest.json", backupPath, true);
            EditorUtility.DisplayDialog("备份创建", $"已创建备份: {backupPath}", "确定");
        }
        
        File.WriteAllText("Packages/manifest.json", recommendedManifest);
        EditorUtility.DisplayDialog("配置已更新", "已应用推荐的包配置\n请重新导入包", "确定");
        
        AssetDatabase.Refresh();
    }
    
    static void CleanupSampleFolders()
    {
        // 检查并建议清理示例文件夹
        string[] sampleFolders = {
            "Assets/Samples/XR Interaction Toolkit/3.1.2",
            "Assets/Samples/XR Interaction Toolkit/2.5.2"
        };
        
        string report = "示例文件夹检查:\n\n";
        foreach (string folder in sampleFolders)
        {
            if (Directory.Exists(folder))
            {
                report += $"📁 {folder} - 存在\n";
                report += $"   大小: {GetDirectorySize(folder) / 1024 / 1024} MB\n";
                
                // 检查是否包含脚本
                string[] csFiles = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);
                report += $"   脚本文件: {csFiles.Length} 个\n";
                
                if (csFiles.Length > 0)
                {
                    report += "   ⚠️ 包含脚本，删除可能导致编译错误\n";
                }
            }
            else
            {
                report += $"✅ {folder} - 不存在\n";
            }
        }
        
        report += "\n建议:\n";
        report += "1. 如果不需要示例，可以删除整个 Samples 文件夹\n";
        report += "2. 删除前备份重要脚本\n";
        report += "3. 删除后重新编译项目\n";
        
        if (EditorUtility.DisplayDialog("示例文件夹检查", report, "查看详情", "取消"))
        {
            // 在项目中高亮显示 Samples 文件夹
            Object samplesFolder = AssetDatabase.LoadAssetAtPath<Object>("Assets/Samples");
            if (samplesFolder != null)
            {
                Selection.activeObject = samplesFolder;
                EditorGUIUtility.PingObject(samplesFolder);
            }
        }
    }
    
    static int CountLines(string text)
    {
        return text.Split('\n').Length;
    }
    
    static void CheckKeyPackages(string manifestContent)
    {
        List<string> keyPackages = new List<string>()
        {
            "com.unity.xr.arfoundation",
            "com.unity.xr.interaction.toolkit", 
            "com.unity.textmeshpro",
            "com.unity.timeline"
        };
        
        foreach (string package in keyPackages)
        {
            if (!manifestContent.Contains($"\"{package}\":"))
            {
                Debug.LogWarning($"⚠️ 缺少关键包: {package}");
            }
        }
    }
    
    static long GetDirectorySize(string path)
    {
        long size = 0;
        if (Directory.Exists(path))
        {
            foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                FileInfo info = new FileInfo(file);
                size += info.Length;
            }
        }
        return size;
    }
}
#endif
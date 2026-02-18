using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class FixUnityPackages : EditorWindow
{
    [MenuItem("Tools/修复 Unity 包依赖")]
    public static void ShowWindow()
    {
        GetWindow<FixUnityPackages>("修复包依赖");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Unity 项目包依赖修复工具", EditorStyles.boldLabel);
        GUILayout.Space(20);
        
        if (GUILayout.Button("1. 分析当前包配置", GUILayout.Height(40)))
        {
            AnalyzeCurrentPackages();
        }
        
        if (GUILayout.Button("2. 添加 AR Foundation 包", GUILayout.Height(40)))
        {
            AddARFoundationPackages();
        }
        
        if (GUILayout.Button("3. 添加 XR 交互工具包", GUILayout.Height(40)))
        {
            AddXRInteractionPackages();
        }
        
        if (GUILayout.Button("4. 修复所有包依赖", GUILayout.Height(40)))
        {
            FixAllPackages();
        }
        
        GUILayout.Space(20);
        EditorGUILayout.HelpBox("当前错误: 54个红色, 13个黄色\n建议先执行第4步修复所有包依赖", MessageType.Info);
    }
    
    static void AnalyzeCurrentPackages()
    {
        string manifestPath = "Packages/manifest.json";
        if (File.Exists(manifestPath))
        {
            string content = File.ReadAllText(manifestPath);
            EditorUtility.DisplayDialog("包配置分析", 
                $"已找到 manifest.json\n文件大小: {content.Length} 字符\n包含 {content.Split('\n').Length} 行", 
                "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "未找到 Packages/manifest.json", "确定");
        }
    }
    
    static void AddARFoundationPackages()
    {
        // 这些是 AR Foundation 和相关包的最新稳定版本
        string[] arPackages = {
            "\"com.unity.xr.arfoundation\": \"5.1.0\"",
            "\"com.unity.xr.arcore\": \"5.1.0\"",
            "\"com.unity.xr.arkit\": \"5.1.0\"",
            "\"com.unity.xr.arkit-face-tracking\": \"5.1.0\""
        };
        
        UpdateManifestWithPackages(arPackages, "AR Foundation");
    }
    
    static void AddXRInteractionPackages()
    {
        // XR 交互工具包
        string[] xrPackages = {
            "\"com.unity.xr.interaction.toolkit\": \"2.5.2\"",
            "\"com.unity.xr.management\": \"4.4.0\"",
            "\"com.unity.xr.core-utils\": \"2.2.3\""
        };
        
        UpdateManifestWithPackages(xrPackages, "XR Interaction Toolkit");
    }
    
    static void FixAllPackages()
    {
        // 完整的包配置
        string fullManifest = @"{
  ""dependencies"": {
    ""com.unity.collab-proxy"": ""2.0.5"",
    ""com.unity.ide.rider"": ""3.0.24"",
    ""com.unity.ide.visualstudio"": ""2.0.18"",
    ""com.unity.ide.vscode"": ""1.2.5"",
    ""com.unity.test-framework"": ""1.1.33"",
    ""com.unity.textmeshpro"": ""3.0.6"",
    ""com.unity.timeline"": ""1.7.5"",
    ""com.unity.ugui"": ""1.0.0"",
    
    // AR Foundation 包
    ""com.unity.xr.arfoundation"": ""5.1.0"",
    ""com.unity.xr.arcore"": ""5.1.0"",
    ""com.unity.xr.arkit"": ""5.1.0"",
    ""com.unity.xr.arkit-face-tracking"": ""5.1.0"",
    
    // XR 交互工具包
    ""com.unity.xr.interaction.toolkit"": ""2.5.2"",
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
        
        File.WriteAllText("Packages/manifest.json", fullManifest);
        EditorUtility.DisplayDialog("成功", "已更新完整的包配置\n请重新导入包并重新编译", "确定");
        
        // 触发包重新导入
        AssetDatabase.Refresh();
        EditorApplication.ExecuteMenuItem("Window/Package Manager");
    }
    
    static void UpdateManifestWithPackages(string[] packages, string packageType)
    {
        string manifestPath = "Packages/manifest.json";
        if (!File.Exists(manifestPath))
        {
            EditorUtility.DisplayDialog("错误", "未找到 Packages/manifest.json", "确定");
            return;
        }
        
        string content = File.ReadAllText(manifestPath);
        
        // 简单的添加逻辑（实际项目需要更复杂的 JSON 解析）
        foreach (string package in packages)
        {
            if (!content.Contains(package.Split(':')[0]))
            {
                // 在 dependencies 部分添加包
                int depsIndex = content.IndexOf("\"dependencies\": {");
                if (depsIndex > 0)
                {
                    int insertIndex = content.IndexOf('\n', depsIndex) + 1;
                    content = content.Insert(insertIndex, "    " + package + ",\n");
                }
            }
        }
        
        File.WriteAllText(manifestPath, content);
        EditorUtility.DisplayDialog("成功", $"已添加 {packageType} 包\n请重新导入包", "确定");
        
        AssetDatabase.Refresh();
    }
}
#endif
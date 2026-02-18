using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
public class CleanupProject : EditorWindow
{
    [MenuItem("Tools/清理项目")]
    public static void ShowWindow()
    {
        GetWindow<CleanupProject>("项目清理工具");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Unity 项目清理工具", EditorStyles.boldLabel);
        GUILayout.Space(20);
        
        if (GUILayout.Button("1. 清理临时文件", GUILayout.Height(40)))
        {
            CleanTempFiles();
        }
        
        if (GUILayout.Button("2. 重新生成项目文件", GUILayout.Height(40)))
        {
            RegenerateProjectFiles();
        }
        
        if (GUILayout.Button("3. 检查冗余脚本", GUILayout.Height(40)))
        {
            CheckRedundantScripts();
        }
        
        if (GUILayout.Button("4. 创建干净备份", GUILayout.Height(40)))
        {
            CreateCleanBackup();
        }
        
        GUILayout.Space(20);
        EditorGUILayout.HelpBox("谨慎操作！某些操作可能需要重新导入项目。", MessageType.Warning);
    }
    
    static void CleanTempFiles()
    {
        List<string> tempFolders = new List<string>()
        {
            "Library",
            "Temp",
            "Obj",
            "Build",
            "Builds"
        };
        
        string report = "临时文件夹检查:\n\n";
        bool hasTempFiles = false;
        
        foreach (string folder in tempFolders)
        {
            if (Directory.Exists(folder))
            {
                long size = GetDirectorySize(folder);
                report += $"📁 {folder} - {size / 1024 / 1024} MB\n";
                hasTempFiles = true;
            }
            else
            {
                report += $"✅ {folder} - 不存在\n";
            }
        }
        
        report += "\n注意:\n";
        report += "• Library/ 包含编译缓存，删除后需要重新导入\n";
        report += "• 其他临时文件夹可以安全删除\n";
        
        if (hasTempFiles && EditorUtility.DisplayDialog("临时文件检查", report, "清理 Library", "取消"))
        {
            // 只清理 Library，其他让用户手动决定
            if (Directory.Exists("Library"))
            {
                Directory.Delete("Library", true);
                EditorUtility.DisplayDialog("清理完成", "已删除 Library 文件夹\n请重新打开 Unity 项目", "确定");
            }
        }
    }
    
    static void RegenerateProjectFiles()
    {
        string[] projectFiles = {
            ".csproj",
            ".sln"
        };
        
        string report = "将重新生成项目文件:\n\n";
        foreach (string ext in projectFiles)
        {
            string[] files = Directory.GetFiles(".", "*" + ext, SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                report += $"• {Path.GetFileName(file)}\n";
            }
        }
        
        report += "\n操作:\n";
        report += "1. 删除现有项目文件\n";
        report += "2. 重新生成 Rider/VS 项目文件\n";
        report += "3. 可能需要重启 Unity\n";
        
        if (EditorUtility.DisplayDialog("重新生成项目文件", report, "继续", "取消"))
        {
            // 触发重新生成项目文件
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            EditorUtility.DisplayDialog("操作完成", "已请求重新生成项目文件\n请等待编译完成", "确定");
        }
    }
    
    static void CheckRedundantScripts()
    {
        // 检查可能冗余的脚本
        string[] potentialRedundantPaths = {
            "Assets/Samples",
            "Assets/Plugins/Android",
            "Assets/Plugins/iOS"
        };
        
        string report = "可能冗余的文件夹:\n\n";
        foreach (string path in potentialRedundantPaths)
        {
            if (Directory.Exists(path))
            {
                string[] csFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
                string[] dllFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
                
                report += $"📁 {path}\n";
                report += $"   CS 文件: {csFiles.Length} 个\n";
                report += $"   DLL 文件: {dllFiles.Length} 个\n";
                
                if (csFiles.Length > 0)
                {
                    report += "   ⚠️ 包含脚本，请谨慎处理\n";
                }
            }
        }
        
        report += "\n建议:\n";
        report += "• Samples/ 如果不需示例可以删除\n";
        report += "• Plugins/ 确保只包含当前平台需要的\n";
        report += "• 删除前备份重要文件\n";
        
        EditorUtility.DisplayDialog("冗余脚本检查", report, "确定");
    }
    
    static void CreateCleanBackup()
    {
        string backupName = $"Backup_{System.DateTime.Now:yyyyMMdd_HHmmss}";
        string backupPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), backupName);
        
        string[] includeFolders = {
            "Assets",
            "ProjectSettings",
            "Packages"
        };
        
        string[] excludeFolders = {
            "Library",
            "Temp",
            "Obj",
            "Build",
            "Builds",
            "Logs"
        };
        
        string report = "将创建干净备份:\n\n";
        report += $"名称: {backupName}\n";
        report += $"位置: {backupPath}\n\n";
        
        report += "包含:\n";
        foreach (string folder in includeFolders)
        {
            report += $"• {folder}/\n";
        }
        
        report += "\n排除:\n";
        foreach (string folder in excludeFolders)
        {
            report += $"• {folder}/\n";
        }
        
        if (EditorUtility.DisplayDialog("创建干净备份", report, "创建", "取消"))
        {
            try
            {
                // 创建备份目录
                Directory.CreateDirectory(backupPath);
                
                // 复制包含的文件夹
                foreach (string folder in includeFolders)
                {
                    if (Directory.Exists(folder))
                    {
                        CopyDirectory(folder, Path.Combine(backupPath, folder), excludeFolders);
                    }
                }
                
                EditorUtility.DisplayDialog("备份完成", $"已创建干净备份到:\n{backupPath}", "确定");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("错误", $"备份失败:\n{e.Message}", "确定");
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
                try
                {
                    FileInfo info = new FileInfo(file);
                    size += info.Length;
                }
                catch { }
            }
        }
        return size;
    }
    
    static void CopyDirectory(string sourceDir, string destDir, string[] excludePatterns)
    {
        Directory.CreateDirectory(destDir);
        
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destDir, fileName);
            
            // 检查是否在排除列表中
            bool shouldExclude = false;
            foreach (string exclude in excludePatterns)
            {
                if (file.Contains(exclude))
                {
                    shouldExclude = true;
                    break;
                }
            }
            
            if (!shouldExclude)
            {
                File.Copy(file, destFile, true);
            }
        }
        
        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(dir);
            string destSubDir = Path.Combine(destDir, dirName);
            
            // 检查是否在排除列表中
            bool shouldExclude = false;
            foreach (string exclude in excludePatterns)
            {
                if (dir.Contains(exclude))
                {
                    shouldExclude = true;
                    break;
                }
            }
            
            if (!shouldExclude)
            {
                CopyDirectory(dir, destSubDir, excludePatterns);
            }
        }
    }
}
#endif
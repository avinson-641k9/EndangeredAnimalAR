using System.IO;
using UnityEngine;

/// <summary>
/// API配置加载器
/// 提供简单的方法来加载API配置
/// </summary>
public class ConfigLoader : MonoBehaviour
{
    [System.Serializable]
    public class APIConfig
    {
        public string deepseek_api_key;
        public string deepseek_model;
        public string deepseek_endpoint;
    }

    /// <summary>
    /// 从配置文件加载DeepSeek API配置
    /// </summary>
    public static APIConfig LoadAPIConfig()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, "api_config.json");
        
        if (File.Exists(configPath))
        {
            try
            {
                string json = File.ReadAllText(configPath);
                return JsonUtility.FromJson<APIConfig>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ConfigLoader] 配置文件加载失败: {e.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"[ConfigLoader] 配置文件不存在: {configPath}");
            return null;
        }
    }

    /// <summary>
    /// 检查API配置是否有效
    /// </summary>
    public static bool IsAPIConfigValid()
    {
        var config = LoadAPIConfig();
        return config != null && 
               !string.IsNullOrEmpty(config.deepseek_api_key) &&
               !string.IsNullOrEmpty(config.deepseek_model) &&
               !string.IsNullOrEmpty(config.deepseek_endpoint);
    }
}
using UnityEngine;

public class TestAPI : MonoBehaviour
{
    void Start()
    {
        // 测试API配置加载
        var config = ConfigLoader.LoadAPIConfig();
        
        if (config != null)
        {
            Debug.Log("[TestAPI] API配置加载成功:");
            Debug.Log($"  Model: {config.deepseek_model}");
            Debug.Log($"  Endpoint: {config.deepseek_endpoint}");
            Debug.Log($"  API Key 长度: {config.deepseek_api_key?.Length ?? 0} 字符");
            
            if (!string.IsNullOrEmpty(config.deepseek_api_key))
            {
                // 隐藏大部分API密钥，只显示开头和结尾
                string maskedKey = config.deepseek_api_key.Substring(0, 6) + 
                                  "***" + 
                                  config.deepseek_api_key.Substring(config.deepseek_api_key.Length - 4);
                Debug.Log($"  Masked API Key: {maskedKey}");
            }
        }
        else
        {
            Debug.LogError("[TestAPI] API配置加载失败!");
        }
        
        // 测试配置有效性
        bool isValid = ConfigLoader.IsAPIConfigValid();
        Debug.Log($"[TestAPI] API配置有效性: {isValid}");
    }
}
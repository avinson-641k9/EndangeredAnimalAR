using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// API连接测试脚本
/// 用于验证DeepSeek API连接是否正常
/// </summary>
public class APITest : MonoBehaviour
{
    [Header("API 配置")]
    public string testPrompt = "你好，这是一个API连接测试";
    public string apiKey = "";
    public string apiEndpoint = "https://api.deepseek.com/chat/completions";
    public string modelName = "deepseek-chat";

    [Header("测试结果")]
    public bool isConnected = false;
    public string lastErrorMessage = "";

    [System.Serializable]
    private class TestRequest
    {
        public string model;
        public TestMessage[] messages;
        public float temperature = 0.7f;
        public int max_tokens = 100;
    }

    [System.Serializable]
    private class TestMessage
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    private class TestResponse
    {
        public TestChoice[] choices;
        public string id;
        public string object_type;
        public long created;
        public string model;
    }

    [System.Serializable]
    private class TestChoice
    {
        public int index;
        public TestMessage message;
        public object logprobs;
        public string finish_reason;
    }

    /// <summary>
    /// 开始API连接测试
    /// </summary>
    public void StartTest()
    {
        StartCoroutine(TestAPIConnection());
    }

    private IEnumerator TestAPIConnection()
    {
        // 尝试从配置文件加载API密钥
        if (string.IsNullOrEmpty(apiKey))
        {
            LoadAPIConfig();
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            lastErrorMessage = "API密钥未配置";
            isConnected = false;
            Debug.LogError("[APITest] " + lastErrorMessage);
            yield break;
        }

        var request = new TestRequest
        {
            model = modelName,
            messages = new TestMessage[] {
                new TestMessage { role = "system", content = "你是一个API连接测试助手，只需要回复'API连接正常'即可" },
                new TestMessage { role = "user", content = testPrompt }
            },
            temperature = 0.7f,
            max_tokens = 100
        };

        string jsonBody = JsonUtility.ToJson(request);

        using (UnityWebRequest www = new UnityWebRequest(apiEndpoint, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            Debug.Log("[APITest] 正在发送API测试请求...");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonUtility.FromJson<TestResponse>(www.downloadHandler.text);
                    
                    if (response.choices != null && response.choices.Length > 0)
                    {
                        isConnected = true;
                        lastErrorMessage = "";
                        Debug.Log($"[APITest] API连接成功！返回: {response.choices[0].message.content}");
                    }
                    else
                    {
                        isConnected = false;
                        lastErrorMessage = "API返回格式异常";
                        Debug.LogError("[APITest] " + lastErrorMessage);
                    }
                }
                catch (System.Exception e)
                {
                    isConnected = false;
                    lastErrorMessage = $"解析响应失败: {e.Message}";
                    Debug.LogError("[APITest] " + lastErrorMessage);
                }
            }
            else
            {
                isConnected = false;
                lastErrorMessage = $"API请求失败: {www.error}";
                Debug.LogError($"[APITest] {lastErrorMessage}");
            }
        }
    }

    /// <summary>
    /// 从配置文件加载API配置
    /// </summary>
    private void LoadAPIConfig()
    {
        var config = ConfigLoader.LoadAPIConfig();
        if (config != null)
        {
            apiKey = config.deepseek_api_key;
            modelName = config.deepseek_model;
            apiEndpoint = config.deepseek_endpoint;
        }
    }

}
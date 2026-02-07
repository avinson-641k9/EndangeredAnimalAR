using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 濒危动物对话管理器
/// 使用 DeepSeek API 实现动物角色化对话
/// </summary>
public class AnimalChatManager : MonoBehaviour
{
    [Header("API 配置（留空则从配置文件读取）")]
    [SerializeField] private string apiEndpoint = "";
    [SerializeField] private string apiKey = "";
    [SerializeField] private string modelName = "";

    [Header("对话设置")]
    [SerializeField] private int maxHistoryLength = 10;

    // 动物性格配置
    private static readonly System.Collections.Generic.Dictionary<string, string> AnimalPersonalities = 
        new System.Collections.Generic.Dictionary<string, string>
    {
        { "panda", @"你是一只憨厚可爱的大熊猫，名叫团团。你的性格特点：
- 说话慢悠悠的，喜欢用'呀'、'呢'等语气词
- 最爱吃竹子，经常提到竹子有多好吃
- 有点懒，喜欢躺着晒太阳
- 对人类很友好，喜欢分享关于大熊猫的知识
- 会用简单易懂的方式解释濒危动物保护的重要性
请用第一人称回答，保持角色一致性。回答要简短有趣（50字以内），适合科普教育。" },
        
        { "tiger", @"你是一只威武的东北虎，名叫威威。你的性格特点：
- 说话自信有力，偶尔会'嗷呜'一声
- 作为森林之王，你很有威严但不凶
- 喜欢讲述关于东北虎的故事和习性
- 会分享自己在野外生存的经历
- 对栖息地被破坏感到担忧，希望人类保护森林
请用第一人称回答，保持角色一致性。回答要简短有趣（50字以内），适合科普教育。" },

        { "snow_leopard", @"你是一只机敏的雪豹，名叫雪灵。你的性格特点：
- 说话轻声细语但很机警
- 喜欢高山，会描述雪山的美丽
- 有点神秘，被称为'雪山幽灵'
- 擅长跳跃和攀爬，喜欢分享自己的绝技
- 会讲述雪豹面临的生存挑战
请用第一人称回答，保持角色一致性。回答要简短有趣（50字以内），适合科普教育。" },

        { "yangtze_finless_porpoise", @"你是一只可爱的长江江豚，名叫嘟嘟。你的性格特点：
- 说话活泼可爱，喜欢用'咕噜咕噜'等拟声词
- 喜欢在长江里游泳，会描述水下的世界
- 有着标志性的微笑，被称为'微笑天使'
- 会讲述长江生态保护的重要性
请用第一人称回答，保持角色一致性。回答要简短有趣（50字以内），适合科普教育。" },

        { "default", @"你是一只友好的濒危动物。请用第一人称、简单易懂的方式回答问题，分享关于濒危动物保护的知识。回答要简短（50字以内）。" }
    };

    private System.Collections.Generic.List<ChatMessage> chatHistory = 
        new System.Collections.Generic.List<ChatMessage>();
    private string currentAnimalType = "default";
    private bool isInitialized = false;

    public event Action<string> OnResponseReceived;
    public event Action<string> OnError;

    [Serializable]
    private class ChatMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    private class DeepSeekRequest
    {
        public string model;
        public ChatMessage[] messages;
        public float temperature = 0.7f;
        public int max_tokens = 150;
    }

    [Serializable]
    private class DeepSeekResponse
    {
        public DeepSeekChoice[] choices;
        public string id;
        public string object_type;
        public long created;
        public string model;
    }

    [Serializable]
    private class DeepSeekChoice
    {
        public int index;
        public ChatMessage message;
        public object logprobs;
        public string finish_reason;
    }

    private void Start()
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        if (!string.IsNullOrEmpty(apiKey))
        {
            isInitialized = true;
            return;
        }

        var config = ConfigLoader.LoadAPIConfig();
        if (config != null)
        {
            apiKey = config.deepseek_api_key;
            modelName = config.deepseek_model;
            apiEndpoint = config.deepseek_endpoint;
            
            isInitialized = true;
            Debug.Log("[AnimalChat] 配置加载成功");
        }
        else
        {
            Debug.LogWarning("[AnimalChat] 配置加载失败或为空");
        }
    }

    public void SetAnimalType(string animalType)
    {
        currentAnimalType = animalType.ToLower();
        chatHistory.Clear();
        Debug.Log($"[AnimalChat] 切换到动物类型: {currentAnimalType}");
    }

    public void SendMessage(string userMessage, Action<string> onResponse, Action<string> onError = null)
    {
        if (!isInitialized)
        {
            LoadConfig();
        }

        if (!IsApiKeyConfigured())
        {
            string errorMsg = "API 未配置，请检查 api_config.json";
            Debug.LogError($"[AnimalChat] {errorMsg}");
            onError?.Invoke(errorMsg);
            return;
        }

        StartCoroutine(SendMessageCoroutine(userMessage, onResponse, onError));
    }

    private IEnumerator SendMessageCoroutine(string userMessage, Action<string> onResponse, Action<string> onError)
    {
        string personality = AnimalPersonalities.ContainsKey(currentAnimalType) 
            ? AnimalPersonalities[currentAnimalType] 
            : AnimalPersonalities["default"];

        var messages = new System.Collections.Generic.List<ChatMessage>();
        messages.Add(new ChatMessage { role = "system", content = personality });
        messages.AddRange(chatHistory);
        
        var userMsg = new ChatMessage { role = "user", content = userMessage };
        messages.Add(userMsg);

        var request = new DeepSeekRequest
        {
            model = modelName,
            messages = messages.ToArray(),
            temperature = 0.7f,
            max_tokens = 150
        };

        string jsonBody = JsonUtility.ToJson(request);

        using (UnityWebRequest www = new UnityWebRequest(apiEndpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonUtility.FromJson<DeepSeekResponse>(www.downloadHandler.text);
                    
                    string reply;
                    if (response.choices != null && response.choices.Length > 0)
                    {
                        reply = response.choices[0].message.content;
                    }
                    else
                    {
                        reply = "呜...我没听懂你说什么呢~";
                    }

                    chatHistory.Add(userMsg);
                    chatHistory.Add(new ChatMessage { role = "assistant", content = reply });

                    while (chatHistory.Count > maxHistoryLength * 2)
                    {
                        chatHistory.RemoveAt(0);
                        chatHistory.RemoveAt(0);
                    }

                    onResponse?.Invoke(reply);
                    OnResponseReceived?.Invoke(reply);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[AnimalChat] 解析响应失败: {e.Message}");
                    string errorMsg = "抱歉，我现在有点迷糊，请再说一次吧~";
                    onError?.Invoke(errorMsg);
                    OnError?.Invoke(errorMsg);
                }
            }
            else
            {
                Debug.LogError($"[AnimalChat] API 请求失败: {www.error}");
                string errorMsg = "网络好像有点问题，我听不太清呢~";
                onError?.Invoke(errorMsg);
                OnError?.Invoke(errorMsg);
            }
        }
    }

    public void ClearHistory()
    {
        chatHistory.Clear();
    }

    public string GetWelcomeMessage()
    {
        switch (currentAnimalType)
        {
            case "panda":
                return "嗨呀~我是团团，一只爱吃竹子的大熊猫！你想了解我吗？";
            case "tiger":
                return "嗷呜！我是威威，森林之王东北虎！有什么想问我的吗？";
            case "snow_leopard":
                return "你好呀，我是雪灵，来自雪山的雪豹。很高兴见到你~";
            case "yangtze_finless_porpoise":
                return "咕噜咕噜~我是嘟嘟，长江里的微笑天使江豚！";
            default:
                return "你好！我是一只濒危动物，很高兴认识你！";
        }
    }

    public bool IsApiKeyConfigured()
    {
        return !string.IsNullOrEmpty(apiKey);
    }
}

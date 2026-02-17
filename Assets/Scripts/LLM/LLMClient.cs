using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

namespace EndangeredAnimalAR.LLM
{
    /// <summary>
    /// LLM 客户端 - 与本地 LLM 服务器通信
    /// </summary>
    public class LLMClient : MonoBehaviour
    {
        [Header("服务器设置")]
        [SerializeField] private string serverURL = "http://localhost:5000";
        
        [Header("UI 引用")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text responseText;
        [SerializeField] private Button sendButton;
        [SerializeField] private TMP_Dropdown animalDropdown;
        
        [Header("调试")]
        [SerializeField] private bool debugMode = true;
        
        private string currentAnimal = "大熊猫";
        
        void Start()
        {
            // 初始化 UI 事件
            if (sendButton != null)
                sendButton.onClick.AddListener(OnSendButtonClicked);
            
            if (inputField != null)
                inputField.onEndEdit.AddListener(OnInputFieldSubmit);
            
            if (animalDropdown != null)
            {
                animalDropdown.onValueChanged.AddListener(OnAnimalSelected);
                LoadAnimalList();
            }
            
            // 测试服务器连接
            StartCoroutine(TestServerConnection());
        }
        
        /// <summary>
        /// 测试服务器连接
        /// </summary>
        private IEnumerator TestServerConnection()
        {
            string url = $"{serverURL}/health";
            
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("✅ LLM 服务器连接成功");
                    if (responseText != null)
                        responseText.text = "LLM 服务器已就绪，可以开始对话！";
                }
                else
                {
                    Debug.LogError($"❌ LLM 服务器连接失败: {request.error}");
                    if (responseText != null)
                        responseText.text = $"服务器连接失败: {request.error}\n请确保 LLM 服务器正在运行。";
                }
            }
        }
        
        /// <summary>
        /// 加载动物列表
        /// </summary>
        private void LoadAnimalList()
        {
            StartCoroutine(LoadAnimalsCoroutine());
        }
        
        private IEnumerator LoadAnimalsCoroutine()
        {
            string url = $"{serverURL}/animals";
            
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    // 解析 JSON 响应
                    AnimalListResponse response = JsonUtility.FromJson<AnimalListResponse>(
                        "{\"animals\":" + request.downloadHandler.text + "}"
                    );
                    
                    if (animalDropdown != null && response.animals != null)
                    {
                        animalDropdown.ClearOptions();
                        animalDropdown.AddOptions(new List<string>(response.animals));
                        currentAnimal = response.animals[0];
                    }
                }
                else if (debugMode)
                {
                    Debug.LogWarning("使用默认动物列表");
                    // 使用默认列表
                    animalDropdown.ClearOptions();
                    animalDropdown.AddOptions(new List<string> { "大熊猫", "东北虎", "长江江豚" });
                    currentAnimal = "大熊猫";
                }
            }
        }
        
        /// <summary>
        /// 发送消息到 LLM 服务器
        /// </summary>
        public void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
                
            StartCoroutine(SendMessageCoroutine(message));
        }
        
        private IEnumerator SendMessageCoroutine(string message)
        {
            // 创建请求数据
            ChatRequest requestData = new ChatRequest
            {
                message = message,
                animal = currentAnimal
            };
            
            string jsonData = JsonUtility.ToJson(requestData);
            string url = $"{serverURL}/chat";
            
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                
                // 显示发送状态
                if (responseText != null)
                    responseText.text = "正在思考...";
                
                if (sendButton != null)
                    sendButton.interactable = false;
                
                yield return request.SendWebRequest();
                
                if (sendButton != null)
                    sendButton.interactable = true;
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    // 解析响应
                    ChatResponse response = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                    
                    if (responseText != null)
                        responseText.text = response.response;
                    
                    if (debugMode)
                        Debug.Log($"LLM 响应: {response.response}");
                }
                else
                {
                    string errorMsg = $"请求失败: {request.error}";
                    
                    if (responseText != null)
                        responseText.text = errorMsg;
                    
                    Debug.LogError(errorMsg);
                }
            }
        }
        
        /// <summary>
        /// 获取动物信息
        /// </summary>
        public void GetAnimalInfo(string animalName)
        {
            StartCoroutine(GetAnimalInfoCoroutine(animalName));
        }
        
        private IEnumerator GetAnimalInfoCoroutine(string animalName)
        {
            string url = $"{serverURL}/animal_info/{UnityWebRequest.EscapeURL(animalName)}";
            
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    // 这里可以解析并显示动物信息
                    Debug.Log($"动物信息: {request.downloadHandler.text}");
                }
            }
        }
        
        // UI 事件处理
        private void OnSendButtonClicked()
        {
            if (inputField != null && !string.IsNullOrEmpty(inputField.text))
            {
                SendMessage(inputField.text);
                inputField.text = "";
            }
        }
        
        private void OnInputFieldSubmit(string text)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                OnSendButtonClicked();
            }
        }
        
        private void OnAnimalSelected(int index)
        {
            if (animalDropdown != null)
            {
                currentAnimal = animalDropdown.options[index].text;
                Debug.Log($"当前选择的动物: {currentAnimal}");
                
                // 获取新动物的信息
                GetAnimalInfo(currentAnimal);
            }
        }
        
        // 数据类
        [System.Serializable]
        private class ChatRequest
        {
            public string message;
            public string animal;
        }
        
        [System.Serializable]
        private class ChatResponse
        {
            public string response;
            public string animal;
            public float timestamp;
        }
        
        [System.Serializable]
        private class AnimalListResponse
        {
            public string[] animals;
            public int count;
        }
        
        [System.Serializable]
        private class AnimalInfo
        {
            public string description;
            public string status;
            public string population;
            public string threats;
            public string conservation;
        }
    }
}
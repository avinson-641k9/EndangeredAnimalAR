using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动物预制件管理器 - 统一管理所有动物预制件和配置
/// </summary>
public class AnimalPrefabManager : MonoBehaviour
{
    [Header("动物预制件配置")]
    public List<AnimalPrefabConfig> animalConfigs = new List<AnimalPrefabConfig>();
    
    [Header("默认设置")]
    public float defaultScale = 1.0f;
    public Vector3 defaultSpawnOffset = Vector3.zero;
    
    [Header("外部引用")]
    public AnimalConfigManager configManager;
    public ResourceManager resourceManager;
    
    private Dictionary<string, AnimalPrefabConfig> configMap = new Dictionary<string, AnimalPrefabConfig>();
    
    [System.Serializable]
    public class AnimalPrefabConfig
    {
        [Tooltip("识别图名称（需与Reference Image Library中的名称一致）")]
        public string imageName;
        
        [Tooltip("动物预制件")]
        public GameObject animalPrefab;
        
        [Tooltip("生成位置偏移")]
        public Vector3 spawnOffset = Vector3.zero;
        
        [Tooltip("初始缩放比例")]
        public float scale = 1.0f;
        
        [Tooltip("动物类型标识")]
        public AnimalType animalType = AnimalType.Panda;
        
        [Tooltip("动物性格描述")]
        public string personalityDescription = "";
        
        [Tooltip("初始对话提示")]
        public string welcomeMessage = "";
        
        [Tooltip("动物信息")]
        public AnimalInfo animalInfo;
        
        [Tooltip("动物定义资源（可选，用于自动填充信息）")]
        public AnimalDefinition animalDefinition;
    }
    
    [System.Serializable]
    public class AnimalInfo
    {
        public string displayName = "";
        public string scientificName = "";
        public string habitat = "";
        public string diet = "";
        public string conservationStatus = "";
        public string funFact = "";
        public string protectionTips = "";
    }
    
    public enum AnimalType
    {
        Panda,           // 大熊猫
        Tiger,           // 东北虎
        SnowLeopard,     // 雪豹
        YangtzeFinlessPorpoise, // 长江江豚
        Other            // 其他
    }
    
    void Awake()
    {
        InitializeConfigMap();
        InitializeExternalManagers();
    }
    
    /// <summary>
    /// 初始化外部管理器
    /// </summary>
    private void InitializeExternalManagers()
    {
        if (configManager == null)
        {
            configManager = FindObjectOfType<AnimalConfigManager>();
        }
        
        if (resourceManager == null)
        {
            resourceManager = ResourceManager.Instance;
        }
    }
    
    /// <summary>
    /// 初始化配置映射
    /// </summary>
    private void InitializeConfigMap()
    {
        configMap.Clear();
        
        foreach (var config in animalConfigs)
        {
            if (!string.IsNullOrEmpty(config.imageName))
            {
                // 如果有动物定义，优先使用定义中的信息
                if (config.animalDefinition != null)
                {
                    UpdateConfigFromDefinition(ref config, config.animalDefinition);
                }
                
                configMap[config.imageName.ToLower()] = config;
            }
        }
    }
    
    /// <summary>
    /// 从动物定义更新配置
    /// </summary>
    private void UpdateConfigFromDefinition(ref AnimalPrefabConfig config, AnimalDefinition definition)
    {
        if (definition != null)
        {
            // 更新基本信息
            if (string.IsNullOrEmpty(config.personalityDescription))
                config.personalityDescription = definition.personalityDescription;
            if (string.IsNullOrEmpty(config.welcomeMessage))
                config.welcomeMessage = definition.welcomeMessage;
            
            // 更新动物信息
            if (config.animalInfo == null)
                config.animalInfo = new AnimalInfo();
            
            if (string.IsNullOrEmpty(config.animalInfo.displayName))
                config.animalInfo.displayName = definition.GetDisplayName();
            if (string.IsNullOrEmpty(config.animalInfo.scientificName))
                config.animalInfo.scientificName = definition.scientificName;
            if (string.IsNullOrEmpty(config.animalInfo.habitat))
                config.animalInfo.habitat = definition.habitat;
            if (string.IsNullOrEmpty(config.animalInfo.diet))
                config.animalInfo.diet = definition.diet;
            if (string.IsNullOrEmpty(config.animalInfo.conservationStatus))
                config.animalInfo.conservationStatus = definition.conservationStatus;
            if (string.IsNullOrEmpty(config.animalInfo.funFact))
                config.animalInfo.funFact = definition.funFacts;
            if (string.IsNullOrEmpty(config.animalInfo.protectionTips))
                config.animalInfo.protectionTips = definition.conservationTips;
            
            // 更新缩放和偏移
            if (config.scale == 1.0f && definition.defaultScale != 1.0f)
                config.scale = definition.defaultScale;
            if (config.spawnOffset == Vector3.zero && definition.spawnOffset != Vector3.zero)
                config.spawnOffset = definition.spawnOffset;
        }
    }
    
    /// <summary>
    /// 根据图像名称获取动物配置
    /// </summary>
    public AnimalPrefabConfig GetAnimalConfig(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            return null;
        }
        
        AnimalPrefabConfig config;
        if (configMap.TryGetValue(imageName.ToLower(), out config))
        {
            return config;
        }
        
        // 如果没找到精确匹配，尝试模糊匹配
        foreach (var kvp in configMap)
        {
            if (imageName.ToLower().Contains(kvp.Key) || kvp.Key.Contains(imageName.ToLower()))
            {
                return kvp.Value;
            }
        }
        
        // 如果仍然找不到，尝试从配置管理器获取
        if (configManager != null)
        {
            string animalName = ExtractAnimalNameFromImage(imageName);
            var def = configManager.GetAnimalDefinition(animalName);
            if (def != null)
            {
                // 创建临时配置
                config = new AnimalPrefabConfig
                {
                    imageName = imageName,
                    animalDefinition = def,
                    scale = def.defaultScale,
                    spawnOffset = def.spawnOffset,
                    personalityDescription = def.personalityDescription,
                    welcomeMessage = def.welcomeMessage
                };
                
                UpdateConfigFromDefinition(ref config, def);
                return config;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// 从图像名称中提取动物名称
    /// </summary>
    private string ExtractAnimalNameFromImage(string imageName)
    {
        // 简单的提取逻辑，可以根据实际命名规则调整
        imageName = imageName.ToLower();
        
        if (imageName.Contains("panda") || imageName.Contains("熊猫"))
            return "panda";
        else if (imageName.Contains("tiger") || imageName.Contains("老虎"))
            return "tiger";
        else if (imageName.Contains("leopard") || imageName.Contains("雪豹"))
            return "snow leopard";
        else if (imageName.Contains("porpoise") || imageName.Contains("江豚"))
            return "porpoise";
        else
            return imageName;
    }
    
    /// <summary>
    /// 生成动物实例
    /// </summary>
    public GameObject SpawnAnimal(string imageName, Vector3 position, Quaternion rotation)
    {
        var config = GetAnimalConfig(imageName);
        if (config == null)
        {
            Debug.LogWarning($"[AnimalPrefabManager] 未找到图像 '{imageName}' 对应的动物配置");
            return null;
        }
        
        // 如果配置中没有预制件，尝试从资源管理器加载
        if (config.animalPrefab == null && resourceManager != null)
        {
            string animalName = ExtractAnimalNameFromImage(imageName);
            config.animalPrefab = resourceManager.LoadResource<GameObject>($"Animals/{animalName}");
            
            // 如果还是没有找到，使用默认的占位模型
            if (config.animalPrefab == null)
            {
                config.animalPrefab = CreatePlaceholderModel();
            }
        }
        
        // 计算最终位置和缩放
        Vector3 finalPosition = position + config.spawnOffset;
        Vector3 finalScale = Vector3.one * config.scale;
        
        // 生成动物
        GameObject animalInstance = Instantiate(config.animalPrefab, finalPosition, rotation);
        animalInstance.transform.localScale = finalScale;
        animalInstance.name = $"{config.animalPrefab.name}_{imageName}";
        
        // 添加动物标识组件（如果预制件中没有的话）
        AddAnimalIdentifier(animalInstance, config);
        
        Debug.Log($"[AnimalPrefabManager] 生成动物: {config.animalPrefab.name} (图像: {imageName})");
        
        return animalInstance;
    }
    
    /// <summary>
    /// 创建占位模型
    /// </summary>
    private GameObject CreatePlaceholderModel()
    {
        // 创建一个简单的立方体作为占位模型
        GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        placeholder.name = "PlaceholderAnimal";
        
        // 添加基础材质
        Renderer renderer = placeholder.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.color = Color.gray;
            renderer.material = material;
        }
        
        // 移除碰撞器，因为我们只需要视觉效果
        DestroyImmediate(placeholder.GetComponent<BoxCollider>());
        
        return placeholder;
    }
    
    /// <summary>
    /// 添加动物标识组件
    /// </summary>
    private void AddAnimalIdentifier(GameObject animalObject, AnimalPrefabConfig config)
    {
        var identifier = animalObject.GetComponent<AnimalIdentifier>();
        if (identifier == null)
        {
            identifier = animalObject.AddComponent<AnimalIdentifier>();
        }
        
        identifier.animalType = config.animalType;
        identifier.imageName = config.imageName;
        identifier.personalityDescription = config.personalityDescription;
        identifier.welcomeMessage = config.welcomeMessage;
        identifier.animalInfo = config.animalInfo;
    }
    
    /// <summary>
    /// 获取所有支持的动物类型
    /// </summary>
    public List<AnimalType> GetSupportedAnimalTypes()
    {
        var types = new List<AnimalType>();
        
        foreach (var config in animalConfigs)
        {
            if (!types.Contains(config.animalType))
            {
                types.Add(config.animalType);
            }
        }
        
        return types;
    }
    
    /// <summary>
    /// 获取所有支持的图像名称
    /// </summary>
    public List<string> GetSupportedImageNames()
    {
        var names = new List<string>();
        
        foreach (var config in animalConfigs)
        {
            if (!string.IsNullOrEmpty(config.imageName))
            {
                names.Add(config.imageName);
            }
        }
        
        return names;
    }
    
    /// <summary>
    /// 检查是否支持特定图像
    /// </summary>
    public bool IsImageSupported(string imageName)
    {
        return GetAnimalConfig(imageName) != null;
    }
    
    /// <summary>
    /// 更新动物配置
    /// </summary>
    public void UpdateAnimalConfig(string imageName, AnimalPrefabConfig newConfig)
    {
        // 查找现有配置
        for (int i = 0; i < animalConfigs.Count; i++)
        {
            if (animalConfigs[i].imageName.Equals(imageName, System.StringComparison.OrdinalIgnoreCase))
            {
                animalConfigs[i] = newConfig;
                InitializeConfigMap(); // 重新初始化映射
                return;
            }
        }
        
        // 如果没找到，添加新配置
        animalConfigs.Add(newConfig);
        InitializeConfigMap();
    }
    
    /// <summary>
    /// 获取动物信息
    /// </summary>
    public AnimalInfo GetAnimalInfo(string imageName)
    {
        var config = GetAnimalConfig(imageName);
        return config?.animalInfo;
    }
    
    /// <summary>
    /// 获取动物类型
    /// </summary>
    public AnimalType GetAnimalType(string imageName)
    {
        var config = GetAnimalConfig(imageName);
        return config != null ? config.animalType : AnimalType.Other;
    }
    
    /// <summary>
    /// 获取欢迎消息
    /// </summary>
    public string GetWelcomeMessage(string imageName)
    {
        var config = GetAnimalConfig(imageName);
        return config?.welcomeMessage ?? "你好！我是你的动物朋友。";
    }
    
    /// <summary>
    /// 获取性格描述
    /// </summary>
    public string GetPersonalityDescription(string imageName)
    {
        var config = GetAnimalConfig(imageName);
        return config?.personalityDescription ?? "友好的濒危动物";
    }
}

/// <summary>
/// 动物标识组件 - 附加到生成的动物对象上
/// </summary>
[System.Serializable]
public class AnimalIdentifier : MonoBehaviour
{
    [HideInInspector] public AnimalPrefabManager.AnimalType animalType;
    [HideInInspector] public string imageName;
    [HideInInspector] public string personalityDescription;
    [HideInInspector] public string welcomeMessage;
    [HideInInspector] public AnimalPrefabManager.AnimalInfo animalInfo;
    
    void Start()
    {
        // 确保该组件附加到对象上
        if (GetComponent<AnimalIdentifier>() == null)
        {
            gameObject.AddComponent<AnimalIdentifier>();
        }
    }
    
    /// <summary>
    /// 获取动物显示名称
    /// </summary>
    public string GetDisplayName()
    {
        return animalInfo?.displayName ?? animalType.ToString();
    }
    
    /// <summary>
    /// 获取动物科学名称
    /// </summary>
    public string GetScientificName()
    {
        return animalInfo?.scientificName ?? "";
    }
    
    /// <summary>
    /// 获取动物栖息地信息
    /// </summary>
    public string GetHabitatInfo()
    {
        return animalInfo?.habitat ?? "";
    }
    
    /// <summary>
    /// 获取动物饮食信息
    /// </summary>
    public string GetDietInfo()
    {
        return animalInfo?.diet ?? "";
    }
    
    /// <summary>
    /// 获取保护状态
    /// </summary>
    public string GetConservationStatus()
    {
        return animalInfo?.conservationStatus ?? "";
    }
    
    /// <summary>
    /// 获取趣味事实
    /// </summary>
    public string GetFunFact()
    {
        return animalInfo?.funFact ?? "";
    }
    
    /// <summary>
    /// 获取保护建议
    /// </summary>
    public string GetProtectionTips()
    {
        return animalInfo?.protectionTips ?? "";
    }
}
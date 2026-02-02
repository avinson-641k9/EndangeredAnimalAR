using UnityEngine;

public class TestAnimalSystem : MonoBehaviour
{
    public AnimalPrefabManager prefabManager;
    public AnimalConfigManager configManager;
    public ResourceManager resourceManager;
    
    void Start()
    {
        Debug.Log("[TestAnimalSystem] 开始测试动物系统...");
        
        // 初始化管理器
        if (prefabManager == null)
            prefabManager = FindObjectOfType<AnimalPrefabManager>();
        
        if (configManager == null)
            configManager = FindObjectOfType<AnimalConfigManager>();
        
        if (resourceManager == null)
            resourceManager = ResourceManager.Instance;
        
        // 测试配置管理器
        if (configManager != null)
        {
            Debug.Log($"[TestAnimalSystem] 动物配置数量: {configManager.GetAnimalCount()}");
            var supportedAnimals = configManager.GetSupportedAnimalNames();
            Debug.Log($"[TestAnimalSystem] 支持的动物: {string.Join(", ", supportedAnimals)}");
        }
        else
        {
            Debug.LogWarning("[TestAnimalSystem] 未找到 AnimalConfigManager");
        }
        
        // 测试资源管理器
        if (resourceManager != null)
        {
            Debug.Log("[TestAnimalSystem] 资源管理器已找到，准备预加载资源...");
            resourceManager.PreloadCommonResources();
        }
        else
        {
            Debug.LogWarning("[TestAnimalSystem] 未找到 ResourceManager");
        }
        
        Debug.Log("[TestAnimalSystem] 动物系统测试完成");
    }
}
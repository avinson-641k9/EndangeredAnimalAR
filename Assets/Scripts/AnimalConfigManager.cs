using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动物配置管理器 - 管理所有动物定义和配置
/// </summary>
public class AnimalConfigManager : MonoBehaviour
{
    [Header("动物定义配置")]
    public List<AnimalDefinition> animalDefinitions = new List<AnimalDefinition>();
    
    [Header("默认设置")]
    public float defaultRecognitionDistance = 3.0f;
    public Vector3 defaultSpawnOffset = Vector3.zero;
    public float defaultScale = 1.0f;
    
    private Dictionary<string, AnimalDefinition> animalDict = new Dictionary<string, AnimalDefinition>();
    
    void Awake()
    {
        InitializeAnimalDictionary();
    }
    
    /// <summary>
    /// 初始化动物字典
    /// </summary>
    private void InitializeAnimalDictionary()
    {
        animalDict.Clear();
        
        foreach (var animalDef in animalDefinitions)
        {
            if (animalDef != null)
            {
                // 使用动物名称作为键，转换为小写以便比较
                string key = animalDef.animalName.ToLower();
                if (!animalDict.ContainsKey(key))
                {
                    animalDict[key] = animalDef;
                }
                else
                {
                    Debug.LogWarning($"[AnimalConfigManager] 动物名称重复: {key}");
                }
            }
        }
    }
    
    /// <summary>
    /// 根据名称获取动物定义
    /// </summary>
    public AnimalDefinition GetAnimalDefinition(string animalName)
    {
        if (string.IsNullOrEmpty(animalName))
        {
            return null;
        }
        
        AnimalDefinition def;
        if (animalDict.TryGetValue(animalName.ToLower(), out def))
        {
            return def;
        }
        
        // 如果没找到精确匹配，尝试模糊匹配
        foreach (var kvp in animalDict)
        {
            if (kvp.Key.Contains(animalName.ToLower()) || animalName.ToLower().Contains(kvp.Key))
            {
                return kvp.Value;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// 检查是否支持特定动物
    /// </summary>
    public bool IsAnimalSupported(string animalName)
    {
        return GetAnimalDefinition(animalName) != null;
    }
    
    /// <summary>
    /// 获取所有支持的动物名称
    /// </summary>
    public List<string> GetSupportedAnimalNames()
    {
        var names = new List<string>();
        foreach (var kvp in animalDict)
        {
            names.Add(kvp.Value.GetDisplayName());
        }
        return names;
    }
    
    /// <summary>
    /// 添加动物定义
    /// </summary>
    public void AddAnimalDefinition(AnimalDefinition animalDef)
    {
        if (animalDef != null && !animalDefinitions.Contains(animalDef))
        {
            animalDefinitions.Add(animalDef);
            InitializeAnimalDictionary(); // 重新初始化字典
        }
    }
    
    /// <summary>
    /// 移除动物定义
    /// </summary>
    public void RemoveAnimalDefinition(AnimalDefinition animalDef)
    {
        if (animalDef != null && animalDefinitions.Contains(animalDef))
        {
            animalDefinitions.Remove(animalDef);
            InitializeAnimalDictionary(); // 重新初始化字典
        }
    }
    
    /// <summary>
    /// 获取默认的动物定义
    /// </summary>
    public AnimalDefinition GetDefaultAnimalDefinition()
    {
        if (animalDefinitions.Count > 0)
        {
            return animalDefinitions[0]; // 返回第一个作为默认
        }
        return null;
    }
    
    /// <summary>
    /// 获取动物总数
    /// </summary>
    public int GetAnimalCount()
    {
        return animalDefinitions.Count;
    }
    
    /// <summary>
    /// 根据类型获取动物定义
    /// </summary>
    public List<AnimalDefinition> GetAnimalsByType(string animalType)
    {
        var result = new List<AnimalDefinition>();
        foreach (var animalDef in animalDefinitions)
        {
            if (animalDef != null && animalDef.animalType.ToLower().Contains(animalType.ToLower()))
            {
                result.Add(animalDef);
            }
        }
        return result;
    }
}
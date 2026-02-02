using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// 濒危动物 AR 管理器
/// 负责识别图像标记并生成对应的 3D 动物模型
/// </summary>
public class AnimalARManager : MonoBehaviour
{
    [Header("AR 组件")]
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    [Header("动物预制体配置")]
    [Tooltip("图像名称与动物预制体的映射")]
    [SerializeField] private List<AnimalMapping> animalMappings = new List<AnimalMapping>();

    // 已生成的动物实例缓存
    private Dictionary<string, GameObject> spawnedAnimals = new Dictionary<string, GameObject>();

    [System.Serializable]
    public class AnimalMapping
    {
        public string imageName;        // 识别图的名称（在 Reference Image Library 中设置）
        public GameObject animalPrefab; // 对应的动物预制体
        public Vector3 spawnOffset;     // 生成位置偏移
        public float scale = 1f;        // 缩放比例
    }

    private void OnEnable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    /// <summary>
    /// 当追踪的图像状态改变时调用
    /// </summary>
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 处理新检测到的图像
        foreach (var trackedImage in eventArgs.added)
        {
            HandleTrackedImage(trackedImage);
        }

        // 处理更新的图像（位置变化等）
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImage(trackedImage);
        }

        // 处理丢失的图像
        foreach (var trackedImage in eventArgs.removed)
        {
            RemoveTrackedImage(trackedImage);
        }
    }

    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        Debug.Log($"[AnimalAR] 检测到图像: {imageName}");

        // 查找对应的动物配置
        AnimalMapping mapping = animalMappings.Find(m => m.imageName == imageName);
        if (mapping == null || mapping.animalPrefab == null)
        {
            Debug.LogWarning($"[AnimalAR] 未找到图像 '{imageName}' 对应的动物配置");
            return;
        }

        // 如果还没有生成这个动物
        if (!spawnedAnimals.ContainsKey(imageName))
        {
            // 生成动物模型
            Vector3 position = trackedImage.transform.position + mapping.spawnOffset;
            Quaternion rotation = trackedImage.transform.rotation;
            
            GameObject animal = Instantiate(mapping.animalPrefab, position, rotation);
            animal.transform.localScale = Vector3.one * mapping.scale;
            animal.name = $"Animal_{imageName}";
            
            // 将动物设为图像的子对象，这样会跟随图像移动
            animal.transform.SetParent(trackedImage.transform);
            
            spawnedAnimals[imageName] = animal;
            Debug.Log($"[AnimalAR] 生成动物: {imageName}");

            // TODO: 这里可以触发动物出现动画、音效等
            OnAnimalSpawned(animal, imageName);
        }
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        
        if (spawnedAnimals.TryGetValue(imageName, out GameObject animal))
        {
            // 根据追踪状态显示/隐藏动物
            bool isTracking = trackedImage.trackingState == TrackingState.Tracking;
            animal.SetActive(isTracking);
        }
    }

    private void RemoveTrackedImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        
        if (spawnedAnimals.TryGetValue(imageName, out GameObject animal))
        {
            Destroy(animal);
            spawnedAnimals.Remove(imageName);
            Debug.Log($"[AnimalAR] 移除动物: {imageName}");
        }
    }

    /// <summary>
    /// 动物生成后的回调 - 可以在这里添加对话系统初始化等
    /// </summary>
    protected virtual void OnAnimalSpawned(GameObject animal, string animalType)
    {
        // 子类可以重写这个方法来添加额外功能
        // 比如：初始化对话系统、播放欢迎语音等
    }

    /// <summary>
    /// 获取当前场景中的所有动物
    /// </summary>
    public Dictionary<string, GameObject> GetSpawnedAnimals()
    {
        return new Dictionary<string, GameObject>(spawnedAnimals);
    }
}

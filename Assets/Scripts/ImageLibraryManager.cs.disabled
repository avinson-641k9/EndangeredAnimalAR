using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// AR识别图库管理器 - 管理和配置AR识别图库
/// </summary>
public class ImageLibraryManager : MonoBehaviour
{
    [Header("AR组件引用")]
    public ARTrackedImageManager trackedImageManager;
    
    [Header("识别图库配置")]
    public List<ImageConfiguration> imageConfigurations = new List<ImageConfiguration>();
    
    [Header("调试选项")]
    public bool showDebugLogs = true;
    public bool enableImageTracking = true;
    
    private Dictionary<string, ImageConfiguration> configMap = new Dictionary<string, ImageConfiguration>();
    private Dictionary<string, ARTrackedImage> activeTrackedImages = new Dictionary<string, ARTrackedImage>();
    
    [System.Serializable]
    public class ImageConfiguration
    {
        [Tooltip("识别图名称（必须与Reference Image Library中的名称一致）")]
        public string imageName;
        
        [Tooltip("对应动物类型")]
        public string animalType;
        
        [Tooltip("图像宽度（米）- 用于正确缩放")]
        public float physicalWidth = 0.1f;
        
        [Tooltip("启用状态")]
        public bool isEnabled = true;
        
        [Tooltip("特殊效果配置")]
        public ImageEffectConfig effectConfig;
        
        [Tooltip("生成配置")]
        public SpawnConfig spawnConfig;
    }
    
    [System.Serializable]
    public class ImageEffectConfig
    {
        public bool playAnimationOnDetection = true;
        public float animationDuration = 1.0f;
        public bool playSoundOnDetection = true;
        public AudioClip detectionSound;
        public bool spawnParticlesOnDetection = true;
        public ParticleSystem detectionParticles;
    }
    
    [System.Serializable]
    public class SpawnConfig
    {
        public Vector3 spawnOffset = Vector3.zero;
        public float spawnScale = 1.0f;
        public bool rotateToMatchImage = true;
        public bool parentToImage = true;
    }
    
    void Start()
    {
        InitializeComponents();
        InitializeConfigMap();
        SetupEventHandlers();
    }
    
    /// <summary>
    /// 初始化组件引用
    /// </summary>
    private void InitializeComponents()
    {
        if (trackedImageManager == null)
        {
            trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        }
        
        if (trackedImageManager == null)
        {
            Debug.LogError("[ImageLibraryManager] 未找到 ARTrackedImageManager 组件！");
            return;
        }
        
        // 启用图像跟踪（如果需要）
        if (enableImageTracking)
        {
            trackedImageManager.enabled = true;
        }
    }
    
    /// <summary>
    /// 初始化配置映射
    /// </summary>
    private void InitializeConfigMap()
    {
        configMap.Clear();
        
        foreach (var config in imageConfigurations)
        {
            if (!string.IsNullOrEmpty(config.imageName) && config.isEnabled)
            {
                configMap[config.imageName.ToLower()] = config;
                
                if (showDebugLogs)
                {
                    Debug.Log($"[ImageLibraryManager] 注册识别图: {config.imageName} -> {config.animalType}");
                }
            }
        }
    }
    
    /// <summary>
    /// 设置事件处理器
    /// </summary>
    private void SetupEventHandlers()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }
    
    /// <summary>
    /// 当追踪图像状态改变时调用
    /// </summary>
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 处理新添加的图像
        foreach (var trackedImage in eventArgs.added)
        {
            OnImageAdded(trackedImage);
        }
        
        // 处理更新的图像
        foreach (var trackedImage in eventArgs.updated)
        {
            OnImageUpdated(trackedImage);
        }
        
        // 处理移除的图像
        foreach (var trackedImage in eventArgs.removed)
        {
            OnImageRemoved(trackedImage);
        }
    }
    
    /// <summary>
    /// 当图像被添加时
    /// </summary>
    private void OnImageAdded(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        
        if (showDebugLogs)
        {
            Debug.Log($"[ImageLibraryManager] 检测到图像: {imageName} (物理尺寸: {trackedImage.referenceImage.size})");
        }
        
        // 检查是否在配置中
        if (configMap.ContainsKey(imageName.ToLower()))
        {
            // 存储活动的追踪图像
            activeTrackedImages[imageName.ToLower()] = trackedImage;
            
            // 应用配置
            ApplyImageConfiguration(trackedImage);
            
            // 触发自定义事件
            OnImageDetected(trackedImage);
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"[ImageLibraryManager] 检测到未配置的图像: {imageName}");
            }
        }
    }
    
    /// <summary>
    /// 当图像被更新时
    /// </summary>
    private void OnImageUpdated(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        
        if (activeTrackedImages.ContainsKey(imageName.ToLower()))
        {
            // 更新追踪状态
            activeTrackedImages[imageName.ToLower()] = trackedImage;
            
            // 触发更新事件
            OnImageTracked(trackedImage);
        }
    }
    
    /// <summary>
    /// 当图像被移除时
    /// </summary>
    private void OnImageRemoved(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        
        if (activeTrackedImages.ContainsKey(imageName.ToLower()))
        {
            // 触发移除事件
            OnImageLost(trackedImage);
            
            // 从活动列表中移除
            activeTrackedImages.Remove(imageName.ToLower());
        }
    }
    
    /// <summary>
    /// 应用图像配置
    /// </summary>
    private void ApplyImageConfiguration(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        
        if (configMap.TryGetValue(imageName.ToLower(), out ImageConfiguration config))
        {
            // 设置物理尺寸（如果配置了）
            if (config.physicalWidth > 0)
            {
                // 这里可以调整图像的物理尺寸
                // 注意：AR Foundation通常根据参考图像库中的尺寸自动设置
            }
            
            // 应用特殊效果
            ApplyImageEffects(trackedImage, config.effectConfig);
        }
    }
    
    /// <summary>
    /// 应用图像效果
    /// </summary>
    private void ApplyImageEffects(ARTrackedImage trackedImage, ImageEffectConfig effectConfig)
    {
        if (effectConfig == null) return;
        
        // 播放检测音效
        if (effectConfig.playSoundOnDetection && effectConfig.detectionSound != null)
        {
            AudioSource.PlayClipAtPoint(effectConfig.detectionSound, trackedImage.transform.position);
        }
        
        // 生成粒子效果
        if (effectConfig.spawnParticlesOnDetection && effectConfig.detectionParticles != null)
        {
            var particles = Instantiate(effectConfig.detectionParticles, trackedImage.transform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, effectConfig.detectionParticles.main.duration);
        }
        
        // 播放动画（如果有动画组件）
        if (effectConfig.playAnimationOnDetection)
        {
            StartCoroutine(RunDetectionAnimation(trackedImage, effectConfig.animationDuration));
        }
    }
    
    /// <summary>
    /// 运行检测动画
    /// </summary>
    private IEnumerator RunDetectionAnimation(ARTrackedImage trackedImage, float duration)
    {
        // 这里可以实现视觉反馈动画
        // 例如：淡入效果、脉冲效果等
        
        float startTime = Time.time;
        Vector3 originalScale = trackedImage.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        
        // 简单的放大缩小动画
        while (Time.time - startTime < duration)
        {
            float progress = (Time.time - startTime) / duration;
            float scale = Mathf.Lerp(1.0f, 1.1f, Mathf.Sin(progress * Mathf.PI));
            trackedImage.transform.localScale = originalScale * scale;
            
            yield return null;
        }
        
        trackedImage.transform.localScale = originalScale;
    }
    
    /// <summary>
    /// 图像检测事件
    /// </summary>
    protected virtual void OnImageDetected(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        
        if (showDebugLogs)
        {
            Debug.Log($"[ImageLibraryManager] 图像检测完成: {imageName}");
        }
        
        // 这里可以添加自定义逻辑，比如通知其他系统
        NotifyImageDetection(imageName, trackedImage.transform.position);
    }
    
    /// <summary>
    /// 图像追踪事件
    /// </summary>
    protected virtual void OnImageTracked(ARTrackedImage trackedImage)
    {
        // 当图像位置/旋转更新时调用
    }
    
    /// <summary>
    /// 图像丢失事件
    /// </summary>
    protected virtual void OnImageLost(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        
        if (showDebugLogs)
        {
            Debug.Log($"[ImageLibraryManager] 图像丢失: {imageName}");
        }
        
        // 通知其他系统图像已丢失
        NotifyImageLoss(imageName);
    }
    
    /// <summary>
    /// 通知图像检测
    /// </summary>
    private void NotifyImageDetection(string imageName, Vector3 position)
    {
        // 发送Unity事件或消息给其他系统
        // 例如：通知AnimalARManager生成动物模型
        
        // 使用SendMessage（如果其他对象上有相应方法）
        SendMessage("OnImageDetected", new ImageDetectionData { imageName = imageName, position = position }, SendMessageOptions.DontRequireReceiver);
        
        // 或者使用事件系统
        OnImageDetectedInternal?.Invoke(imageName, position);
    }
    
    /// <summary>
    /// 通知图像丢失
    /// </summary>
    private void NotifyImageLoss(string imageName)
    {
        // 发送Unity事件或消息给其他系统
        SendMessage("OnImageLost", imageName, SendMessageOptions.DontRequireReceiver);
        
        // 或者使用事件系统
        OnImageLostInternal?.Invoke(imageName);
    }
    
    // 内部事件，供其他脚本订阅
    public System.Action<string, Vector3> OnImageDetectedInternal;
    public System.Action<string> OnImageLostInternal;
    
    /// <summary>
    /// 获取图像配置
    /// </summary>
    public ImageConfiguration GetImageConfiguration(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            return null;
        }
        
        ImageConfiguration config;
        if (configMap.TryGetValue(imageName.ToLower(), out config))
        {
            return config;
        }
        
        return null;
    }
    
    /// <summary>
    /// 检查图像是否已配置
    /// </summary>
    public bool IsImageConfigured(string imageName)
    {
        return configMap.ContainsKey(imageName.ToLower());
    }
    
    /// <summary>
    /// 获取所有活动的追踪图像
    /// </summary>
    public List<ARTrackedImage> GetActiveTrackedImages()
    {
        return new List<ARTrackedImage>(activeTrackedImages.Values);
    }
    
    /// <summary>
    /// 获取活动图像数量
    /// </summary>
    public int GetActiveImageCount()
    {
        return activeTrackedImages.Count;
    }
    
    /// <summary>
    /// 检查是否有图像正在被追踪
    /// </summary>
    public bool HasActiveImages()
    {
        return activeTrackedImages.Count > 0;
    }
    
    /// <summary>
    /// 添加新的图像配置
    /// </summary>
    public void AddImageConfiguration(ImageConfiguration config)
    {
        if (!string.IsNullOrEmpty(config.imageName))
        {
            // 检查是否已存在
            for (int i = 0; i < imageConfigurations.Count; i++)
            {
                if (imageConfigurations[i].imageName.ToLower() == config.imageName.ToLower())
                {
                    imageConfigurations[i] = config; // 更新现有配置
                    InitializeConfigMap(); // 重新初始化映射
                    return;
                }
            }
            
            // 添加新配置
            imageConfigurations.Add(config);
            InitializeConfigMap(); // 重新初始化映射
        }
    }
    
    /// <summary>
    /// 移除图像配置
    /// </summary>
    public void RemoveImageConfiguration(string imageName)
    {
        imageConfigurations.RemoveAll(config => config.imageName.ToLower() == imageName.ToLower());
        InitializeConfigMap(); // 重新初始化映射
    }
    
    /// <summary>
    /// 获取支持的所有动物类型
    /// </summary>
    public List<string> GetSupportedAnimalTypes()
    {
        var types = new List<string>();
        
        foreach (var config in imageConfigurations)
        {
            if (!string.IsNullOrEmpty(config.animalType) && !types.Contains(config.animalType))
            {
                types.Add(config.animalType);
            }
        }
        
        return types;
    }
    
    void OnDestroy()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }
}

/// <summary>
/// 图像检测数据结构
/// </summary>
[System.Serializable]
public struct ImageDetectionData
{
    public string imageName;
    public Vector3 position;
}
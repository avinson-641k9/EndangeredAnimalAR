using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源管理器 - 负责动态加载和管理3D模型、纹理、音效等资源
/// </summary>
public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;
    
    // 缓存已加载的资源
    private Dictionary<string, Object> loadedResources = new Dictionary<string, Object>();
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static ResourceManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject rmObj = new GameObject("ResourceManager");
                instance = rmObj.AddComponent<ResourceManager>();
            }
            return instance;
        }
    }
    
    /// <summary>
    /// 异步加载资源
    /// </summary>
    public IEnumerator LoadResourceAsync<T>(string resourcePath, System.Action<T> onComplete) where T : Object
    {
        // 检查是否已缓存
        if (loadedResources.ContainsKey(resourcePath))
        {
            onComplete?.Invoke(loadedResources[resourcePath] as T);
            yield break;
        }
        
        ResourceRequest request = Resources.LoadAsync<T>(resourcePath);
        yield return request;
        
        if (request.asset != null)
        {
            loadedResources[resourcePath] = request.asset;
            onComplete?.Invoke(request.asset as T);
        }
        else
        {
            Debug.LogError($"[ResourceManager] 未能加载资源: {resourcePath}");
            onComplete?.Invoke(null);
        }
    }
    
    /// <summary>
    /// 同步加载资源
    /// </summary>
    public T LoadResource<T>(string resourcePath) where T : Object
    {
        // 检查是否已缓存
        if (loadedResources.ContainsKey(resourcePath))
        {
            return loadedResources[resourcePath] as T;
        }
        
        T resource = Resources.Load<T>(resourcePath);
        if (resource != null)
        {
            loadedResources[resourcePath] = resource;
        }
        else
        {
            Debug.LogError($"[ResourceManager] 未能加载资源: {resourcePath}");
        }
        
        return resource;
    }
    
    /// <summary>
    /// 卸载资源
    /// </summary>
    public void UnloadResource(string resourcePath)
    {
        if (loadedResources.ContainsKey(resourcePath))
        {
            // 注意：Resources.UnloadAsset只适用于非材质和纹理的资源
            // 对于材质和纹理，通常不应该卸载，因为其他对象可能在使用
            loadedResources.Remove(resourcePath);
        }
    }
    
    /// <summary>
    /// 清除所有缓存的资源
    /// </summary>
    public void ClearCache()
    {
        loadedResources.Clear();
    }
    
    /// <summary>
    /// 预加载常用资源
    /// </summary>
    public void PreloadCommonResources()
    {
        // 预加载常用的动物模型和资源
        StartCoroutine(PreloadAnimalResources());
    }
    
    private IEnumerator PreloadAnimalResources()
    {
        // 预加载常见的动物资源
        string[] commonAnimals = {
            "Animals/Panda",
            "Animals/Tiger", 
            "Animals/SnowLeopard",
            "Animals/Porpoise"
        };
        
        foreach (string animalPath in commonAnimals)
        {
            yield return LoadResourceAsync<GameObject>(animalPath, (obj) => {
                if (obj != null)
                {
                    Debug.Log($"[ResourceManager] 预加载动物资源: {animalPath}");
                }
            });
        }
    }
    
    /// <summary>
    /// 加载动物模型
    /// </summary>
    public IEnumerator LoadAnimalModel(string animalName, System.Action<GameObject> onComplete)
    {
        string resourcePath = $"Animals/{animalName}";
        yield return LoadResourceAsync<GameObject>(resourcePath, onComplete);
    }
    
    /// <summary>
    /// 加载动物音效
    /// </summary>
    public IEnumerator LoadAnimalAudio(string animalName, string audioType, System.Action<AudioClip> onComplete)
    {
        string resourcePath = $"Audio/{animalName}_{audioType}";
        yield return LoadResourceAsync<AudioClip>(resourcePath, onComplete);
    }
    
    /// <summary>
    /// 加载动物纹理
    /// </summary>
    public IEnumerator LoadAnimalTexture(string animalName, string textureType, System.Action<Texture2D> onComplete)
    {
        string resourcePath = $"Textures/{animalName}_{textureType}";
        yield return LoadResourceAsync<Texture2D>(resourcePath, onComplete);
    }
}
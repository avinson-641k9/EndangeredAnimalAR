using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// 简单场景创建器 - 在编辑器中创建最小可运行场景
/// </summary>
public class SimpleSceneCreator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/创建最小测试场景")]
    public static void CreateMinimalScene()
    {
        // 创建新场景
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        // 添加简单AR控制器
        GameObject arController = new GameObject("ARController");
        arController.AddComponent<SimpleARController>();
        
        // 创建简单UI
        CreateSimpleUI();
        
        // 保存场景
        string scenePath = "Assets/Scenes/MinimalTestScene.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        
        Debug.Log($"✅ 已创建最小测试场景: {scenePath}");
        Debug.Log("✅ 场景应该没有任何编译错误");
    }
    
    static void CreateSimpleUI()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // 创建测试按钮
        GameObject buttonObj = new GameObject("TestButton");
        buttonObj.transform.SetParent(canvasObj.transform);
        
        // 添加按钮组件
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.blue;
        
        Button button = buttonObj.AddComponent<Button>();
        
        // 添加按钮文本
        GameObject textObj = new GameObject("ButtonText");
        textObj.transform.SetParent(buttonObj.transform);
        
        Text text = textObj.AddComponent<Text>();
        text.text = "点击测试";
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 24;
        
        // 设置按钮大小和位置
        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 60);
        rect.anchoredPosition = Vector2.zero;
        
        // 设置文本大小
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        // 连接按钮事件
        button.onClick.AddListener(() => {
            Debug.Log("✅ 按钮点击测试成功！");
            FindObjectOfType<SimpleARController>()?.LogTest("按钮被点击");
        });
    }
#endif
}
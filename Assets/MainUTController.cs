using UnityEngine;
using UnityEngine.UI; // 引入UI库

public class MainUIController : MonoBehaviour
{
    // --- 这里预留接口，以后把其它画布拖进来 ---
    [Header("UI 面板接口")]
    public GameObject shopCanvas;      // 商店画布（以后做好了再拖进来）
    public GameObject inventoryCanvas; // 仓库画布（以后做好了再拖进来）
    public GameObject taskPanel;       // 任务面板

    // --- 这里是数据接口 ---
    [Header("数据展示接口")]
    public TMPro.TextMeshProUGUI moneyText; // 用来显示金钱的文本组件

    // 游戏开始时运行
    void Start()
    {
        Debug.Log("主界面初始化完成！");
        UpdateMoneyUI(100); // 假装我们有100块钱
    }

    // --- 下面是给按钮点击用的“空接口” ---

    // 1. 点击商店按钮
    public void OnShopButtonClicked()
    {
        Debug.Log("【测试】点击了商店按钮 -> 这里以后写：切换到商店画布的代码");
        // 比如：shopCanvas.SetActive(true);
    }

    // 2. 点击仓库按钮
    public void OnInventoryButtonClicked()
    {
        Debug.Log("【测试】点击了仓库按钮 -> 这里以后写：切换到仓库画布的代码");
    }

    // 3. 点击AR相机按钮
    public void OnARButtonClicked()
    {
        Debug.Log("【测试】点击了AR按钮 -> 这里以后写：加载AR场景的代码");
        // 比如：SceneManager.LoadScene("ARScene");
    }

    // 4. 预留给外部调用的刷新金钱接口
    public void UpdateMoneyUI(int currentMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = "$ " + currentMoney;
        }
    }
}
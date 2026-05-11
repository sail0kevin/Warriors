using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingButton : MonoBehaviour
{
    [Header("建筑配置")]
    public GameObject buildingPrefab;

    [Header("资源消耗（和你截图里的数值对应）")]
    public int woodCost; // 木头消耗（比如50）
    public int goldCost;  // 金币消耗（比如10）

    [Header("绑定UI文字（只用这一个Text！）")]
    public TextMeshProUGUI costText; // 你的CostText

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        // 自动生成两行文字：木头在上，金币在下，和你手动写的格式完全一样
        UpdateCostText();
    }

    // 核心：单Text里自动生成两行，和你UI里的格式100%匹配
    void UpdateCostText()
    {
        if (costText != null)
        {
            // 用\n换行，和你手动分行的效果完全一样
            costText.text = $"{woodCost}\n{goldCost}";
        }
    }

    // 点击按钮，检查资源是否足够
    void OnClick()
    {
        bool woodOk = ResourceManager.Instance.HasEnoughResource("wood", woodCost);
        bool goldOk = ResourceManager.Instance.HasEnoughResource("gold", goldCost);

        if (!woodOk || !goldOk)
        {
            Debug.Log("❌ 资源不足，无法建造！");
            return;
        }

        BuildingPlacementManager.Instance.StartPlacingBuilding(this);
    }

    // 扣资源方法（和之前一样，不用改）
    public bool SpendResources()
    {
        bool woodSpent = ResourceManager.Instance.SpendResource("wood", woodCost);
        if (!woodSpent) return false;

        bool goldSpent = ResourceManager.Instance.SpendResource("gold", goldCost);
        if (!goldSpent)
        {
            ResourceManager.Instance.AddResource("wood", woodCost);
            return false;
        }

        return true;
    }
}
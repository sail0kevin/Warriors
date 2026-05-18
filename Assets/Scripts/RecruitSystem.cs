using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 建筑招募系统。挂载在建筑上。
/// 点击建筑 → 打开/关闭该建筑的招募面板（通过 PanelManager）。
/// 每个建筑存自己的面板引用，不依赖任何全局查找。
/// </summary>
public class RecruitSystem : MonoBehaviour
{
    [System.Serializable]
    public class RecruitOption
    {
        public GameObject unitPrefab;
        public int woodCost;
        public int meatCost;
        public int goldCost;
        public int populationCost = 1;
        public float coolDown = 2f;
        [HideInInspector] public float currentCoolDown;
    }

    [Header("=== 生成设置 ===")]
    public Vector3 spawnOffset = new Vector3(1, 0, 0);

    [Header("=== 该建筑专属的招募面板（Canvas 下）===")]
    public GameObject recruitPanel;

    [Header("=== 招募选项 ===")]
    public List<RecruitOption> recruitOptions = new List<RecruitOption>();

    void Start()
    {
        // 确保游戏开始时招募面板是关闭的
        if (recruitPanel != null)
            recruitPanel.SetActive(false);
    }

    void Update()
    {
        // 冷却倒计时
        foreach (var opt in recruitOptions)
        {
            if (opt.currentCoolDown > 0f)
            {
                opt.currentCoolDown -= Time.deltaTime;
                if (opt.currentCoolDown < 0f) opt.currentCoolDown = 0f;
            }
        }
    }

    /// <summary>点击建筑 → 切换招募面板</summary>
    void OnMouseDown()
    {
        if (recruitPanel == null || PanelManager.Instance == null) return;

        // 每次打开前重新绑定按钮 → 多个建筑可共享同一面板
        WireUpButtons();
        PanelManager.Instance.TogglePanel(recruitPanel, this);
    }

    /// <summary>尝试招募指定选项（按钮事件调用）</summary>
    public void TryRecruit(int index)
    {
        if (index < 0 || index >= recruitOptions.Count) return;
        var opt = recruitOptions[index];

        if (opt.currentCoolDown > 0f)
        {
            Debug.Log("冷却中！");
            return;
        }

        if (!HasResources(opt))
        {
            Debug.Log("资源不足！");
            return;
        }

        if (!ResourceManager.Instance.HasPopulationCapacity(opt.populationCost))
        {
            Debug.Log("人口不足！");
            return;
        }

        SpendResources(opt);
        ResourceManager.Instance.UsePopulation(opt.populationCost);

        Vector3 pos = transform.position + spawnOffset;
        if (opt.unitPrefab != null)
        {
            var unit = Instantiate(opt.unitPrefab, pos, Quaternion.identity);
            var pop = unit.GetComponent<UnitPopulation>();
            if (pop != null) pop.recruited = true;
        }

        opt.currentCoolDown = opt.coolDown;
    }

    /// <summary>绑定面板内的按钮：先清除旧监听，再按子物体顺序匹配招募选项</summary>
    private void WireUpButtons()
    {
        if (recruitPanel == null) return;

        Button[] buttons = recruitPanel.GetComponentsInChildren<Button>();
        int count = Mathf.Min(buttons.Length, recruitOptions.Count);

        for (int i = 0; i < count; i++)
        {
            int idx = i; // 闭包安全
            buttons[i].onClick.RemoveAllListeners(); // 清除旧绑定（支持面板共享）
            buttons[i].onClick.AddListener(() => TryRecruit(idx));
        }
    }

    private bool HasResources(RecruitOption opt)
    {
        if (opt.woodCost > 0 && !ResourceManager.Instance.HasEnoughResource("wood", opt.woodCost)) return false;
        if (opt.meatCost > 0 && !ResourceManager.Instance.HasEnoughResource("meat", opt.meatCost)) return false;
        if (opt.goldCost > 0 && !ResourceManager.Instance.HasEnoughResource("gold", opt.goldCost)) return false;
        return true;
    }

    private void SpendResources(RecruitOption opt)
    {
        if (opt.woodCost > 0) ResourceManager.Instance.SpendResource("wood", opt.woodCost);
        if (opt.meatCost > 0) ResourceManager.Instance.SpendResource("meat", opt.meatCost);
        if (opt.goldCost > 0) ResourceManager.Instance.SpendResource("gold", opt.goldCost);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecruitSystem : MonoBehaviour
{
    [System.Serializable]
    public class RecruitOption
    {
        public GameObject unitPrefab;
        public int woodCost = 0;
        public int meatCost = 0;
        public int goldCost = 0;
        public float coolDown = 2f;
        [HideInInspector] public float currentCoolDown;
        [HideInInspector] public bool canRecruit = true;
    }

    [Header("=== 建筑基础设置 ===")]
    public Vector3 spawnOffset = new Vector3(1, 0, 0);

    [Header("=== 招募选项（按顺序对应面板里的按钮）===")]
    public List<RecruitOption> recruitOptions = new List<RecruitOption>();

    private GameObject recruitPanel;
    private GameObject recruitPanelOwner; // 当前谁打开了面板

    public void SetRecruitBinding(GameObject panel, List<Button> buttons)
    {
        recruitPanel = panel;

        if (buttons == null || buttons.Count == 0) return;

        int count = Mathf.Min(buttons.Count, recruitOptions.Count);
        for (int i = 0; i < count; i++)
        {
            var localOption = recruitOptions[i];
            var btn = buttons[i];
            btn.onClick.AddListener(() => OnRecruitButtonClicked(localOption));
        }
    }

    void OnRecruitButtonClicked(RecruitOption option)
    {
        if (recruitPanelOwner != null)
        {
            var rs = recruitPanelOwner.GetComponent<RecruitSystem>();
            if (rs != null) rs.SpawnUnit(option);
        }
    }

    void Start()
    {
        if (RecruitManager.Instance != null)
            RecruitManager.Instance.BindRecruitPanel(this);
    }

    void Update()
    {
        foreach (var option in recruitOptions)
        {
            if (!option.canRecruit)
            {
                option.currentCoolDown -= Time.deltaTime;
                if (option.currentCoolDown <= 0)
                    option.canRecruit = true;
            }
        }
    }

    void OnMouseDown()
    {
        if (recruitPanel == null) return;
        ToggleRecruitPanel();
    }

    public void ToggleRecruitPanel()
    {
        if (recruitPanel == null || PanelManager.Instance == null)
            return;

        recruitPanelOwner = gameObject;

        var buildingPanelToggle = FindAnyObjectByType<BuildingPanelToggle>();
        if (buildingPanelToggle != null)
            buildingPanelToggle.BuildingPanel.SetActive(false);

        PanelManager.Instance.TogglePanel(recruitPanel);
    }

    public void SpawnUnit(RecruitOption option)
    {
        if (!option.canRecruit)
        {
            Debug.Log("冷却中！");
            return;
        }

        if (!CheckResources(option))
        {
            Debug.Log("资源不足！");
            return;
        }

        SpendResources(option);

        Vector3 spawnPos = transform.position + spawnOffset;
        if (option.unitPrefab != null)
            Instantiate(option.unitPrefab, spawnPos, Quaternion.identity);

        option.canRecruit = false;
        option.currentCoolDown = option.coolDown;
    }

    private bool CheckResources(RecruitOption option)
    {
        if (option.woodCost > 0 && !ResourceManager.Instance.HasEnoughResource("wood", option.woodCost))
            return false;
        if (option.meatCost > 0 && !ResourceManager.Instance.HasEnoughResource("meat", option.meatCost))
            return false;
        if (option.goldCost > 0 && !ResourceManager.Instance.HasEnoughResource("gold", option.goldCost))
            return false;
        return true;
    }

    private void SpendResources(RecruitOption option)
    {
        if (option.woodCost > 0) ResourceManager.Instance.SpendResource("wood", option.woodCost);
        if (option.meatCost > 0) ResourceManager.Instance.SpendResource("meat", option.meatCost);
        if (option.goldCost > 0) ResourceManager.Instance.SpendResource("gold", option.goldCost);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelToggle : MonoBehaviour
{
    [Header("建筑面板")]
    public GameObject BuildingPanel;

    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (BuildingPanel == null) return;

        // 1. 点击按钮，先关闭所有招募面板
        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.CloseAllPanels();
        }

        // 2. 切换建筑面板状态（开 ↔ 关，逻辑绝对不会错）
        BuildingPanel.SetActive(!BuildingPanel.activeSelf);
    }
}
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 通用面板切换按钮。挂载在 UI 按钮上，Inspector 拖入要切换的面板。
/// 点击 → 通过 PanelManager 打开/关闭目标面板。
/// </summary>
public class BuildingPanelToggle : MonoBehaviour
{
    [Header("=== 要切换的面板 ===")]
    public GameObject targetPanel;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (targetPanel != null && PanelManager.Instance != null)
            PanelManager.Instance.TogglePanel(targetPanel);
    }
}

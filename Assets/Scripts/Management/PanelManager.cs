using UnityEngine;

/// <summary>
/// 通用面板管理器。保证同一时间只有一个面板打开。
/// 支持多个实体共享同一面板（通过 owner 参数区分）。
/// </summary>
public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    /// <summary>当前打开的面板</summary>
    public GameObject CurrentPanel { get; private set; }

    /// <summary>当前面板的所有者（哪个对象打开的）。同面板不同 owner 不会触发关闭。</summary>
    public object CurrentOwner { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>打开指定面板并记录所有者。自动关闭旧面板。</summary>
    public void OpenPanel(GameObject panel, object owner = null)
    {
        if (panel == null) return;

        // 关闭旧面板（不同面板才关）
        if (CurrentPanel != null && CurrentPanel != panel)
            CurrentPanel.SetActive(false);

        panel.SetActive(true);
        CurrentPanel = panel;
        CurrentOwner = owner;
    }

    /// <summary>切换面板：同面板+同所有者→关，否则→开（并切换所有者）</summary>
    public void TogglePanel(GameObject panel, object owner = null)
    {
        if (panel == null) return;

        // 同面板 + 同所有者 + 已打开 → 关闭
        if (CurrentPanel == panel && CurrentOwner == owner && panel.activeSelf)
        {
            panel.SetActive(false);
            CurrentPanel = null;
            CurrentOwner = null;
        }
        else
        {
            OpenPanel(panel, owner);
        }
    }

    /// <summary>关闭当前面板</summary>
    public void CloseCurrentPanel()
    {
        if (CurrentPanel != null)
        {
            CurrentPanel.SetActive(false);
            CurrentPanel = null;
            CurrentOwner = null;
        }
    }
}

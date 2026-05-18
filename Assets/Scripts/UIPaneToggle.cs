using UnityEngine;
using UnityEngine.UI;

public class UIPanelToggle : MonoBehaviour
{
    [Header("设置")]
    public GameObject TargetPanel;
    public bool PauseGameWhenOpen = false;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(TogglePanel);
    }

    void TogglePanel()
    {
        if (TargetPanel == null) return;

        // 先获取当前面板的状态
        bool isPanelCurrentlyOpen = TargetPanel.activeSelf;

        // 🔥 核心修复：
        // 1. 如果面板是【关闭的】，这次点击是要打开它 → 先关所有其他面板，再打开当前面板
        // 2. 如果面板是【打开的】，这次点击是要关闭它 → 直接关闭，不做其他操作
        if (!isPanelCurrentlyOpen)
        {
            // 打开面板前，先关闭所有其他面板（招募面板会被关掉）
            if (PanelManager.Instance != null)
            {
                PanelManager.Instance.CloseCurrentPanel();
            }

            // 打开当前建筑面板
            TargetPanel.SetActive(true);
        }
        else
        {
            // 直接关闭当前建筑面板，不触发任何其他逻辑
            TargetPanel.SetActive(false);
        }

        // 暂停游戏逻辑（保持不变）
        if (PauseGameWhenOpen)
        {
            Time.timeScale = TargetPanel.activeSelf ? 0 : 1;
        }
    }
}
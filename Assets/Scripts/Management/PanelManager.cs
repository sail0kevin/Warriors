using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    private GameObject currentOpenPanel;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 核心：点击建筑 打开/关闭面板
    public void TogglePanel(GameObject panel)
    {
        if (currentOpenPanel == panel)
        {
            panel.SetActive(false);
            currentOpenPanel = null;
        }
        else
        {
            if (currentOpenPanel != null)
                currentOpenPanel.SetActive(false);
            
            panel.SetActive(true);
            currentOpenPanel = panel;
        }
    }

    // 核心：建造时 关闭所有面板
    public void CloseAllPanels()
    {
        if (currentOpenPanel != null)
        {
            currentOpenPanel.SetActive(false);
            currentOpenPanel = null;
        }
    }
}
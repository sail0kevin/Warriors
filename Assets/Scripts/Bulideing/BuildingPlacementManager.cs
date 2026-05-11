using UnityEngine;

public class BuildingPlacementManager : MonoBehaviour
{
    // 单例
    public static BuildingPlacementManager Instance;

    [Header("预览设置")]
    public Color previewColor = new Color(1, 1, 1, 0.5f); // 半透明预览

    private BuildingButton currentBuildingButton; // 当前要放置的建筑配置
    private GameObject previewBuilding; // 半透明预览建筑
    private bool isPlacing = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isPlacing) return;

        // 1. 更新预览建筑的位置，跟着鼠标走
        UpdatePreviewPosition();

        // 2. 左键点击：放置建筑
        if (Input.GetMouseButtonDown(0))
        {
            PlaceBuilding();
        }

        // 3. 右键点击：取消放置
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacing();
        }
    }

    // 开始放置建筑
// 这是唯一的、无错误的方法，直接替换！
    // 这是唯一的、无错误的方法，直接替换！
    public void StartPlacingBuilding(BuildingButton button)
    {
        // 1. 点击建造按钮，自动关闭所有招募面板
        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.CloseAllPanels();
        }

        currentBuildingButton = button;
        isPlacing = true;

        // 销毁旧的预览建筑
        if (previewBuilding != null)
        {
            Destroy(previewBuilding);
        }

        // 生成新的预览建筑
        previewBuilding = Instantiate(button.buildingPrefab);
        previewBuilding.name = "BuildingPreview";

        // 设置半透明效果
        foreach (SpriteRenderer sr in previewBuilding.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = previewColor;
        }

        // 禁用预览碰撞体 → 解决单位东倒西歪
        foreach (Collider2D col in previewBuilding.GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }
    }
    // 更新预览位置，跟着鼠标移动
    void UpdatePreviewPosition()
    {
        // 把鼠标屏幕坐标转成2D世界坐标
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // 2D游戏，z轴设为0
        previewBuilding.transform.position = mouseWorldPos;
    }

    // 放置建筑，扣资源，实例化正式建筑
    void PlaceBuilding()
    {
        if (currentBuildingButton == null || previewBuilding == null) return;

    // 调用按钮的扣资源方法（木头+金币）
        bool canBuild = currentBuildingButton.SpendResources();
        if (!canBuild)
        {
        Debug.Log("❌ 资源不足，放置失败！");
        CancelPlacing();
        return;
        }

    // 实例化正式建筑
        Vector3 placePos = previewBuilding.transform.position;
        Instantiate(currentBuildingButton.buildingPrefab, placePos, Quaternion.identity);
        Debug.Log($"✅ 建造了 {currentBuildingButton.buildingPrefab.name}！");

        // 关闭放置模式
        CancelPlacing();
    }

    // 取消放置，销毁预览
    void CancelPlacing()
    {
        isPlacing = false;
        currentBuildingButton = null;
        Destroy(previewBuilding);
        previewBuilding = null;
    }
}
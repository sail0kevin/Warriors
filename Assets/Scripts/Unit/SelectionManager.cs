using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2D 单位框选 / 点选 / 右键移动。框选视觉用「世界空间 Sprite」，与 Physics2D 同一坐标系，不依赖 Canvas。
/// </summary>
public class SelectionManager : MonoBehaviour
{
    [Header("相机与平面")]
    [Tooltip("留空则用 Camera.main")]
    public Camera gameplayCamera;

    [Tooltip("单位与碰撞体所在的 Z（世界坐标）。正交相机常见为 0。")]
    public float gameplayPlaneZ = 0f;

    [Header("框选")]
    public float dragThreshold = 5f;

    [Header("框选条样式（世界空间 Sprite）")]
    [Tooltip("显示在所有单位之上")]
    public int boxSortingOrder = 500;

    public string boxSortingLayerName = "Default";

    [Tooltip("框选半透明颜色")]
    public Color boxColor = new Color(1f, 1f, 1f, 0.25f);

    private readonly List<UnitController> selectedUnits = new List<UnitController>();

    private Camera Cam => gameplayCamera != null ? gameplayCamera : Camera.main;

    private Vector2 startScreen;
    private Vector2 endScreen;
    private bool isDrag;

    private Transform boxRoot;
    private SpriteRenderer boxRenderer;
    private Sprite whiteSprite;

    void Awake()
    {
        BuildWorldBoxVisual();
    }

    void OnDestroy()
    {
        if (boxRoot != null)
            Destroy(boxRoot.gameObject);
        if (whiteSprite != null)
            Destroy(whiteSprite);
    }

    void BuildWorldBoxVisual()
    {
        var go = new GameObject("SelectionBoxWorld");
        go.transform.SetParent(transform, false);
        boxRoot = go.transform;

        boxRenderer = go.AddComponent<SpriteRenderer>();
        Texture2D t = Texture2D.whiteTexture;
        // 让 Sprite 在 scale=1 时约 1 世界单位宽，避免 4x4@PPU100 只有 0.04 单位导致框看不见
        float ppu = Mathf.Max(t.width, 1f);
        whiteSprite = Sprite.Create(
            t,
            new Rect(0, 0, t.width, t.height),
            new Vector2(0.5f, 0.5f),
            ppu,
            0,
            SpriteMeshType.FullRect);
        boxRenderer.sprite = whiteSprite;
        boxRenderer.color = boxColor;
        boxRenderer.sortingLayerName = boxSortingLayerName;
        boxRenderer.sortingOrder = boxSortingOrder;

        boxRoot.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Cam == null) return;
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startScreen = Input.mousePosition;
            endScreen = startScreen;
            isDrag = false;
            boxRoot.gameObject.SetActive(true);
            RefreshBoxVisual();
        }

        if (Input.GetMouseButton(0))
        {
            endScreen = Input.mousePosition;
            if (Vector2.Distance(startScreen, endScreen) > dragThreshold)
            {
                isDrag = true;
                RefreshBoxVisual();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            endScreen = Input.mousePosition;
            boxRoot.gameObject.SetActive(false);
            if (isDrag) SelectInBox();
            else SelectSingle();
        }

        if (Input.GetMouseButtonDown(1))
            MoveUnits();
    }

    void RefreshBoxVisual()
    {
        if (!TryScreenToWorldOnPlane(Cam, startScreen, gameplayPlaneZ, out Vector2 w0)) return;
        if (!TryScreenToWorldOnPlane(Cam, endScreen, gameplayPlaneZ, out Vector2 w1)) return;

        Vector2 min = Vector2.Min(w0, w1);
        Vector2 max = Vector2.Max(w0, w1);
        Vector2 center = (min + max) * 0.5f;
        Vector2 size = max - min;

        const float minPx = 0.02f;
        float sx = Mathf.Max(size.x, minPx);
        float sy = Mathf.Max(size.y, minPx);

        boxRoot.position = new Vector3(center.x, center.y, gameplayPlaneZ - 0.001f);
        boxRoot.localRotation = Quaternion.identity;
        boxRoot.localScale = new Vector3(sx, sy, 1f);
    }

    /// <summary>屏幕像素 → 与世界 Z = planeZ 平面的交点（XY）。</summary>
    static bool TryScreenToWorldOnPlane(Camera cam, Vector2 screenPixel, float planeZ, out Vector2 worldXY)
    {
        worldXY = Vector2.zero;
        if (cam == null) return false;

        Ray ray = cam.ScreenPointToRay(new Vector3(screenPixel.x, screenPixel.y, 0f));
        if (Mathf.Abs(ray.direction.z) < 1e-6f)
            return false;

        float t = (planeZ - ray.origin.z) / ray.direction.z;
        Vector3 p = ray.GetPoint(t);
        worldXY = new Vector2(p.x, p.y);
        return true;
    }

    void SelectInBox()
    {
        ClearSelected();
        Camera cam = Cam;
        if (cam == null) return;
        if (!TryScreenToWorldOnPlane(cam, startScreen, gameplayPlaneZ, out Vector2 a)) return;
        if (!TryScreenToWorldOnPlane(cam, endScreen, gameplayPlaneZ, out Vector2 b)) return;

        Vector2 min = Vector2.Min(a, b);
        Vector2 max = Vector2.Max(a, b);

        Collider2D[] cols = Physics2D.OverlapAreaAll(min, max);
        foreach (Collider2D c in cols)
        {
            UnitController unit = c.GetComponent<UnitController>();
            if (unit != null)
            {
                selectedUnits.Add(unit);
                unit.SetSelected(true);
            }
        }
    }

    void SelectSingle()
    {
        ClearSelected();
        Camera cam = Cam;
        if (cam == null) return;
        if (!TryScreenToWorldOnPlane(cam, Input.mousePosition, gameplayPlaneZ, out Vector2 origin)) return;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);
        if (hit.collider == null) return;

        UnitController unit = hit.collider.GetComponent<UnitController>();
        if (unit == null) return;

        selectedUnits.Add(unit);
        unit.SetSelected(true);
    }

    void MoveUnits()
    {
        if (selectedUnits.Count == 0) return;
        Camera cam = Cam;
        if (cam == null) return;
        if (!TryScreenToWorldOnPlane(cam, Input.mousePosition, gameplayPlaneZ, out Vector2 target)) return;

        // 根据地数量分散到目标点周围的网格
        int count = selectedUnits.Count;
        int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt((float)count / cols);
        float spacing = 0.6f;

        for (int i = 0; i < count; i++)
        {
            int row = i / cols;
            int col = i % cols;
            Vector2 offset = new Vector2(
                (col - (cols - 1) * 0.5f) * spacing,
                (row - (rows - 1) * 0.5f) * spacing
            );
            selectedUnits[i].SetMoveTarget(target + offset);
        }
    }

    void ClearSelected()
    {
        foreach (UnitController u in selectedUnits)
        {
            if (u != null) u.SetSelected(false);
        }
        selectedUnits.Clear();
    }
}

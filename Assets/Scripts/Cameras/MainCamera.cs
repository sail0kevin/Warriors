using UnityEngine;
using UnityEngine.EventSystems;

public class MainCamera : MonoBehaviour
{
    [Header("地图边界（整数）")]
    public int minX = -20, maxX = 20, minY = -15, maxY = 15;

    public float dragSpeed = 0.15f;
    public float edgeScrollSpeed = 8f;
    public float edgeBorder = 15f;
    public float zoomSpeed = 2f;
    public float minOrthoSize = 5f;
    public float maxOrthoSize = 15f;

    [Tooltip("瓦片/单位所在世界 Z，用于把鼠标正确投到 XY 平面（缩放以鼠标为中心）")]
    public float worldPlaneZ = 0f;

    Camera cam;
    Vector3 lastMouse;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("RTS 相机脚本需挂在带 Camera 的对象上。");
            enabled = false;
            return;
        }
        cam.orthographic = true;
    }

    void Update()
    {
        bool pointerOnUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (!pointerOnUI)
        {
            if (Input.GetMouseButtonDown(0))
                lastMouse = Input.mousePosition;
            if (Input.GetMouseButton(0))
            {
                Vector3 d = Input.mousePosition - lastMouse;
                transform.Translate(new Vector3(-d.x * dragSpeed, -d.y * dragSpeed, 0f), Space.World);
                lastMouse = Input.mousePosition;
            }

            Vector2 mp = Input.mousePosition;
            Vector3 edge = Vector3.zero;
            if (mp.x < edgeBorder) edge.x = -1f;
            if (mp.x > Screen.width - edgeBorder) edge.x = 1f;
            if (mp.y < edgeBorder) edge.y = -1f;
            if (mp.y > Screen.height - edgeBorder) edge.y = 1f;
            transform.Translate(edge * edgeScrollSpeed * Time.deltaTime, Space.World);
        }

        // 滚轮缩放：不随 UI 检测一起屏蔽，否则全屏/大面积 UI 会导致无法缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float oldSize = cam.orthographicSize;
            float newSize = Mathf.Clamp(oldSize - scroll * zoomSpeed, minOrthoSize, maxOrthoSize);
            Vector3 pivot = MouseWorldOnPlaneXY();
            float ratio = newSize / oldSize;
            Vector3 pos = pivot + (transform.position - pivot) * ratio;
            pos.z = transform.position.z;
            cam.orthographicSize = newSize;
            transform.position = pos;
        }

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, minX + halfW, maxX - halfW);
        p.y = Mathf.Clamp(p.y, minY + halfH, maxY - halfH);
        transform.position = p;
    }

    Vector3 MouseWorldOnPlaneXY()
    {
        Vector3 m = Input.mousePosition;
        m.z = cam.WorldToScreenPoint(new Vector3(0f, 0f, worldPlaneZ)).z;
        Vector3 w = cam.ScreenToWorldPoint(m);
        w.z = transform.position.z;
        return w;
    }
}

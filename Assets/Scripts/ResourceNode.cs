using UnityEngine;

public enum ResourceType
{
    Wood,
    Gold,
    Meat,
}

/// <summary>
/// 挂在场景资源物体上（树、金矿、羊等）。
/// 支持随采集进度改变大小，和采集完后生成残骸（树桩）。
/// </summary>
public class ResourceNode : MonoBehaviour
{
    public ResourceType resourceType = ResourceType.Wood;
    public int amount = 100;
    public float gatherRadius = 1f;

    [Header("视觉变化（可选）")]
    [Tooltip("随资源减少缩放 Sprite（金矿从大到小）")]
    public bool useScaleDepletion = false;
    [Tooltip("采集完后生成的物体（如树桩 prefab）")]
    public GameObject depletedPrefab;

    private int initialAmount;
    private SpriteRenderer sr;

    void Awake()
    {
        initialAmount = amount;
        sr = GetComponent<SpriteRenderer>();
    }

    public void OnGathered(int takenAmount)
    {
        amount -= takenAmount;

        // 缩放变化（金矿越采越小）
        if (useScaleDepletion && sr != null)
        {
            float ratio = Mathf.Max(0.2f, (float)amount / initialAmount);
            transform.localScale = Vector3.one * ratio;
        }

        // 枯竭
        if (amount <= 0)
        {
            if (depletedPrefab != null)
                Instantiate(depletedPrefab, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}

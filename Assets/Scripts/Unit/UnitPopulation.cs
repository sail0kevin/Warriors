using UnityEngine;

/// <summary>
/// 单位人口组件。挂载在单位 Prefab 上。
/// 场景预置单位：Start 时自动消耗人口。
/// 招募生成单位：由 RecruitSystem 标记跳过，避免重复扣除。
/// 死亡时自动释放人口。
/// </summary>
public class UnitPopulation : MonoBehaviour
{
    [Header("=== 该单位占用的人口 ===")]
    public int populationCost = 1;

    /// <summary>招募系统生成时设为 true，跳过 Start 的人口扣除</summary>
    [HideInInspector] public bool recruited;

    void Start()
    {
        var hs = GetComponent<HealthSystem>();
        if (hs != null)
            hs.OnDied += OnUnitDied;

        // 场景预置单位 → 自动消耗人口
        if (!recruited && ResourceManager.Instance != null)
            ResourceManager.Instance.UsePopulation(populationCost);
    }

    void OnUnitDied()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.ReleasePopulation(populationCost);
    }
}

using UnityEngine;

/// <summary>
/// 建筑人口组件。挂载在建筑 Prefab 上。
/// 建筑生成时自动增加人口上限，销毁时自动扣除。
/// </summary>
public class BuildingPopulation : MonoBehaviour
{
    [Header("=== 该建筑提供的人口上限 ===")]
    public int populationBonus = 20;

    void Start()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.AddMaxPopulation(populationBonus);
    }

    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.RemoveMaxPopulation(populationBonus);
    }
}

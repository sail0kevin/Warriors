using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("初始资源")]
    public int gold = 1000;
    public int meat = 1000;
    public int wood = 1000;

    [Header("人口")]
    public int currentPopulation = 0;
    public int maxPopulation = 0;
    public const int POPULATION_CAP = 200;

    void Awake()
    {
        Instance = this;
    }

    // ==================== 资源 ====================

    public void AddResource(string type, int amount)
    {
        switch (type.ToLower())
        {
            case "gold": gold += amount; break;
            case "meat": meat += amount; break;
            case "wood": wood += amount; break;
        }
        UIManager.Instance?.UpdateResourceUI();
    }

    public bool HasEnoughResource(string type, int amount)
    {
        switch (type.ToLower())
        {
            case "gold": return gold >= amount;
            case "meat": return meat >= amount;
            case "wood": return wood >= amount;
            default: return false;
        }
    }

    public bool SpendResource(string type, int amount)
    {
        bool enough = false;
        switch (type.ToLower())
        {
            case "gold": enough = gold >= amount; if (enough) gold -= amount; break;
            case "meat": enough = meat >= amount; if (enough) meat -= amount; break;
            case "wood": enough = wood >= amount; if (enough) wood -= amount; break;
        }
        if (enough) UIManager.Instance?.UpdateResourceUI();
        return enough;
    }

    // ==================== 人口 ====================

    /// <summary>建筑生成时调用，增加人口上限（不超过全局上限200）</summary>
    public void AddMaxPopulation(int amount)
    {
        maxPopulation += amount;
        if (maxPopulation > POPULATION_CAP)
            maxPopulation = POPULATION_CAP;
        UIManager.Instance?.UpdateResourceUI();
    }

    /// <summary>建筑销毁时调用，减少人口上限</summary>
    public void RemoveMaxPopulation(int amount)
    {
        maxPopulation -= amount;
        if (maxPopulation < 0) maxPopulation = 0;
        if (currentPopulation > maxPopulation)
            currentPopulation = maxPopulation;
        UIManager.Instance?.UpdateResourceUI();
    }

    /// <summary>招募单位时检查人口是否足够</summary>
    public bool HasPopulationCapacity(int amount)
    {
        return currentPopulation + amount <= maxPopulation;
    }

    /// <summary>招募单位时消耗人口</summary>
    public void UsePopulation(int amount)
    {
        currentPopulation += amount;
        UIManager.Instance?.UpdateResourceUI();
    }

    /// <summary>单位死亡时释放人口</summary>
    public void ReleasePopulation(int amount)
    {
        currentPopulation -= amount;
        if (currentPopulation < 0) currentPopulation = 0;
        UIManager.Instance?.UpdateResourceUI();
    }
}

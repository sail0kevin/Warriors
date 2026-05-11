using UnityEngine;

// 必须和文件名一致
public class ResourceManager : MonoBehaviour
{
    // 全局唯一单例（修复Instance报错）
    public static ResourceManager Instance { get; private set; }

    [Header("初始资源")]
    public int gold = 1000;
    public int meat = 1000;
    public int wood = 1000;

    void Awake()
    {
        Instance = this;
    }

    // 增加资源
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
    
    // 检查资源是否足够（建造前预检查）
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

    // 扣除资源
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
}
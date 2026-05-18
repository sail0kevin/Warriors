using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("资源文字")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI meatText;
    public TextMeshProUGUI woodText;

    [Header("人口文字")]
    public TextMeshProUGUI populationText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateResourceUI()
    {
        var rm = ResourceManager.Instance;
        if (rm == null) return;

        if (goldText != null) goldText.text = rm.gold.ToString();
        if (meatText != null) meatText.text = rm.meat.ToString();
        if (woodText != null) woodText.text = rm.wood.ToString();
        if (populationText != null)
            populationText.text = $"{rm.currentPopulation}/{rm.maxPopulation}";
    }

    void Start()
    {
        UpdateResourceUI();
    }
}

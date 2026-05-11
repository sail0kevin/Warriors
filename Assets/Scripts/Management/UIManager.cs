using UnityEngine;
using TMPro; // 启用TextMeshPro

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("绑定 TextMeshPro 文字")]
    public TextMeshProUGUI goldText;  // 改成TMP
    public TextMeshProUGUI meatText;
    public TextMeshProUGUI woodText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateResourceUI()
    {
        goldText.text = ResourceManager.Instance.gold.ToString();
        meatText.text = ResourceManager.Instance.meat.ToString();
        woodText.text = ResourceManager.Instance.wood.ToString();
    }

    void Start()
    {
        UpdateResourceUI();
    }
}
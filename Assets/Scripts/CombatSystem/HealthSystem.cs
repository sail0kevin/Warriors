using UnityEngine;
using System;

[DefaultExecutionOrder(-30)]
public class HealthSystem : MonoBehaviour
{
    [Header("血量")]
    public float maxHealth = 100f;
    [Tooltip("血量归零时自动销毁物体（建筑用 true，单位用 false）")]
    public bool destroyOnDeath = false;

    [Header("血条显示")]
    [SerializeField] private float barWidth = 1.2f;
    [SerializeField] private float barHeight = 0.12f;
    [SerializeField] private float yOffset = 0.6f;
    [Tooltip("血量比例高于此值时自动隐藏（满血不显示）")]
    [SerializeField] [Range(0.01f, 1f)] private float hideThreshold = 0.99f;

    [Header("颜色")]
    [SerializeField] private Color highHealthColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color midHealthColor = new Color(0.9f, 0.8f, 0.1f);
    [SerializeField] private Color lowHealthColor = new Color(0.9f, 0.1f, 0.1f);

    private GameObject backgroundBar;
    private GameObject fillBar;
    private SpriteRenderer backgroundSR;
    private SpriteRenderer fillSR;
    private Sprite whiteSprite;
    private bool isForcedVisible;
    private float hp;

    public float CurrentHealth => hp;
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    public Action OnDied;
    public Action<float, float> OnHealthChanged;

    void Awake()
    {
        hp = maxHealth;
        CreateBar();
        HideBar();
    }

    void CreateBar()
    {
        whiteSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f, 0, SpriteMeshType.FullRect);

        backgroundBar = new GameObject("HealthBarBG");
        backgroundBar.transform.SetParent(transform, false);
        backgroundSR = backgroundBar.AddComponent<SpriteRenderer>();
        backgroundSR.sprite = whiteSprite;
        backgroundSR.color = Color.gray;
        backgroundSR.sortingOrder = 10;
        backgroundBar.transform.localPosition = Vector3.up * yOffset;
        backgroundBar.transform.localScale = new Vector3(barWidth, barHeight, 1f);

        fillBar = new GameObject("HealthBarFill");
        fillBar.transform.SetParent(transform, false);
        fillSR = fillBar.AddComponent<SpriteRenderer>();
        fillSR.sprite = whiteSprite;
        fillSR.color = highHealthColor;
        fillSR.sortingOrder = 11;
        fillBar.transform.localPosition = Vector3.up * yOffset;
        fillBar.transform.localScale = new Vector3(barWidth, barHeight, 1f);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        hp -= amount;

        OnHealthChanged?.Invoke(hp, maxHealth);
        if (hp <= 0f)
        {
            IsDead = true;
            HideBar();
            OnDied?.Invoke();
            if (destroyOnDeath) Destroy(gameObject);
        }
        else
        {
            UpdateBarVisual();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        hp = Mathf.Min(hp + amount, maxHealth);
        OnHealthChanged?.Invoke(hp, maxHealth);
        UpdateBarVisual();
    }

    void UpdateBarVisual()
    {
        float ratio = Mathf.Clamp01(hp / maxHealth);
        fillBar.transform.localScale = new Vector3(barWidth * ratio, barHeight, 1f);

        if (ratio > 0.6f)
            fillSR.color = highHealthColor;
        else if (ratio > 0.3f)
            fillSR.color = midHealthColor;
        else
            fillSR.color = lowHealthColor;

        if (isForcedVisible)
            ShowBar();
        else if (ratio < hideThreshold)
            ShowBar();
        else
            HideBar();
    }

    void ShowBar()
    {
        if (backgroundBar != null) backgroundBar.SetActive(true);
        if (fillBar != null) fillBar.SetActive(true);
    }

    void HideBar()
    {
        if (!isForcedVisible)
        {
            if (backgroundBar != null) backgroundBar.SetActive(false);
            if (fillBar != null) fillBar.SetActive(false);
        }
    }

    public void SetForcedVisible(bool value)
    {
        isForcedVisible = value;
        if (value)
            ShowBar();
    }

    void OnDestroy()
    {
        if (whiteSprite != null) Destroy(whiteSprite);
    }
}

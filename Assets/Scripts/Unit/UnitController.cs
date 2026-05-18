using UnityEngine;

public class UnitController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public bool isSelected;

    [Header("到达判定")]
    [Tooltip("小于此距离视为已到达点击点")]
    public float stopDistance = 0.28f;

    [Tooltip("离目标不超过此距离时，才允许「挤在一起动不了」时提前结束行走")]
    public float stuckNearTargetRadius = 2.2f;

    [Tooltip("连续这么长时间几乎没在靠近目标 → 视为被挡住，结束行走状态")]
    public float stuckNoProgressTime = 0.45f;

    [Tooltip("单帧向目标靠近不足此值则累积「无进展」计时（与 stuckNoProgressTime 配合）")]
    public float minProgressPerFrame = 0.012f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 targetPos;
    public bool IsMoving { get; private set; }
    private float lastDistanceToTarget;
    private float noProgressTimer;
    private bool combatRooting;
    private HealthSystem healthSystem;
    private UnitCombat unitCombat;
    private Color originalColor;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        targetPos = transform.position;
        healthSystem = GetComponent<HealthSystem>();
        unitCombat = GetComponent<UnitCombat>();
        originalColor = GetComponent<SpriteRenderer>().color;

        // 没有 UnitCombat 的单位（农民、治疗师等）也要能正常死亡
        if (healthSystem != null && unitCombat == null)
            healthSystem.OnDied += OnUnitDiedWithoutCombat;
    }

    void OnUnitDiedWithoutCombat()
    {
        enabled = false;
        if (rb != null) rb.velocity = Vector2.zero;
        Destroy(gameObject, 0.5f);
    }

    void Update()
    {
        if (combatRooting)
        {
            rb.velocity = Vector2.zero;
            if (anim != null) anim.SetBool("IsMoving", false);
            return;
        }

        Vector2 pos = transform.position;
        float dis = Vector2.Distance(pos, targetPos);

        if (!IsMoving)
        {
            rb.velocity = Vector2.zero;
            if (anim != null) anim.SetBool("IsMoving", false);
            return;
        }

        if (dis <= stopDistance)
        {
            StopMoving();
            return;
        }

        // 很多单位挤同一落点：物理顶死，距离永远略大于 stopDistance → 用「长时间几乎没在靠近」结束行走
        float progressTowardTarget = lastDistanceToTarget - dis;
        if (dis <= stuckNearTargetRadius && progressTowardTarget < minProgressPerFrame)
            noProgressTimer += Time.deltaTime;
        else
            noProgressTimer = 0f;

        lastDistanceToTarget = dis;

        if (noProgressTimer >= stuckNoProgressTime)
        {
            StopMoving();
            return;
        }

        Vector2 dir = (targetPos - pos).normalized;
        rb.velocity = dir * moveSpeed;

        if (anim != null)
            anim.SetBool("IsMoving", true);
    }

    void StopMoving()
    {
        IsMoving = false;
        noProgressTimer = 0f;
        rb.velocity = Vector2.zero;
        if (anim != null)
            anim.SetBool("IsMoving", false);
    }

    public void SetMoveTarget(Vector2 pos)
    {
        combatRooting = false;
        targetPos = pos;
        IsMoving = true;
        lastDistanceToTarget = Vector2.Distance(transform.position, targetPos);
        noProgressTimer = 0f;
    }

    /// <summary>战斗时定身：不移动、不推速度，由 UnitCombat 开关。</summary>
    public void SetCombatRooting(bool rooted)
    {
        combatRooting = rooted;
        if (rooted)
            StopMoving();
    }

    public void SetSelected(bool sel)
    {
        isSelected = sel;
        GetComponent<SpriteRenderer>().color = sel ? Color.yellow : originalColor;

        if (healthSystem != null)
        {
            if (sel)
            {
                healthSystem.SetForcedVisible(true);
            }
            else
            {
                healthSystem.SetForcedVisible(false);
                if (unitCombat != null && unitCombat.CurrentHealth >= unitCombat.MaxHealth * 0.99f)
                    healthSystem.SetForcedVisible(false);
            }
        }
    }
}
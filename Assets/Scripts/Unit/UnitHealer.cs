using System.Collections;
using UnityEngine;

/// <summary>
/// 治疗师：搜索友方残血单位，自动接近并治疗。
/// 没有攻击能力，不与 UnitCombat 共存。
/// </summary>
[RequireComponent(typeof(UnitController))]
public class UnitHealer : MonoBehaviour
{
    [Header("数值")]
    public float healAmount = 20f;
    [Tooltip("治疗距离（进入此范围才会执行治疗）")]
    public float healRange = 1.2f;
    [Tooltip("搜索友方伤员的距离")]
    public float detectRange = 5f;
    public float healCooldown = 1.5f;

    [Header("动画")]
    [Tooltip("Animator 里存在的治疗 Trigger 名")]
    public string healTriggerName = "Heal";

    UnitController unit;
    Animator anim;
    SpriteRenderer spriteRenderer;
    Collider2D bodyCollider;

    float nextHealReadyTime;
    bool healRoutineRunning;

    void Awake()
    {
        unit = GetComponent<UnitController>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bodyCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        var target = FindClosestWoundedAlly();

        if (target.hs != null && target.col != null)
        {
            FaceToward(target.col.transform.position);

            float dist = GetEdgeDistance(target.col);

            if (dist <= healRange)
            {
                // 在治疗范围内 → 定身，治疗
                unit.SetCombatRooting(true);
                if (!healRoutineRunning && Time.time >= nextHealReadyTime)
                    StartCoroutine(HealRoutine(target.hs));
            }
            else if (!unit.isSelected)
            {
                // 在搜索范围内但超出治疗范围 → 自动接近伤员
                unit.SetCombatRooting(false);
                unit.SetMoveTarget(target.col.transform.position);
            }
            else
            {
                unit.SetCombatRooting(false);
            }
        }
        else
        {
            unit.SetCombatRooting(false);
        }
    }

    (HealthSystem hs, Collider2D col) FindClosestWoundedAlly()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange);
        HealthSystem best = null;
        Collider2D bestCol = null;
        float bestRatio = 1f;
        string myTag = gameObject.tag;

        foreach (Collider2D h in hits)
        {
            if (h == bodyCollider) continue;

            // 只治疗单位（有 UnitController），不治疗建筑
            if (h.GetComponent<UnitController>() == null) continue;

            HealthSystem hs = h.GetComponent<HealthSystem>();
            if (hs == null || hs.IsDead) continue;

            float ratio = hs.CurrentHealth / hs.MaxHealth;
            if (ratio >= 1f) continue; // 满血不治

            // 只治疗同一 Tag 阵营的
            if (!h.CompareTag(myTag)) continue;

            // 优先治疗血量比例最低的
            if (ratio < bestRatio)
            {
                bestRatio = ratio;
                best = hs;
                bestCol = h;
            }
        }

        return (best, bestCol);
    }

    IEnumerator HealRoutine(HealthSystem targetHS)
    {
        healRoutineRunning = true;
        nextHealReadyTime = Time.time + healCooldown;

        if (anim != null && !string.IsNullOrEmpty(healTriggerName))
        {
            foreach (AnimatorControllerParameter p in anim.parameters)
            {
                if (p.name == healTriggerName && p.type == AnimatorControllerParameterType.Trigger)
                {
                    anim.SetTrigger(healTriggerName);
                    break;
                }
            }
        }

        yield return new WaitForSeconds(0.3f);

        if (targetHS != null && !targetHS.IsDead)
            targetHS.Heal(healAmount);

        healRoutineRunning = false;
    }

    void FaceToward(Vector3 worldPos)
    {
        if (spriteRenderer == null) return;
        float dx = worldPos.x - transform.position.x;
        if (dx > 0.02f) spriteRenderer.flipX = false;
        else if (dx < -0.02f) spriteRenderer.flipX = true;
    }

    float GetEdgeDistance(Collider2D targetCol)
    {
        if (bodyCollider != null && targetCol != null)
            return Mathf.Max(0f, bodyCollider.Distance(targetCol).distance);
        return Vector2.Distance(transform.position, targetCol != null ? targetCol.transform.position : transform.position);
    }
}

using System.Collections;
using UnityEngine;

/// <summary>
/// 血量、攻击力、自动索敌近战（同范围内最近异队单位）。攻击时定身并尝试触发 Animator 上的 Attack 类 Trigger。
/// 请在 Animator 里添加 Trigger 参数（如 Attack / Attack1），并接上攻击动画状态。
/// </summary>
[DefaultExecutionOrder(-20)]
[RequireComponent(typeof(UnitController))]
public class UnitCombat : MonoBehaviour
{
    [Header("数值（可按兵种在 Inspector 里改）")]
    public float attackDamage = 14f;
    [Tooltip("攻击距离（进入此范围才会出刀）")]
    public float attackRange = 1.05f;
    [Tooltip("索敌/仇恨距离（进入此范围会朝向并开始追击）")]
    public float detectRange = 4f;
    public float attackCooldown = 1f;
    [Tooltip("出招后延迟多久结算伤害，略对齐挥砍帧")]
    public float damageHitDelay = 0.22f;

    [Header("动画")]
    [Tooltip("Animator 里存在的 Trigger 名；若不存在会依次尝试 Attack、Attack1、Attack2")]
    public string attackTriggerName = "Attack";

    UnitController unit;
    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Collider2D bodyCollider;
    HealthSystem healthSystem;

    float nextAttackReadyTime;
    bool attackRoutineRunning;

    static readonly string[] FallbackAttackTriggers = { "Attack", "Attack1", "Attack2" };

    public float CurrentHealth => healthSystem != null ? healthSystem.CurrentHealth : 0f;
    public float MaxHealth => healthSystem != null ? healthSystem.MaxHealth : 0f;
    public bool IsDead => healthSystem != null && healthSystem.IsDead;

    void Awake()
    {
        unit = GetComponent<UnitController>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDied += OnUnitDied;
    }

    void OnUnitDied()
    {
        StopAllCoroutines();
        attackRoutineRunning = false;
        unit.SetCombatRooting(false);
        unit.enabled = false;
        enabled = false;
        if (rb != null) rb.velocity = Vector2.zero;
        if (bodyCollider != null) bodyCollider.enabled = false;
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0.45f;
            spriteRenderer.color = c;
        }
        Destroy(gameObject, 1.6f);
    }

    Transform activeTarget; // 当前追击的敌人

    void Update()
    {
        if (healthSystem.IsDead) return;

        var target = FindClosestHostileInRange();

        if (target.position != null)
        {
            activeTarget = target.position;

            // 始终面向敌人
            FaceToward(activeTarget.position);

            float distToTarget = GetEdgeDistance(target.targetCollider);

            if (distToTarget <= attackRange)
            {
                // 在攻击范围内 → 定身，攻击
                unit.SetCombatRooting(true);
                if (!attackRoutineRunning && Time.time >= nextAttackReadyTime)
                    StartCoroutine(AttackRoutine(target));
            }
            else if (!unit.isSelected)
            {
                // 在仇恨范围内但超出攻击范围 → 未选中的单位自动追击
                unit.SetCombatRooting(false);
                unit.SetMoveTarget(activeTarget.position);
            }
            else
            {
                // 选中的单位 → 只面向，玩家自己控制移动
                unit.SetCombatRooting(false);
            }
        }
        else
        {
            activeTarget = null;
            unit.SetCombatRooting(false);
        }
    }

    void FaceToward(Vector3 worldPos)
    {
        if (spriteRenderer == null) return;
        float dx = worldPos.x - transform.position.x;
        if (dx > 0.02f) spriteRenderer.flipX = false;
        else if (dx < -0.02f) spriteRenderer.flipX = true;
    }

    (Transform position, Collider2D targetCollider, HealthSystem hs, UnitCombat uc) FindClosestHostileInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange);
        Transform bestPos = null;
        Collider2D bestCol = null;
        HealthSystem bestHS = null;
        UnitCombat bestUC = null;
        float bestDist = float.MaxValue;
        bool isSelfEnemy = gameObject.CompareTag("Enemy");

        foreach (Collider2D h in hits)
        {
            if (h == bodyCollider) continue;

            bool isHostile = h.CompareTag("Enemy") != isSelfEnemy;

            // 优先找 UnitCombat 类型的敌人
            UnitCombat otherUC = h.GetComponent<UnitCombat>();
            if (otherUC != null && otherUC != this && !otherUC.IsDead && isHostile)
            {
                float d = Vector2.SqrMagnitude((Vector2)h.transform.position - (Vector2)transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestPos = h.transform;
                    bestCol = h;
                    bestUC = otherUC;
                    bestHS = null;
                }
                continue;
            }

            // 再找带 HealthSystem 的敌方建筑（无 UnitController = 建筑）
            HealthSystem otherHS = h.GetComponent<HealthSystem>();
            if (otherHS != null && !otherHS.IsDead && h.GetComponent<UnitController>() == null && isHostile)
            {
                float d = Vector2.SqrMagnitude((Vector2)h.transform.position - (Vector2)transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestPos = h.transform;
                    bestCol = h;
                    bestHS = otherHS;
                    bestUC = null;
                }
            }
            // 再再找带 HealthSystem 的敌方单位（没有 UnitCombat 的单位，如治疗师）
            else if (otherHS != null && !otherHS.IsDead && h.GetComponent<UnitController>() != null && isHostile)
            {
                float d = Vector2.SqrMagnitude((Vector2)h.transform.position - (Vector2)transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestPos = h.transform;
                    bestCol = h;
                    bestHS = otherHS;
                    bestUC = null;
                }
            }
        }

        if (bestPos == null) return (null, null, null, null);
        return (bestPos, bestCol, bestHS, bestUC);
    }

    float GetEdgeDistance(Collider2D targetCol)
    {
        if (bodyCollider != null && targetCol != null)
            return Mathf.Max(0f, bodyCollider.Distance(targetCol).distance);
        return Vector2.Distance(transform.position, targetCol != null ? targetCol.transform.position : transform.position);
    }

    IEnumerator AttackRoutine((Transform position, Collider2D targetCollider, HealthSystem hs, UnitCombat uc) target)
    {
        attackRoutineRunning = true;
        nextAttackReadyTime = Time.time + attackCooldown;
        TryFireAttackTrigger();

        yield return new WaitForSeconds(damageHitDelay);

        if (target.position != null && target.hs != null && !target.hs.IsDead)
        {
            if (GetEdgeDistance(target.targetCollider) <= attackRange * 1.2f)
                target.hs.TakeDamage(attackDamage);
        }
        else if (target.position != null && target.uc != null && !target.uc.IsDead)
        {
            if (GetEdgeDistance(target.targetCollider) <= attackRange * 1.2f)
                target.uc.TakeDamage(attackDamage);
        }

        attackRoutineRunning = false;
    }

    void TryFireAttackTrigger()
    {
        if (anim == null) return;

        if (!string.IsNullOrEmpty(attackTriggerName) && AnimatorHasTrigger(anim, attackTriggerName))
        {
            anim.SetTrigger(attackTriggerName);
            return;
        }

        foreach (string t in FallbackAttackTriggers)
        {
            if (AnimatorHasTrigger(anim, t))
            {
                anim.SetTrigger(t);
                return;
            }
        }
    }

    static bool AnimatorHasTrigger(Animator a, string name)
    {
        foreach (AnimatorControllerParameter p in a.parameters)
        {
            if (p.name == name && p.type == AnimatorControllerParameterType.Trigger)
                return true;
        }

        return false;
    }

    public void TakeDamage(float amount)
    {
        if (healthSystem != null)
            healthSystem.TakeDamage(amount);
    }
}

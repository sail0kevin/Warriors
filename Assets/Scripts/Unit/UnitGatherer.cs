using System.Collections;
using UnityEngine;

/// <summary>
/// 农民采集行为：自动搜索附近资源 → 走向资源 → 播放采集动画 → 获得资源。
/// </summary>
[RequireComponent(typeof(UnitController))]
public class UnitGatherer : MonoBehaviour
{
    [Header("采集设置")]
    public float gatherRange = 1f;
    public float detectRange = 6f;
    public float gatherAmount = 5f;
    public float gatherCooldown = 1.5f;

    [Header("动画")]
    [Tooltip("Animator Trigger 名前缀，后面自动加资源类型：GatherWood / GatherGold / GatherMeat")]
    public string gatherTriggerPrefix = "Gather";

    UnitController unit;
    Animator anim;
    SpriteRenderer spriteRenderer;
    Collider2D bodyCollider;
    ResourceNode currentTarget;
    float nextGatherTime;
    bool gatherRoutineRunning;

    void Awake()
    {
        unit = GetComponent<UnitController>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bodyCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (currentTarget == null || currentTarget.amount <= 0)
            currentTarget = FindNearestResource();

        if (currentTarget != null)
        {
            FaceToward(currentTarget.transform.position);

            float dist = Vector2.Distance(transform.position, currentTarget.transform.position);

            if (dist <= gatherRange)
            {
                unit.SetCombatRooting(true);
                if (!gatherRoutineRunning && Time.time >= nextGatherTime)
                    StartCoroutine(GatherRoutine());
            }
            else if (!unit.isSelected)
            {
                unit.SetCombatRooting(false);
                unit.SetMoveTarget(currentTarget.transform.position);
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

    ResourceNode FindNearestResource()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange);
        ResourceNode best = null;
        float bestDist = float.MaxValue;

        foreach (Collider2D h in hits)
        {
            ResourceNode node = h.GetComponent<ResourceNode>();
            if (node == null || node.amount <= 0) continue;
            float d = Vector2.SqrMagnitude((Vector2)h.transform.position - (Vector2)transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = node;
            }
        }

        return best;
    }

    IEnumerator GatherRoutine()
    {
        gatherRoutineRunning = true;
        nextGatherTime = Time.time + gatherCooldown;

        // 播放对应动画
        if (anim != null && currentTarget != null)
        {
            string triggerName = gatherTriggerPrefix + currentTarget.resourceType.ToString();
            foreach (AnimatorControllerParameter p in anim.parameters)
            {
                if (p.name == triggerName && p.type == AnimatorControllerParameterType.Trigger)
                {
                    anim.SetTrigger(triggerName);
                    break;
                }
            }
        }

        yield return new WaitForSeconds(0.3f);

        if (currentTarget != null && currentTarget.amount > 0)
        {
            float take = Mathf.Min(gatherAmount, currentTarget.amount);

            // 通知资源节点（它自己处理缩放、变树桩等）
            currentTarget.OnGathered((int)take);

            // 加到 ResourceManager
            string resName = currentTarget.resourceType.ToString().ToLower();
            ResourceManager.Instance.AddResource(resName, (int)take);

            if (currentTarget.amount <= 0)
            {
                currentTarget = null;
                unit.SetCombatRooting(false);
            }
        }

        gatherRoutineRunning = false;
    }

    void FaceToward(Vector3 worldPos)
    {
        if (spriteRenderer == null) return;
        float dx = worldPos.x - transform.position.x;
        if (dx > 0.02f) spriteRenderer.flipX = false;
        else if (dx < -0.02f) spriteRenderer.flipX = true;
    }
}

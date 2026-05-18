using UnityEngine;

/// <summary>
/// 羊 AI：威胁靠近就逃跑。Freeze() 由农民采集时调用。
/// 同时兼容动态和运动学刚体。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class SheepAI : MonoBehaviour
{
    [Header("逃跑")]
    public float fleeRange = 3f;
    public float fleeSpeed = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private UnitController unit;
    private bool caught;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        unit = GetComponent<UnitController>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.bodyType = RigidbodyType2D.Dynamic; // 确保是动态刚体，velocity 才能生效
    }

    void Update()
    {
        if (caught) return;

        Vector2 fleeDir = Vector2.zero;
        float closestDist = float.MaxValue;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, fleeRange);
        foreach (Collider2D h in hits)
        {
            if (h.gameObject == gameObject) continue;
            if (h.GetComponent<UnitGatherer>() != null || h.GetComponent<UnitCombat>() != null)
            {
                float dist = Vector2.Distance(transform.position, h.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    fleeDir = ((Vector2)transform.position - (Vector2)h.transform.position).normalized;
                }
            }
        }

        if (closestDist < fleeRange)
        {
            rb.velocity = fleeDir * fleeSpeed;
            if (sr != null) sr.flipX = fleeDir.x < 0;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    /// <summary>被农民抓住，永久冻结</summary>
    public void Freeze()
    {
        caught = true;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        if (unit != null) unit.enabled = false;
    }
}

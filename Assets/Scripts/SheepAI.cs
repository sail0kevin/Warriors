using UnityEngine;

/// <summary>
/// 羊 AI：检测到附近单位（农民/敌人）时向反方向逃跑。
/// </summary>
public class SheepAI : MonoBehaviour
{
    public float fleeRange = 3f;
    public float fleeSpeed = 2f;

    private SpriteRenderer sr;
    private Vector2 fleeDir;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        fleeDir = Vector2.zero;
    }

    void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, fleeRange);
        bool scared = false;

        foreach (Collider2D h in hits)
        {
            if (h.gameObject == gameObject) continue;
            if (h.GetComponent<UnitGatherer>() != null || h.GetComponent<UnitCombat>() != null)
            {
                // 远离威胁
                fleeDir = ((Vector2)transform.position - (Vector2)h.transform.position).normalized;
                scared = true;
                break;
            }
        }

        if (scared)
        {
            transform.Translate(fleeDir * fleeSpeed * Time.deltaTime);
            if (sr != null) sr.flipX = fleeDir.x < 0;
        }
    }
}

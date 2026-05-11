using UnityEngine;

/// <summary>移动 + Speed(0/1) + 左右翻面。参数名要和 Animator 里一致。</summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] bool topDown = true;
    [SerializeField] SpriteRenderer sprite; // 空则自动找子物体；仍无则用 scale.x 翻转
    [SerializeField] string speedParam = "Speed"; // Idle/Run 用的 Float
    [SerializeField] bool spriteFacesRight = true; // 反了改成 false

    Vector2 input;
    float scaleXAbs = 1f;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (topDown) { rb.gravityScale = 0f; rb.freezeRotation = true; }
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
        scaleXAbs = Mathf.Max(0.001f, Mathf.Abs(transform.localScale.x));

        if (rb.bodyType == RigidbodyType2D.Static)
            Debug.LogError("[PlayerMovement] Rigidbody2D 为 Static 时无法移动，请改为 Dynamic。", this);
        if ((rb.constraints & RigidbodyConstraints2D.FreezePositionX) != 0)
            Debug.LogWarning("[PlayerMovement] 勾选了 Freeze Position X 会锁死左右移动，请关掉。", this);
        if (rb.bodyType == RigidbodyType2D.Kinematic)
            Debug.LogWarning("[PlayerMovement] Kinematic 下 velocity 常不生效，已改用 MovePosition；更推荐改回 Dynamic。", this);
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = topDown ? Input.GetAxisRaw("Vertical") : 0f;
        input = input.normalized;

        if (anim) anim.SetFloat(speedParam, input.sqrMagnitude > 0f ? 1f : 0f);

        if (Mathf.Abs(input.x) < 0.01f) return;
        bool right = input.x > 0f;
        if (sprite)
            sprite.flipX = spriteFacesRight ? !right : right;
        else
        {
            var s = transform.localScale;
            float sx = (right ? 1f : -1f) * scaleXAbs;
            transform.localScale = new Vector3(spriteFacesRight ? sx : -sx, s.y, s.z);
        }
    }

    void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Static) return;
        Vector2 v = new Vector2(input.x * moveSpeed, topDown ? input.y * moveSpeed : rb.velocity.y);
        if (rb.bodyType == RigidbodyType2D.Kinematic)
            rb.MovePosition(rb.position + v * Time.fixedDeltaTime);
        else
            rb.velocity = v;
    }
}

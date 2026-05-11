using UnityEngine;

/// <summary>G 防御，1/2 攻击。参数名与 Animator 里一致即可。</summary>
public class WarriorAct : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] string guardParam = "Guard";
    [SerializeField] string attack1Param = "Attack1";
    [SerializeField] string attack2Param = "Attack2";
    [SerializeField] KeyCode guardKey = KeyCode.G;
    [SerializeField] KeyCode attack1Key = KeyCode.Alpha1;
    [SerializeField] KeyCode attack2Key = KeyCode.Alpha2;

    void Awake()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!anim) return;
        anim.SetBool(guardParam, Input.GetKey(guardKey));
        if (Input.GetKeyDown(attack1Key)) anim.SetTrigger(attack1Param);
        if (Input.GetKeyDown(attack2Key)) anim.SetTrigger(attack2Param);
    }

    void OnDisable() => anim?.SetBool(guardParam, false);
}

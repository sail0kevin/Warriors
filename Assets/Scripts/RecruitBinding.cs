using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 挂在场景中的建筑实例上，负责把场景里的面板和按钮绑定给该建筑的 RecruitSystem。
/// 此脚本不需要放在 prefab 里，只在场景级别使用。
/// </summary>
public class RecruitBinding : MonoBehaviour
{
    [Header("场景面板（在 Canvas 下）")]
    public GameObject recruitPanel;

    [Header("招募按钮列表（按顺序对应 RecruitOptions）")]
    public List<Button> recruitButtons = new List<Button>();

    void Awake()
    {
        var rs = GetComponent<RecruitSystem>();
        if (rs != null && recruitPanel != null)
        {
            rs.SetRecruitBinding(recruitPanel, recruitButtons);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 场景管理器：维护 "建筑实例 ↔ 面板" 的对应关系。
/// 新建的建筑实例会自动找最近的面板绑定。
/// 挂在场景空物体上。
/// </summary>
public class RecruitManager : MonoBehaviour
{
    public static RecruitManager Instance { get; private set; }

    [System.Serializable]
    public class RecruitBinding
    {
        [Tooltip("这个面板对应的建筑 prefab 名称（比如 Barracks）")]
        public string buildingName;
        [Tooltip("场景里的招募面板 GameObject")]
        public GameObject recruitPanel;
        [Tooltip("招募按钮列表，顺序对应 RecruitOptions")]
        public List<Button> recruitButtons = new List<Button>();
    }

    public List<RecruitBinding> bindings = new List<RecruitBinding>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>建筑实例调用，自动找到对应面板并绑定。</summary>
    public void BindRecruitPanel(RecruitSystem recruitSystem)
    {
        string buildingName = recruitSystem.gameObject.name.Replace("(Clone)", "").Trim();

        foreach (var b in bindings)
        {
            if (b.buildingName == buildingName)
            {
                recruitSystem.SetRecruitBinding(b.recruitPanel, b.recruitButtons);
                return;
            }
        }

        Debug.LogWarning($"[RecruitManager] 未找到建筑 '{buildingName}' 的招募面板绑定。");
    }
}

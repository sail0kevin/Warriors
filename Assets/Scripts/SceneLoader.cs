using UnityEngine;
using UnityEngine.SceneManagement; // 必须引入这个命名空间，才能管理场景

public class SceneLoader : MonoBehaviour
{
    // 给按钮调用的方法：加载游戏场景
    public void LoadGameScene()
    {
        // 括号里写你游戏场景的名字，也就是你的SampleScene
        SceneManager.LoadScene("SampleScene");
    }
}
using MiniGameSDK;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    void Start()
    {
        SDKManager.Instance.InitSDK((p) =>
        {
            SDKManager.GetSpecial<IDouyinSpecial>().OnInitCompletedCallback();
        });

        SDKManager.Instance.Login(code => Debug.Log(code), err => { });

        // 安全调用抖音侧边栏
        var dy = SDKManager.GetSpecial<IDouyinSpecial>();
        dy?.ShowSideBar((b) =>
        {
            Debug.Log($"侧边栏显示 {b}");
        });

        // 带防抖广告
        AdTool.ShowAdvReward((isFinished) => { Debug.Log($"奖励 isFinished{isFinished}"); }, null);
    }
}
using MiniGameSDK;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    void Start()
    {
        SDKManager.Init();

        SDKManager.API.Login(code => Debug.Log(code), err => { });

        // 安全调用抖音侧边栏
        var dy = SDKManager.GetSpecial<IDouyinSpecial>();
        dy?.ShowSideBar();

        // 带防抖广告
        AdTool.ShowRewardAd(() => { Debug.Log("奖励"); }, null, null);
    }
}
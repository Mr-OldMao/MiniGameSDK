using MiniGameSDK;
using TTSDK;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
    public Button btnInitSDK;
    public Button btnSidebar;
    public Button btnAdvReward;
    public Button btnAdvInsert;
    public Button btnAdvBanner;
    public Button btnAdvBannerHide;

    void Start()
    {
        btnAdvBannerHide.interactable = false;

        btnInitSDK.onClick.AddListener(() =>
        {
            SDKManager.Instance.InitSDK((p) =>
            {
                SDKManager.GetSpecial<IDouyinSpecial>().OnInitCompletedCallback();
                Debug.Log($"SDK初始化完成");
            });
        });

        btnAdvReward.onClick.AddListener(() =>
        {
            AdTool.ShowAdvReward((isPlayedFinish) =>
            {
                Debug.Log($"激励广告回调 isPlayedFinish:{isPlayedFinish}");
            }, null);
        });
        btnAdvInsert.onClick.AddListener(() =>
        {
            SDKManager.Instance.ShowAdvInsert(() =>
            {
                Debug.Log($"插屏广告回调");
            });
        });
        btnAdvBanner.onClick.AddListener(() =>
        {
            SDKManager.Instance.ShowAdvBanner(0, () =>
            {
                Debug.Log($"Banner广告回调");
                btnAdvBannerHide.interactable = true;
            });
        });

        btnAdvBannerHide.onClick.AddListener(() =>
        {
            SDKManager.Instance.HideAdvBanner();
            btnAdvBannerHide.interactable = false;
        });

#if SDK_DY
        btnSidebar.gameObject.SetActive(true);
        btnSidebar.onClick.AddListener(() =>
        {
            var dy = SDKManager.GetSpecial<IDouyinSpecial>();
            dy?.ShowSideBar((b) =>
            {
                Debug.Log($"显示侧边栏 {b}");
            });
        });
#else
        btnSidebar.gameObject.SetActive(false);
#endif
    }
}
using System;

namespace MiniGameSDK
{
    public interface IGameSDK
    {
        void Init();
        bool IsReady();
        string GetPlatformName();

        // 登录
        void Login(Action<string> onSuccess, Action<string> onFail);
        void GetUserInfo(Action<UserInfo> onSuccess, Action<string> onFail);
        bool IsLoggedIn();

        // 广告
        void ShowRewardAd(Action onReward, Action onClose, Action<string> onError);
        void ShowInterstitialAd(Action onClose, Action<string> onError);
        void ShowBanner(int position = 0);
        void HideBanner();

        // 分享 & 跳转
        void Share(string title, string imgUrl, Action onSuccess, Action onFail);
        void NavigateToMiniGame(string appId, string path);

        // 排行榜 & 存储
        void UploadScore(int rankId, int score, Action onSuccess, Action<string> onFail);
        void GetRankList(int rankId, Action<RankData[]> onSuccess, Action<string> onFail);
        void SetStorage(string key, string value, Action onSuccess, Action<string> onFail);
        void GetStorage(string key, Action<string> onSuccess, Action<string> onFail);

        // 系统
        void Vibrate(bool isLong = false);
        void ShowToast(string msg);
        void QuitGame();

        // 生命周期
        void OnShow();
        void OnHide();
    }
}
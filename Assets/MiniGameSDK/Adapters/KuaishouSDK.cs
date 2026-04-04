using System;
using UnityEngine;

namespace MiniGameSDK
{
#if UNITY_KUAISHOU
    public class KuaishouSDK : IGameSDK, IKuaishouSpecial
    {
        public void Init() { }
        public bool IsReady() => true;
        public string GetPlatformName() => "Kuaishou";
        public void Login(Action<string> onSuccess, Action<string> onFail) => onSuccess?.Invoke("ks_code");
        public void GetUserInfo(Action<UserInfo> onSuccess, Action<string> onFail) => onSuccess?.Invoke(new UserInfo());
        public bool IsLoggedIn() => true;
        public void ShowRewardAd(Action onReward, Action onClose, Action<string> onError) => onReward?.Invoke();
        public void ShowInterstitialAd(Action onClose, Action<string> onError) => onClose?.Invoke();
        public void ShowBanner(int position = 0) { }
        public void HideBanner() { }
        public void Share(string title, string imgUrl, Action onSuccess, Action onFail) => onSuccess?.Invoke();
        public void NavigateToMiniGame(string appId, string path) { }
        public void UploadScore(int rankId, int score, Action onSuccess, Action<string> onFail) => onSuccess?.Invoke();
        public void GetRankList(int rankId, Action<RankData[]> onSuccess, Action<string> onFail) => onSuccess?.Invoke(new RankData[0]);
        public void SetStorage(string key, string value, Action onSuccess, Action<string> onFail) => onSuccess?.Invoke();
        public void GetStorage(string key, Action<string> onSuccess, Action<string> onFail) => onSuccess?.Invoke("");
        public void Vibrate(bool isLong = false) { }
        public void ShowToast(string msg) => Debug.Log(msg);
        public void QuitGame() { }
        public void OnShow() { }
        public void OnHide() { }
        public void ShowTaskCenter() { }
    }
#endif
}
using System;
using UnityEngine;


namespace MiniGameSDK
{
#if UNITY_DOUYIN
    using TTSDK;

    public class DouyinSDK : IGameSDK, IDouyinSpecial
    {
        public void Init() { }

        public bool IsReady() => true;

        public string GetPlatformName() => "Douyin";

        public void Login(Action<string> onSuccess, Action<string> onFail)
        {
            TT.Login(res =>
            {
                if (res.IsSuccess) onSuccess?.Invoke(res.Code);
                else onFail?.Invoke(res.ErrMsg);
            });
        }

        public void GetUserInfo(Action<UserInfo> onSuccess, Action<string> onFail)
        {
            TT.GetUserInfo(res =>
            {
                if (!res.IsSuccess)
                {
                    onFail?.Invoke(res.ErrMsg);
                    return;
                }
                var info = new UserInfo
                {
                    nickName = res.Data.Nickname,
                    avatarUrl = res.Data.AvatarUrl,
                    openId = res.Data.OpenId
                };
                onSuccess?.Invoke(info);
            });
        }

        public bool IsLoggedIn() => TT.IsLogin();

        public void ShowRewardAd(Action onReward, Action onClose, Action<string> onError)
        {
            TT.ShowRewardAd(onReward, onClose, err => onError?.Invoke(err.ErrMsg));
        }

        public void ShowInterstitialAd(Action onClose, Action<string> onError)
        {
            TT.ShowInterstitialAd(onClose, err => onError?.Invoke(err.ErrMsg));
        }

        public void ShowBanner(int position = 0)
        {
            TT.ShowBanner(position == 0 ? "bottom" : "top");
        }

        public void HideBanner() => TT.HideBanner();

        public void Share(string title, string imgUrl, Action onSuccess, Action onFail)
        {
            TT.Share(new ShareParam { Title = title, ImageUrl = imgUrl },
                res => { if (res.IsSuccess) onSuccess?.Invoke(); else onFail?.Invoke(res.ErrMsg); });
        }

        public void NavigateToMiniGame(string appId, string path) => TT.NavigateToMiniProgram(appId, path);

        public void UploadScore(int rankId, int score, Action onSuccess, Action<string> onFail)
        {
            TT.SetRank(rankId.ToString(), score, res =>
            {
                if (res.IsSuccess) onSuccess?.Invoke(); else onFail?.Invoke(res.ErrMsg);
            });
        }

        public void GetRankList(int rankId, Action<RankData[]> onSuccess, Action<string> onFail)
            => onSuccess?.Invoke(new RankData[0]);

        public void SetStorage(string key, string value, Action onSuccess, Action<string> onFail)
        {
            TT.SetStorage(key, value, res =>
            {
                if (res.IsSuccess) onSuccess?.Invoke(); else onFail?.Invoke(res.ErrMsg);
            });
        }

        public void GetStorage(string key, Action<string> onSuccess, Action<string> onFail)
        {
            TT.GetStorage(key, res =>
            {
                if (res.IsSuccess) onSuccess?.Invoke(res.Data); else onFail?.Invoke(res.ErrMsg);
            });
        }

        public void Vibrate(bool isLong = false)
        {
            if (isLong) TT.VibrateLong(); else TT.VibrateShort();
        }

        public void ShowToast(string msg) => TT.ShowToast(new ToastParam { Content = msg });
        public void QuitGame() => TT.ExitGame();
        public void OnShow() { }
        public void OnHide() { }
        public void ShowSideBar() => TT.ShowSideBar();
        public void FollowCreator() => TT.FollowCreator();
    }
#endif
}
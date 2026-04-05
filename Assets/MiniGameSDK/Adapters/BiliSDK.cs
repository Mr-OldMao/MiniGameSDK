using System;
using UnityEngine;

namespace MiniGameSDK
{
#if SDK_BL
    public class BiliSDK : IGameSDK, IBiliSpecial
    {
        public void InitSDK(Action<bool> callback = null) { }
        public void Login(Action<bool> callback) { }
        public bool IsReady() => true;
        public string GetPlatformName() => "Bilibili";
        public void Login(Action<string> onSuccess, Action<string> onFail) => onSuccess?.Invoke("ali_code");
        public void GetUserInfo(Action<UserInfo> onSuccess, Action<string> onFail) => onSuccess?.Invoke(new UserInfo());
        public bool IsLoggedIn() => true;
        public void ShowAdvReward(Action<bool> onClose, Action<string> onError = null) => onClose?.Invoke(true);
        public void ShowAdvInsert(Action onClose = null, Action<string> onError = null) => onClose?.Invoke();
        public void ShowAdvBanner(int position = 0, Action onClose = null, Action<string> onError = null) { }
        public void HideAdvBanner() { }
        public void OpenLifeCodePage() { }

        public void ShowTaskCenter() { }
        public void OpenGameCenter() { }
    }
#endif
}
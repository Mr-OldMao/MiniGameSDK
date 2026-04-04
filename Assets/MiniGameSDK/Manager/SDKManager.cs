using UnityEngine;

namespace MiniGameSDK
{
    public static class SDKManager
    {
        private static IGameSDK _sdk;

        public static IGameSDK API
        {
            get
            {
                if (_sdk == null) Init();
                return _sdk;
            }
        }

        public static void Init()
        {
#if UNITY_DOUYIN
            _sdk = new DouyinSDK();
#elif UNITY_WECHAT
            _sdk = new WechatSDK();
#elif UNITY_KUAISHOU
            _sdk = new KuaishouSDK();
#elif UNITY_BILIBILI
            _sdk = new BiliSDK();
#elif UNITY_ALIPAY
            _sdk = new AlipaySDK();
#else
            _sdk = new EditorSDK();
#endif
            _sdk.Init();
        }

        /// <summary>
        /// 获取平台特有接口
        /// </summary>
        public static T GetSpecial<T>() where T : class
        {
            return _sdk as T;
        }
    }
}
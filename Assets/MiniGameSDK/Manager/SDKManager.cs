using System;
using UnityEngine;

namespace MiniGameSDK
{
    public static class SDKManager
    {
        private static IGameSDK m_Instance;

        public static IGameSDK Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    Init();
                }
                return m_Instance;
            }
        }

        private static void Init()
        {
#if UNITY_DOUYIN
            m_Instance = new DouyinSDK();
#elif UNITY_WECHAT
            m_Instance = new WechatSDK();
#elif UNITY_KUAISHOU
            m_Instance = new KuaishouSDK();
#elif UNITY_BILIBILI
            m_Instance = new BiliSDK();
#elif UNITY_ALIPAY
            m_Instance = new AlipaySDK();
#else
            m_Instance = new EditorSDK();
#endif
            //m_Instance.Init();
        }

        /// <summary>
        /// 获取平台特有接口
        /// </summary>
        public static T GetSpecial<T>() where T : class
        {
            return m_Instance as T;
        }
    }
}
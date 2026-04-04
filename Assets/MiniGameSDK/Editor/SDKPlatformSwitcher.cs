using UnityEditor;
using UnityEngine;

namespace MiniGameSDK.Editor
{
    public static class SDKPlatformSwitcher
    {
        // 各平台宏定义
        private const string DOUYIN = "UNITY_DOUYIN;BYTEGAME_MINIGAME";
        private const string WECHAT = "UNITY_WECHAT";
        private const string KUAISHOU = "UNITY_KUAISHOU";
        private const string BILIBILI = "UNITY_BILIBILI";
        private const string ALIPAY = "UNITY_ALIPAY";

        // ====================== 一键切换平台 ======================
        [MenuItem("小游戏/切换到抖音", false, 1)]
        public static void ToDouyin() => Set(DOUYIN, "抖音");

        [MenuItem("小游戏/切换到微信", false, 2)]
        public static void ToWechat() => Set(WECHAT, "微信");

        [MenuItem("小游戏/切换到快手", false, 3)]
        public static void ToKuaishou() => Set(KUAISHOU, "快手");

        [MenuItem("小游戏/切换到B站", false, 4)]
        public static void ToBilibili() => Set(BILIBILI, "B站");

        [MenuItem("小游戏/切换到支付宝", false, 5)]
        public static void ToAlipay() => Set(ALIPAY, "支付宝");

        [MenuItem("小游戏/清空所有宏", false, 10)]
        public static void Clear() => Set("", "已清空所有宏");

        // ====================== 【新增】打印当前SDK平台 ======================
        [MenuItem("小游戏/查看当前SDK平台", false, 11)]
        public static void PrintCurrentPlatform()
        {
            var group = BuildTargetGroup.WebGL;
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            string platform = "未指定任何平台（编辑器模式）";

            // 判断当前宏属于哪个平台
            if (currentDefines.Contains("UNITY_DOUYIN")) platform = "抖音小游戏";
            else if (currentDefines.Contains("UNITY_WECHAT")) platform = "微信小游戏";
            else if (currentDefines.Contains("UNITY_KUAISHOU")) platform = "快手小游戏";
            else if (currentDefines.Contains("UNITY_BILIBILI")) platform = "B站小游戏";
            else if (currentDefines.Contains("UNITY_ALIPAY")) platform = "支付宝小游戏";

            // 弹窗 + 控制台输出
            EditorUtility.DisplayDialog("当前SDK平台", platform, "确定");
            Debug.Log("<color=cyan>[当前平台]</color> " + platform);
            Debug.Log("<color=gray>[宏定义]</color> " + currentDefines);
        }

        /// <summary>
        /// 设置宏并提示
        /// </summary>
        private static void Set(string defines, string title)
        {
            var group = BuildTargetGroup.WebGL;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
            EditorUtility.DisplayDialog("平台切换", title, "OK");
            Debug.Log($"<color=cyan>[平台切换]</color> 已切换至 → {title}");
        }
    }
}
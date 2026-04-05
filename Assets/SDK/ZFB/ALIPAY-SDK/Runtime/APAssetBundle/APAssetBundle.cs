using System.Collections.Generic;
using AlipaySdk.Bridge;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace AlipaySdk
{
        public class APAssetBundle
        {
                public static Dictionary<AssetBundle, string> bundle2path = new Dictionary<AssetBundle, string>();

                public static bool isAbfsReady = CheckSwitch();

                public static bool initKey;
                public static bool enableAPAssetbundle;
                private static bool _reportedUsage = false;

                private static bool CheckSwitch()
                {
                        if (initKey)
                        {
                                return enableAPAssetbundle;
                        }
                        try
                        {
                                enableAPAssetbundle = false;
                                var jsonData = new JsonData { ["configKey"] = "paladin_enable_apassetbundle" };
                                var value = new JsonData
                                {
                                        new JsonData("configService.getConfig"),
                                        jsonData
                                };
                                var switchInfo = AlipaySDK.API.GetSwitch(value);
                                if (!switchInfo.Contains("configKey"))
                                {
                                        return false;
                                }
                                JsonData switchJson = JsonMapper.ToObject(switchInfo);
                                initKey = true;
                                enableAPAssetbundle = switchJson["configKey"].ToString() == "true";
                                AlipaySDK.API.LocalLog(LogType.Log, $"[APAssetBundle] Init APAssetBundle Switch: {enableAPAssetbundle}");
                                return enableAPAssetbundle ? CheckReady() : false;
                        }
                        catch (System.Exception ex)
                        {
                                Debug.LogError($"Exception in CheckSwitch: {ex.Message}");
                                return false;
                        }
                }

#if UNITY_WEBGL && !UNITY_EDITOR
                [method: Preserve]
                [DllImport("__Internal", EntryPoint = "APFSInit")]
                private static extern void APFSInit(int ttl, int capacity);
#else
                public static void APFSInit(int ttl, int capacity)
                {
                }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
                 [method: Preserve]
                [DllImport("__Internal", EntryPoint = "APCheckReady")]
                public static extern bool CheckReady();
#else
                public static bool CheckReady()
                {
                        return false;
                }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
                [method: Preserve]
                [DllImport("__Internal", EntryPoint = "APRegisterAssetBundleUrl")]
                public static extern void RegisterAssetBundleUrl(string path);
#else
                public static void RegisterAssetBundleUrl(string path)
                {
                }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
                [method: Preserve]
                [DllImport("__Internal", EntryPoint = "APUnregisterAssetBundleUrl")]
                public static extern void UnregisterAssetBundleUrl(string path);
#else
                public static void UnregisterAssetBundleUrl(string path)
                {
                }
#endif
                public static UnityWebRequest GetAssetBundle(string uri) => GetAssetBundle(uri, 0U);
                public static UnityWebRequest GetAssetBundle(string uri, uint crc)
                {
                        ReportUsageOnce();
                        if (isAbfsReady)
                        {
                                RegisterAssetBundleUrl(uri);
                        }

                        return new UnityWebRequest(uri, "GET", new DownloadHandlerAPAssetBundle(uri, crc), null);
                }

                private static void ReportUsageOnce()
                {
                        if (_reportedUsage) return;
                        _reportedUsage = true;
                        JsonData param = new JsonData();
                        param["type"] = "used_apassetbundle";
                        string jsonString = JsonMapper.ToJson(param);
                        AlipayWebGLInterface.SendMonitor(1, jsonString);
                }
        }
}
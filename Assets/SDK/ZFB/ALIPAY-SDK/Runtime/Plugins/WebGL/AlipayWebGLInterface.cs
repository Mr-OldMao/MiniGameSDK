using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using UnityEngine;

[assembly: Preserve]
namespace AlipaySdk.Bridge
{
    public class AlipayWebGLInterface
    {
#if UNITY_WEBPLAYER || UNITY_WEBGL
        // 以下接口为 Web 使用，用于调用 JS 代码
        [method: Preserve]
        [DllImport("__Internal", EntryPoint = "unityCallJs")]
        public static extern void unityCallJs(string eventId, string paramJson);

        [DllImport("__Internal", EntryPoint = "unityCallJsSync")]
        public static extern string unityCallJsSync(string eventId, string paramJson);

        [DllImport("__Internal", EntryPoint = "unityCallJsSyncSafe")]
        public static extern string unityCallJsSyncSafe(string eventId, string paramJson);

        [DllImport("__Internal", EntryPoint = "GetAlipayEnv")]
        public static extern string GetAlipayEnv();

        [DllImport("__Internal", EntryPoint = "AlipayIsIDE")]
        public static extern bool IsIDE();
        [DllImport("__Internal", EntryPoint = "AlipaySDKVersion")]
        public static extern string SDKVersion();

        [DllImport("__Internal", EntryPoint = "AlipayWriteBinFileSync")]
        public static extern string WriteBinFileSync(string fileNamePtr, byte[] dataPtr, int dataLength);

        [DllImport("__Internal", EntryPoint = "AlipayReadBinFileSync")]
        public static extern IntPtr ReadBinFileSync(string fileNamePtr, int positionPtr, int lengthPtr);

        [DllImport("__Internal", EntryPoint = "AlipayWriteFileSync")]
        public static extern string WriteFileSync(string fileName, string data, string encoding);

        [DllImport("__Internal", EntryPoint = "AlipayReadFileSync")]
        public static extern string ReadFileSync(string fileName, string encoding, int positionPtr, int lengthPtr);

        [DllImport("__Internal", EntryPoint = "AlipayWriteBinFile")]
        public static extern string WriteBinFile(string fileNamePtr, byte[] dataPtr, int dataLength, string callbackIDPtr);

        [DllImport("__Internal", EntryPoint = "AlipayReadBinFile")]
        public static extern void ReadBinFile(string fileNamePtr, string callbackIDPtr, int positionPtr, int lengthPtr);

        [DllImport("__Internal", EntryPoint = "AlipayFree")]
        public static extern void AlipayFree(IntPtr ptr);

        [DllImport("__Internal", EntryPoint = "GetFSStatsSync")]
        public static extern string GetFSStatsSync(string path, bool recursive);

        [DllImport("__Internal", EntryPoint = "GetUserAvatarData")]
        public static extern void GetUserAvatarData(string url, IntPtr successCallback, IntPtr errorCallback);

        [DllImport("__Internal", EntryPoint = "AlipayCanIUse")]
        public static extern bool CanIUse(string schema);

        [DllImport("__Internal", EntryPoint = "OnStartFeedback")]
        public static extern void OnStartFeedback(string info);

        [DllImport("__Internal", EntryPoint = "TriggerGC")]
        public static extern void TriggerGC();

        [DllImport("__Internal", EntryPoint = "SendMonitor")]
        public static extern void SendMonitor(int type, string jsonString);

        [DllImport("__Internal", EntryPoint = "SetDevicePixelRatio")]
        public static extern bool SetDevicePixelRatio(float scale);

#else
        public static void unityCallJs(string eventId, string paramJson)
        {
            Debug.LogError("message dropped, please check platform");
        } 

        public static string unityCallJsSync(string eventId, string paramJson)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 

        public static string unityCallJsSyncSafe(string eventId, string paramJson)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 

        public static string GetAlipayEnv()
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 

        public static string WriteBinFileSync(string fileNamePtr, byte[] dataPtr, int dataLength)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 

        public static IntPtr ReadBinFileSync(string fileNamePtr, int positionPtr, int lengthPtr)
        {
            Debug.LogError("message dropped, please check platform");
            return IntPtr.Zero;
        } 

        public static string SDKVersion()
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 

        public static bool IsIDE()
        {
            Debug.LogError("message dropped, please check platform");
            return false;
        }

        public static string WriteFileSync(string fileName, string data, string encoding)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 

        public static string ReadFileSync(string fileName, string encoding, int positionPtr, int lengthPtr)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        }
  
        public static string WriteBinFile(string fileNamePtr, byte[] dataPtr, int dataLength, string callbackID)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 

        public static void AlipayFree(IntPtr ptr)
        {
            Debug.LogError("message dropped, please check platform");
        } 

        public static void ReadBinFile(string fileNamePtr, string callbackID, int positionPtr, int lengthPtr)
        {
            Debug.LogError("message dropped, please check platform");
        } 

        public static string GetFSStatsSync(string path, bool recursive)
        {
            Debug.LogError("message dropped, please check platform");
            return string.Empty;
        } 
        
        public static void GetUserAvatarData(string url, IntPtr successCallback, IntPtr errorCallback)
        {
            Debug.LogError("message dropped, please check platform");
        }
        
        public static bool CanIUse(string schema)
        {
            Debug.LogError("message dropped, please check platform");
            return false;
        }

        public static void OnStartFeedback(string info)
        {
            Debug.LogError("message dropped, please check platform");
        }

        public static void TriggerGC()
        {
            Debug.LogError("message dropped, please check platform");
        }

        public static void SendMonitor(int type, string jsonString)
        {
            Debug.LogError("message dropped, please check platform");
        }

        public static bool SetDevicePixelRatio(float scale)
        {
            Debug.LogError("message dropped, please check platform");
            return false;
        }
#endif
    }
}

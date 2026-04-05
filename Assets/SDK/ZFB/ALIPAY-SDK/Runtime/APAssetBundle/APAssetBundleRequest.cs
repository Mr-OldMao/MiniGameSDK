using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

namespace AlipaySdk
{
    public class APAssetBundleRequest : AsyncOperation, IEnumerator
    {
        private static readonly Dictionary<string, APAssetBundleRequest> _urlToRequest =
            new Dictionary<string, APAssetBundleRequest>();
        public string Url;
        private uint _crc;
        private readonly ulong _offset;
        public bool IsDone;
        private AssetBundle _bundle;
        private string _requestId;
        public delegate void APAssetBundleCallback(IntPtr idPtr, uint errCode, IntPtr msgPtr);

        public void Dispose()
        {
        }

        public AssetBundle assetBundle
        {
            get
            {
                if (_bundle == null)
                {
                    _bundle = AssetBundle.LoadFromFile(Url);
                    APAssetBundle.bundle2path.Add(_bundle, Url);
                }
                return _bundle;
            }
        }

        internal APAssetBundleRequest(string url, uint crc, ulong offset)
        {
            Url = url;
            _crc = crc;
            _offset = offset;
            IsDone = false;
            _requestId = url + Random.Range(0.0f, 10000000f);
            _urlToRequest.Add(_requestId, this);
            AlipayAbfsFetchBundleFromXHR(url, _requestId, new APAssetBundleCallback(Callback));
        }

        [MonoPInvokeCallback(typeof(APAssetBundleCallback))]
        public static void Callback(IntPtr idPtr, uint errCode, IntPtr msgPtr)
        {
            string requestId = Marshal.PtrToStringAuto(idPtr);
            _urlToRequest.TryGetValue(requestId, out var request);
            _urlToRequest.Remove(requestId);
            if (errCode == 0U)
            {
                request.IsDone = true;
            }
            else
            {
                string msg = Marshal.PtrToStringAuto(msgPtr);
                Debug.LogError($"AlipayAbfsFetchBundleFromXHR{requestId} Error: {msg}");
                throw new Exception(msg);
            }
        }

        [Preserve]
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal", EntryPoint = "AlipayAbfsFetchBundleFromXHR")]
        private static extern void AlipayAbfsFetchBundleFromXHR(string url, string id, APAssetBundleCallback callback);
#else
        private static void AlipayAbfsFetchBundleFromXHR(string url, string id, APAssetBundleCallback callback)
        {
            throw new PlatformNotSupportedException();
        }
#endif

        public object Current => null;
        public bool MoveNext() => !IsDone;
        public void Reset() => throw new NotImplementedException("Reset Not Implemented");
    }
}
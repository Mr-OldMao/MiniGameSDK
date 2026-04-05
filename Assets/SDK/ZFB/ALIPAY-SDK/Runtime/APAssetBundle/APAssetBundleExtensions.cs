using UnityEngine;

namespace AlipaySdk
{
    public static class APAssetBundleExtensions
    {
        public static void APUnload(this AssetBundle ab, bool unloadAllLoadedObjects)
        {
            ab.Unload(unloadAllLoadedObjects);

            if (!APAssetBundle.isAbfsReady)
                return;

            if (APAssetBundle.bundle2path.TryGetValue(ab, out var path))
            {
                APAssetBundle.UnregisterAssetBundleUrl(path);
                APAssetBundle.bundle2path.Remove(ab);
            }
            else
            {
                Debug.LogError("AssetBundle.Unload() Failure: Unregistered Asset Bundle. Loaded without APAssetBundle methods?");
            }
        }
    }
}
using UnityEngine;
using System;

#if UNITY_EDITOR
#if ALIPAY_ADDRESSABLES_CHECKER
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif
using System.IO;
using UnityEditor;
using UnityEditorInternal;
#endif

public class AlipayAddressablesConfigChecker
{
#if UNITY_EDITOR
    public static bool CheckAddressableSupport()
    {
#if UNITY_2021_2_OR_NEWER
        bool addressablesInstalled = IsAddressablesPackageInstalled();
        string definedSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

        if (addressablesInstalled && !definedSymbols.Contains("ALIPAY_ADDRESSABLES_CHECKER"))
        {
            UpdateAssemblyReferences(true);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, definedSymbols + ";ALIPAY_ADDRESSABLES_CHECKER");
        }
        else if (!addressablesInstalled && definedSymbols.Contains("ALIPAY_ADDRESSABLES_CHECKER"))
        {
            UpdateAssemblyReferences(false);
            definedSymbols = definedSymbols.Replace("ALIPAY_ADDRESSABLES_CHECKER;", "").Replace("ALIPAY_ADDRESSABLES_CHECKER", "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, definedSymbols);
        }
        return addressablesInstalled;
#else
   return false;
#endif
    }

#if UNITY_2021_2_OR_NEWER
    public static void UpdateAssemblyReferences(bool add)
    {
        const string assemblyPath = "Assets/ALIPAY-SDK/Editor/AlipayBuildTool.asmdef";
        const string referenceToAdd = "Unity.Addressables.Editor";

        var assemblyAsset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assemblyPath);
        if (assemblyAsset == null)
        {
            Debug.LogError($"Assembly not found: {assemblyPath}");
            return;
        }

        var jsonText = ((TextAsset)assemblyAsset).text;
        var assemblyData = JsonUtility.FromJson<AssemblyDefinitionData>(jsonText);

        // 强制设置仅Editor平台
        assemblyData.includePlatforms = new[] { "Editor" };
        assemblyData.excludePlatforms = null; // 确保空数组序列化正确

        // 处理引用操作
        if (add)
        {
            if (Array.Exists(assemblyData.references, r => r == referenceToAdd))
            {
                Debug.Log("Reference already exists");
                return;
            }
            Array.Resize(ref assemblyData.references, assemblyData.references.Length + 1);
            assemblyData.references[^1] = referenceToAdd;
        }
        else
        {
            assemblyData.references = Array.FindAll(assemblyData.references, r => r != referenceToAdd);
        }

        // 保留原始结构数据
        var updatedJson = JsonUtility.ToJson(assemblyData, true);
        File.WriteAllText(assemblyPath, updatedJson);

        AssetDatabase.ImportAsset(assemblyPath); // 强制重新导入‌:ml-citation{ref="1,3" data="citationList"}
        EditorApplication.delayCall += () => AssetDatabase.Refresh(); // 延迟刷新避免锁定‌:ml-citation{ref="3" data="citationList"}
    }

    [Serializable]
    private class AssemblyDefinitionData
    {
        public string name;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
    }

    private static bool IsAddressablesPackageInstalled()
    {
        string packageName = "com.unity.addressables";
        foreach (var package in UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages())
        {
            if (package.name == packageName)
            {
                return true;
            }
        }
        return false;
    }
#endif

#if ALIPAY_ADDRESSABLES_CHECKER

    public static bool CheckAddressableConfig()
    {
        if (CheckAddressableAssetGroupSchema())
        {
            EditorUtility.DisplayDialog("Addressables 配置检查警告", "当前工程有BundledAssetGroupSchema配置项开启了ab包缓存，请检查并取消勾选该选项！！！", "确定");
            return false;
        }
        return true;
    }

    public static bool CheckAddressableAssetGroupSchema()
    {
        string[] guids = AssetDatabase.FindAssets("t:BundledAssetGroupSchema");

        if (guids.Length == 0)
        {
            Debug.LogWarning("项目中未找到任何 AddressableAssetGroupSchema。");
            return false;
        }

        bool anyEnabled = false;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            BundledAssetGroupSchema schema = AssetDatabase.LoadAssetAtPath<BundledAssetGroupSchema>(assetPath);

            if (schema != null)
            {
                if (schema.UseAssetBundleCache)
                {
                    Selection.activeObject = schema;
                    anyEnabled = true;
                    break;
                }
            }
            else
            {
                Debug.LogWarning($"未能加载路径 {assetPath} 的 BundledAssetGroupSchema。可能是缺失或已损坏。");
            }
        }

        return anyEnabled;
    }

#endif
#endif

}
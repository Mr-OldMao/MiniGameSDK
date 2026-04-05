using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using LitJson;
using UnityEditor;
using UnityEngine;

public class AlipaySDKUpdate
{
    private static string sdkPath = "ALIPAY-SDK";
    private static string packageJson = "package.json";
    private static string packageJsonURL = "https://mdn.alipayobjects.com/paladin_unity/uri/file/as/AlipaySDKPackageJson";
    private static bool isCheck
    {
        get { return EditorPrefs.GetBool("AlipaySDKCheck", false); }
        set { EditorPrefs.SetBool("AlipaySDKCheck", value); }
    }
    private static string sdkDownloadURL = string.Empty;
    private static string sdkVersion = string.Empty;
    private static string sdkUpdateLogURL = "https://opendocs.alipay.com/mini-game/0ft9zz?pathHash=ebd5cda7";
    [MenuItem("支付宝小游戏 / 插件版本", false, 100)]
    public static void AlipaySDKUpdateCheckMenu()
    {
        string packageJsonPath = Path.Combine(Application.dataPath, sdkPath, packageJson);
        if (!File.Exists(packageJsonPath))
        {
            Debug.LogError("未找到本地包JSON文件。请检查本地配置文件路径是否正确。");
            return;
        }
        string localJson = File.ReadAllText(packageJsonPath);
        sdkVersion = ParseVersionFromJson(localJson);
        bool updateWindow = EditorUtility.DisplayDialog(
            "Alipay SDK 插件版本",
            "当前版本：" + sdkVersion,
            "更新日志",
            "更新检查"
        );
        if (updateWindow)
        {
            Application.OpenURL(sdkUpdateLogURL);
        }
        else
        {
            ForceCheckSDKUpdate();
        }
    }
    public static void ForceCheckSDKUpdate()
    {
        isCheck = false;
        CheckSDKUpdate();
    }
    /// <summary>
    /// 检查SDK更新
    /// </summary>
    private static async void CheckSDKUpdate()
    {
        if (isCheck)
        {
            return;
        }

        var needUpdate = await CheckSDKUpdateAsync();
        if (needUpdate)
        {
            // 弹出选择弹窗
            bool updateConfirmed = EditorUtility.DisplayDialog(
                "SDK更新可用",
                "有新的SDK版本可用。是否更新到最新版本？",
                "是",
                "否"
            );
            if (updateConfirmed)
            {
                if (!string.IsNullOrEmpty(sdkDownloadURL))
                {
                    Application.OpenURL(sdkDownloadURL);
                }
                else
                {
                    Debug.LogError("未找到SDK下载链接。请联系开发者");
                }
            }
        }
    }
    private static async Task<bool> CheckSDKUpdateAsync()
    {
        string remoteJson = await FetchRemoteJsonAsync();

        if (string.IsNullOrEmpty(remoteJson))
        {
            return false;
        }

        string remoteVersion = ParseVersionFromJson(remoteJson);
        string packageJsonPath = Path.Combine(Application.dataPath, sdkPath, packageJson);

        if (!File.Exists(packageJsonPath))
        {
            Debug.LogError("未找到本地包JSON文件。请检查本地配置文件路径是否正确。");
            return false;
        }

        string localJson = File.ReadAllText(packageJsonPath);
        string localVersion = ParseVersionFromJson(localJson);


        if (remoteVersion != null && localVersion != null)
        {
            isCheck = true;
            if (remoteVersion != localVersion)
            {
                Debug.Log($"新的AlipaySDK版本可用：{remoteVersion}。当前版本：{localVersion}。SDK下载链接：{sdkDownloadURL}");
                return true;
            }
            else
            {
                Debug.Log("当前SDK已经是最新的。");
                return false;
            }
        }
        return false;
    }
    private static async Task<string> FetchRemoteJsonAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                return await client.GetStringAsync(packageJsonURL);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"获取更新配置文件失败：{e.Message}");
                return null;
            }
        }
    }
    private static string ParseVersionFromJson(string json)
    {
        try
        {
            JsonData jsonData = JsonMapper.ToObject(json);
            if (jsonData == null || !jsonData.ContainsKey("version"))
            {
                Debug.LogError("JSON格式错误，未找到版本号。");
                return null;
            }
            if (jsonData.ContainsKey("sdkDownloadURL"))
            {
                sdkDownloadURL = jsonData["sdkDownloadURL"].ToString();
            }
            return jsonData["version"].ToString();
        }
        catch
        {
            Debug.LogError("解析JSON失败。");
            return null;
        }
    }
}
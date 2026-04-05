using UnityEditor;
using UnityEngine;

public class AlipayEditorWindow : EditorWindow
{
    private static AlipayEditorWindow sInstance = null;
    public static AlipayEditorWindow Instance { get { return sInstance; } }
    private static AlipayBuildConfig alipayBuildConfig;
    private Vector2 scrollRoot;

    public AlipayEditorWindow()
    {
        sInstance = this;
    }

    [MenuItem("支付宝小游戏 / WebGL打包工具", false, 1)]
    public static void OpenAlipayEditor()
    {
        var window = GetWindow(typeof(AlipayEditorWindow), false, "支付宝WebGL打包工具");
        Vector2 windowSize = new Vector2(550, 700);
        window.position = new Rect(100, 100, 600, 700);
        window.Show();
        AlipaySDKUpdate.ForceCheckSDKUpdate();
    }

    public void OnEnable()
    {
        sInstance = this;
        alipayBuildConfig = AlipayUtil.GetAlipayBuildConfig();
        AlipayAddressablesConfigChecker.CheckAddressableSupport();
    }

    public static AlipayBuildConfig GetEditorConfig()
    {
        return alipayBuildConfig;
    }

    public void OnGUI()
    {
        scrollRoot = EditorGUILayout.BeginScrollView(scrollRoot);
        {
            GUILayout.Space(ToolInfo.spaceHeight);
            BaseOption.RenderGUI();
            GUILayout.Space(ToolInfo.spaceHeight);

            BuildOption.RenderGUI();
            GUILayout.Space(ToolInfo.spaceHeight);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("生成并转换"), GUILayout.Width(100), GUILayout.Height(25)))
                {
                    if (AlipayConvertCore.WebglBuildAndConvert())
                    {
                        ShowNotification(new GUIContent("转换完成"));
                    }
                    GUIUtility.ExitGUI();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

}
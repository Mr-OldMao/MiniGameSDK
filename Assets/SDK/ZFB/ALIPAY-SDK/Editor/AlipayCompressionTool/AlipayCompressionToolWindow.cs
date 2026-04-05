using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AlipaySdk.Editor.CompressionTool
{
    public class AlipayCompressionToolWindow : EditorWindow
    {
        private static AlipayCompressionToolWindow _instance = null;
        internal static AlipayCompressionToolWindow instance
        {
            get
            {
                if (_instance == null)
                    _instance = GetWindow<AlipayCompressionToolWindow>();
                return _instance;
            }
        }
        private AlipayBuildConfig alipayConfig => AlipayUtil.GetAlipayBuildConfig();

        public static string inputFolder = string.Empty;
        public static string outputFolder = string.Empty;
        public static bool debug = false;
        public static bool force = true;
        public static bool isProcessing = false;

        #region TipContent
        private static string windowTitle = "支付宝纹理压缩工具";
        private static string errorTitle = "错误";
        private static string bundlePathEmpty = "Bundle路径不能为空！";
        private static string exportPathEmpty = "导出路径不能为空！";
        private static string customBundlePathLabel = "自定义Bundle路径：";
        private static string customBundlePathButton = "选择路径（默认不用选）";
        private static string resourceStoragePathLabel = "资源处理后存放路径：";
        private static string resourceStoragePathButton = "选择路径（默认不用选）";
        private static string bundleConfigDetailsLabel = "Bundle配置详情：";
        //private static string openBundleConfigPanelButton = "打开Bundle配置面板";
        private static string compressionProgressTitle = "资源压缩进度";
        private static string compressionCompleteNotification = "支付宝纹理压缩处理完成！";
        private static string compressionErrorNotification = "支付宝纹理压缩，资源处理错误：";
        private static string processResourcesButton = "处理资源";

        private enum DebugMode { DebugModeTrue, DebugModeFalse }
        private static DebugMode selectedDebugMode = DebugMode.DebugModeTrue;
        private static string debugModeLabel = "纹理转换处理模式：";
        private static string debugModeTrue = "快速模式： 仅生成ASTC、PNG格式图片（调试使用，IDE无法正常展示）";
        private static string debugModeFalse = "全量模式： 生成ASTC、DXT5（for PC）、ETC2、PNG-min，4种格式。（不支持MacOS）";

        private enum ForceMode { ForceModeTrue, ForceModeFalse }
        private static ForceMode selectedForceMode = ForceMode.ForceModeFalse;
        private static string forceModeLabel = "资源处理模式：";
        private static string forceModeTrue = "资源强制全量处理：强制重新生成所有资源（若资源异常，请勾选该选项强制重新生成！） ";
        private static string forceModeFalse = "资源复用处理：仅处理未处理过或已发生变更的资源（目标文件夹下请手动放置已处理过的资源！）";

        #endregion

        [MenuItem("支付宝小游戏 / 支付宝纹理压缩工具", false, 50)]
        public static void ShowWindow()
        {
            _instance = null;
            instance.titleContent = new GUIContent(windowTitle);
            _instance.minSize = new Vector2(700, 300);
            Vector2 windowSize = _instance.minSize;
            Vector2 screenCenter = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) / 2;
            Rect position = new Rect(screenCenter.x - windowSize.x / 2,
                                     screenCenter.y - windowSize.y / 2,
                                     windowSize.x,
                                     windowSize.y);
            _instance.position = position;
            instance.Show();
        }

        void OnEnable()
        {
            inputFolder = string.IsNullOrEmpty(alipayConfig.AlipayProjectCfg.DerivedPath) ? string.Empty : Path.Combine(alipayConfig.AlipayProjectCfg.DerivedPath, "webgl", "StreamingAssets");
            outputFolder = alipayConfig.AlipayProjectCfg.DerivedPath;
        }

        private void OnGUI()
        {
            GUI.enabled = !isProcessing;
            CompressConfigGUI();
        }

        public static void CompressConfigGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(bundleConfigDetailsLabel, ToolInfo.LabelStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("frameBox");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label(customBundlePathLabel, GUILayout.Width(130));
                    GUILayout.Label(inputFolder, EditorStyles.textField, GUILayout.MinWidth(300));

                    if (GUILayout.Button(customBundlePathButton, GUILayout.Width(150)))
                    {
                        var tempPath = EditorUtility.SaveFolderPanel(customBundlePathButton, "", Application.dataPath);
                        if (!string.IsNullOrEmpty(tempPath))
                        {
                            inputFolder = tempPath;
                            GUIUtility.ExitGUI();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(ToolInfo.groupSpaceHeight);

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label(resourceStoragePathLabel, GUILayout.Width(130));
                    GUILayout.Label(outputFolder, EditorStyles.textField, GUILayout.MinWidth(300));
                    if (GUILayout.Button(resourceStoragePathButton, GUILayout.Width(150)))
                    {
                        var tempPath = EditorUtility.SaveFolderPanel(resourceStoragePathButton, "", Application.dataPath);
                        if (!string.IsNullOrEmpty(tempPath))
                        {
                            outputFolder = tempPath;
                            GUIUtility.ExitGUI();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(ToolInfo.groupSpaceHeight);

                // GUILayout.Label(bundleConfigDetailsLabel);
                // if (GUILayout.Button(openBundleConfigPanelButton, GUILayout.Width(150), GUILayout.Height(30)))
                // {
                //     AlipayCompressionConfigWindow.instance.ShowWindow(inputFolder, outputFolder);
                //     GUIUtility.ExitGUI();
                // }
                // GUILayout.Space(ToolInfo.groupSpaceHeight);

                GUILayout.Label(debugModeLabel, EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical();
                {
                    selectedDebugMode = GUILayout.Toggle(selectedDebugMode == DebugMode.DebugModeTrue, debugModeTrue, "Radio") ? DebugMode.DebugModeTrue : selectedDebugMode;
                    selectedDebugMode = GUILayout.Toggle(selectedDebugMode == DebugMode.DebugModeFalse, debugModeFalse, "Radio") ? DebugMode.DebugModeFalse : selectedDebugMode;
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(ToolInfo.groupSpaceHeight);

                GUILayout.Label(forceModeLabel, EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical();
                {
                    selectedForceMode = GUILayout.Toggle(selectedForceMode == ForceMode.ForceModeTrue, forceModeTrue, "Radio") ? ForceMode.ForceModeTrue : selectedForceMode;
                    selectedForceMode = GUILayout.Toggle(selectedForceMode == ForceMode.ForceModeFalse, forceModeFalse, "Radio") ? ForceMode.ForceModeFalse : selectedForceMode;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(ToolInfo.groupSpaceHeight);

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(processResourcesButton, GUILayout.MaxWidth(150f), GUILayout.MaxHeight(40f)))
                {
                    if (string.IsNullOrEmpty(inputFolder))
                    {
                        EditorUtility.DisplayDialog(errorTitle, bundlePathEmpty, "确定");
                        GUIUtility.ExitGUI();
                        GUILayout.EndHorizontal();
                        return;
                    }

                    if (string.IsNullOrEmpty(outputFolder))
                    {
                        EditorUtility.DisplayDialog(errorTitle, exportPathEmpty, "确定");
                        GUIUtility.ExitGUI();
                        GUILayout.EndHorizontal();
                        return;
                    }
                    isProcessing = true;
                    Progress<ProgressReport> progress = new Progress<ProgressReport>(report =>
                    {
                        EditorUtility.DisplayProgressBar(compressionProgressTitle, report.CurrentTask,
                            (float)report.Percentage / 100);
                    });
                    AlipayAssetCompressionCore.CompressTextAsync((complete, msg) =>
                    {
                        EditorUtility.ClearProgressBar();
                        if (complete)
                        {
                            Debug.Log(msg);
                            instance.ShowNotification(new GUIContent(compressionCompleteNotification));
                        }
                        else
                        {
                            Debug.LogError($"{compressionErrorNotification}{msg}");
                        }

                        isProcessing = false;
                        instance.Repaint();
                    }, inputFolder, outputFolder, selectedDebugMode == DebugMode.DebugModeTrue, selectedForceMode == ForceMode.ForceModeTrue, progress);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}

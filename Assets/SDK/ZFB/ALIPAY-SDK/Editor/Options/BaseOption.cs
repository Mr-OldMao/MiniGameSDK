using System;
using UnityEditor;
using UnityEngine;

public class BaseOption
{
    private static AlipayBuildConfig config => AlipayEditorWindow.GetEditorConfig();
    private static GUIContent useDataZipContent = new GUIContent("压缩首包资源：", "会将首包中的data文件打包为Zip");
    private static GUIContent useDataBrContent = new GUIContent("   ", "会将首包中的data文件压缩为br格式");

    public static void RenderGUI()
    {
        EditorGUI.BeginChangeCheck();
        BaseInfoGUI();
        LoadingInfoGUI();
        if (EditorGUI.EndChangeCheck())
        {
            SaveConfig();
        }
    }

    private static void BaseInfoGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("基础信息", ToolInfo.LabelStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical("frameBox");
        {
            GUILayout.Space(ToolInfo.groupSpaceHeight);
            config.AlipayProjectCfg.AppID = EditorGUILayout.TextField("AppID：", config.AlipayProjectCfg.AppID).Trim();

            GUILayout.Space(ToolInfo.groupSpaceHeight);
            config.AlipayProjectCfg.ProjectName = EditorGUILayout.TextField("项目名：", config.AlipayProjectCfg.ProjectName).Trim();

            GUILayout.Space(ToolInfo.groupSpaceHeight);
            config.AlipayProjectCfg.Orientation = (AlipayScreenOrientation)EditorGUILayout.EnumPopup("游戏方向", config.AlipayProjectCfg.Orientation);

            GUILayout.Space(ToolInfo.groupSpaceHeight);
            EditorGUILayout.BeginHorizontal();
            {
                config.AlipayProjectCfg.DerivedPath = EditorGUILayout.TextField("项目导出路径：", config.AlipayProjectCfg.DerivedPath).Trim();
                if (GUILayout.Button("打开", GUILayout.Width(40), GUILayout.Height(20)))
                {
                    AlipayUtil.OpenFolder(config.AlipayProjectCfg.DerivedPath);
                    GUIUtility.ExitGUI();
                }
                if (GUILayout.Button("选择", GUILayout.Width(40), GUILayout.Height(20)))
                {
                    var tempPath = EditorUtility.SaveFolderPanel("选择导出路径", config.AlipayProjectCfg.DerivedPath, Application.dataPath);
                    if (!string.IsNullOrEmpty(tempPath))
                    {
                        config.AlipayProjectCfg.DerivedPath = tempPath;
                        GUIUtility.ExitGUI();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(ToolInfo.groupSpaceHeight);
            config.AlipayProjectCfg.CDN = EditorGUILayout.TextField("资源CDN：", config.AlipayProjectCfg.CDN).Trim();

            GUILayout.Space(ToolInfo.groupSpaceHeight);
            config.AlipayProjectCfg.MemorySize = EditorGUILayout.IntField("UnityHeap预留内存：", config.AlipayProjectCfg.MemorySize);
            GUILayout.Space(ToolInfo.groupSpaceHeight);
        }
        EditorGUILayout.EndVertical();

    }

    private static void LoadingInfoGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("启动加载配置", ToolInfo.LabelStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("frameBox");
        {
            GUILayout.Space(ToolInfo.groupSpaceHeight);
            EditorGUILayout.BeginHorizontal();
            {
                config.AlipayProjectCfg.LoadingImage = (Texture2D)EditorGUILayout.ObjectField("启动页背景图：", config.AlipayProjectCfg.LoadingImage, typeof(Texture2D), false);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            E_DataFileLoadType selectedLoadType;
            if (!Enum.TryParse(config.AlipayProjectCfg.DataFileLoadType, out selectedLoadType))
            {
                selectedLoadType = E_DataFileLoadType.CDN;
            }
            selectedLoadType = (E_DataFileLoadType)EditorGUILayout.EnumPopup("资源加载方式：", selectedLoadType);
            config.AlipayProjectCfg.DataFileLoadType = selectedLoadType.ToString();
            GUILayout.Space(ToolInfo.groupSpaceHeight);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                bool newUseDataZip = EditorGUILayout.Toggle(useDataZipContent, config.AlipayProjectCfg.UseDataZip);
                if (EditorGUI.EndChangeCheck())
                {
                    config.AlipayProjectCfg.UseDataZip = newUseDataZip;
                    if (config.AlipayProjectCfg.UseDataZip)
                    {
                        EditorUtility.DisplayDialog("注意", "CDN模式下，开启首包压缩资源后，需要将生成的.bin.data、与.bin.data.zip文件都上传到CDN上！！！", "确定");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("注意", "CDN模式下，开启首包压缩资源能有效降低启动耗时，建议勾选！", "确定");
                        config.AlipayProjectCfg.UseDataBrotli = false;
                    }
                }

                if (config.AlipayProjectCfg.UseDataZip)
                {
                    GUILayout.Label("(请确保后缀.data与.data.zip文件一起上传到对应的cdn上！)");
                    GUILayout.FlexibleSpace();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(ToolInfo.groupSpaceHeight);
            if (config.AlipayProjectCfg.UseDataZip)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    bool newUseBr = EditorGUILayout.Toggle(useDataBrContent, config.AlipayProjectCfg.UseDataBrotli);
                    if (EditorGUI.EndChangeCheck())
                    {
                        config.AlipayProjectCfg.UseDataBrotli = newUseBr;
                        if (config.AlipayProjectCfg.UseDataBrotli)
                        {
                            EditorUtility.DisplayDialog("注意", "CDN模式下，开启Br压缩资源后，需要将生成的 .bin.data.br文件 上传到CDN上！！！", "确定");
                        }
                    }
                    GUILayout.Label("Brotli格式  (将.data/.data.zip/.data.br三个文件上传到对应的cdn！)");
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(ToolInfo.groupSpaceHeight);
            }

            config.AlipayProjectCfg.BundleHashLength = EditorGUILayout.IntField("Bundle名中Hash长度: ", config.AlipayProjectCfg.BundleHashLength);
            GUILayout.Space(ToolInfo.groupSpaceHeight);

            config.AlipayProjectCfg.CacheableFileIdentifier = EditorGUILayout.TextField("自动缓存文件标识符: ", config.AlipayProjectCfg.CacheableFileIdentifier);
            GUILayout.Space(ToolInfo.groupSpaceHeight);

            config.AlipayProjectCfg.ExcludeFileIdentifier = EditorGUILayout.TextField("不需要自动缓存文件标识符: ", config.AlipayProjectCfg.ExcludeFileIdentifier);
            GUILayout.Space(ToolInfo.groupSpaceHeight);

        }
        EditorGUILayout.EndVertical();
    }

    private static void SaveConfig()
    {
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}

public enum E_DataFileLoadType
{
    游戏包内,
    CDN
}
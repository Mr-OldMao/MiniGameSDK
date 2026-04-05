using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildOption
{
    private static bool showBuildLayer = true;
    static CompileOptions CompileOptions = AlipayEditorWindow.GetEditorConfig().CompileOptions;
    public static void RenderGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("打包调试", ToolInfo.LabelStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(ToolInfo.groupSpaceHeight);


        if (showBuildLayer)
        {
            GUILayout.BeginVertical("frameBox");
            GUILayout.Space(ToolInfo.groupSpaceHeight);

            DrawToggleOption("Development Build", ref CompileOptions.DevelopBuild);
            DrawToggleOption("Auto Profile", ref CompileOptions.AutoProfile);
            DrawToggleOption("Script Only Build", ref CompileOptions.ScriptOnly);
            DrawToggleOption("Il2Cpp Optimize Size", ref CompileOptions.Il2CppOptimizeSize);
            DrawToggleOption("Profiling Funcs", ref CompileOptions.profilingFuncs, "（需要进行WASM分包，请勾选此项！）");
            //DrawToggleOption("Profiling Memory", ref CompileOptions.ProfilingMemory);
            DrawToggleOption("Use StreamingAssets", ref CompileOptions.UseStreamingAssets);

            GUILayout.EndVertical();
        }
        void DrawToggleOption(string label, ref bool option, string additionalInfo = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(label), GUILayout.Width(140));
            option = EditorGUILayout.Toggle(option);

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                GUILayout.Label(additionalInfo);
                GUILayout.FlexibleSpace();
            }

            GUILayout.EndHorizontal();
        }

    }
}
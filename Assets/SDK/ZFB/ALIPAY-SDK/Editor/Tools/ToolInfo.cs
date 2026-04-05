using UnityEngine;
using UnityEditor;

public class ToolInfo
{
    public const int titileFontSize = 24;
    public const int contentFontSize = 14;
    public const int padding = 10;
    public const int groupSpaceHeight = 5;
    public const int spaceHeight = 8;
    public const string minidevEnvSetupDocument = "https://www.yuque.com/xiaolvgreen/dzygd2/fgy7r1xntqwlqmhb?singleDoc";
    public static AlipayBuildType alipayBuildType;
    private static GUIStyle labelStyle;
    public static GUIStyle LabelStyle
    {
        get
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontSize = ToolInfo.contentFontSize;
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.alignment = TextAnchor.MiddleLeft;
            }
            return labelStyle;
        }
    }

    private static GUIStyle titileLabelStyle;

    public static GUIStyle TitileLabelStyle
    {
        get
        {
            if (titileLabelStyle == null)
            {
                titileLabelStyle = new GUIStyle(GUI.skin.label);
                titileLabelStyle.fontSize = ToolInfo.titileFontSize;
                titileLabelStyle.fontStyle = FontStyle.Bold;
                titileLabelStyle.alignment = TextAnchor.MiddleCenter;
            }
            return titileLabelStyle;
        }
    }
    private static GUIStyle redBoldLabel;
    public static GUIStyle RedBoldLabel
    {
        get
        {
            if (redBoldLabel == null)
            {
                redBoldLabel = new GUIStyle(GUI.skin.label);
                redBoldLabel.fontSize = ToolInfo.contentFontSize;
                redBoldLabel.fontStyle = FontStyle.Bold;
                redBoldLabel.alignment = TextAnchor.MiddleLeft;
                redBoldLabel.normal.textColor = Color.red;
            }
            return redBoldLabel;
        }
    }
    public static bool minidevEnvCorrect = false;

    public const string alipayProductPath = "Build/Alipay";

    public static string alipayKeyPath;

    public static string cdnURL = string.Empty;

    public static bool autoURL = false;


    public static void DrawLine()
    {
        GUILayout.Space(spaceHeight);
        GUILayout.Button("", GUILayout.Height(2));
        GUILayout.Space(spaceHeight);
    }
}

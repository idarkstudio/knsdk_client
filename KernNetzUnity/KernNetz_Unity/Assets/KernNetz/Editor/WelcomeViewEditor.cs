using UnityEditor;
using UnityEngine;

public sealed class WelcomeViewEditor : ISettingView
{
    public KernNetzEditorWindow ParentView { get; set; }
    private static Texture2D BackgroundImage;
    public WelcomeViewEditor(KernNetzEditorWindow entangleEditor)
    {
        ParentView = entangleEditor;
    }

    public void DisplayView()
    {
        GUILayout.Space(15);
        if (BackgroundImage == null)
        {
            string[] paths = AssetDatabase.FindAssets("EntangleGradient2 t:Texture2D");
            if (paths != null && paths.Length > 0)
            {
                BackgroundImage = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(paths[0]));
            }
        }
        UiTitleBox("KernNetz", BackgroundImage);
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("KernNetz (c) all rights reserved 2023");
        GUILayout.EndHorizontal();
    }

    public void ResetView()
    {
    }


    private void UiTitleBox(string title, Texture2D bgIcon)
    {
        GUIStyle bgStyle = EditorGUIUtility.isProSkin ? new GUIStyle(GUI.skin.GetStyle("Label")) : new GUIStyle(GUI.skin.GetStyle("WhiteLabel"));
        bgStyle.padding = new RectOffset(10, 10, 10, 10);
        bgStyle.fontSize = 22;
        bgStyle.fontStyle = FontStyle.Bold;
        if (bgIcon != null)
        {
            bgStyle.normal.background = bgIcon;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        Rect scale = GUILayoutUtility.GetLastRect();
        scale.height = 44;

        GUI.Label(scale, title, bgStyle);
        GUILayout.Space(scale.height + 5);
    }
}


public interface ISettingView
{
    KernNetzEditorWindow ParentView { get; set; }
    void ResetView();
    void DisplayView();
}
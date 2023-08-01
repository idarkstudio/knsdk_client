using UnityEditor;
using UnityEngine;

public class ConfigViewEditor : ISettingView
{
    public KernNetzEditorWindow ParentView { get; set; }

    public ConfigViewEditor(KernNetzEditorWindow entangleEditor)
    {
        ParentView = entangleEditor;
    }

    public void DisplayView()
    {
        GUILayout.BeginVertical("box");

        GUILayout.BeginHorizontal();
        GUILayout.Label("App Key", GUILayout.Width(100));
        ParentView.configuration.Config.AppId = EditorGUILayout.TextField(ParentView.configuration.Config.AppId, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Server IP", GUILayout.Width(100));
        ParentView.configuration.Config.ServerIp = EditorGUILayout.TextField(ParentView.configuration.Config.ServerIp, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Port", GUILayout.Width(100));
        ParentView.configuration.Config.Port = EditorGUILayout.IntField(ParentView.configuration.Config.Port, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sync Rate", GUILayout.Width(100));
        ParentView.configuration.Config.SyncRate = EditorGUILayout.IntSlider(ParentView.configuration.Config.SyncRate, 1, 30, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Transport Layer", GUILayout.Width(100));
        ParentView.Transport = (KernNetzEditorWindow.TransportLayer)EditorGUILayout.EnumPopup(ParentView.Transport, GUILayout.ExpandWidth(true));
        ParentView.configuration.Config.TransportLayer = ParentView.Transport.ToString();
        GUILayout.EndHorizontal();

        if (ParentView.Transport == KernNetzEditorWindow.TransportLayer.WebSockets)
        {
            GUILayout.BeginHorizontal("box");
            ParentView.configuration.Config.IsSecure = GUILayout.Toggle(ParentView.configuration.Config.IsSecure, "IsSecure", GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.HelpBox("ENet is the default transport layer, only change it if you have chaged it on Server.", MessageType.Warning);
        GUILayout.EndVertical();
    }

    public void ResetView()
    {
    }
}
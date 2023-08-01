using System;
using UnityEditor;
using UnityEngine;

public class KernNetzEditorWindow : EditorWindow
{
    private WelcomeViewEditor WelcomeView;
    private ConfigViewEditor ConfigView;
    private PrefabRegistryViewEditor PrefabRegistryView;
    public string appKey = "";
    int selectedModule = -1;

    public KernNetzConfiguration configuration;
    public KernNetzContainer prefabRegistry;
    private enum ViewState { Welcome, Config, PrefabRegistry, Count }
    ViewState view_state = ViewState.Welcome;

    public enum TransportLayer { ENet, LiteNetLib, WebSockets}
    public TransportLayer Transport = TransportLayer.ENet;

    [MenuItem("Tools/KernNetz/Settings")]
    public static void Init()
    {
        KernNetzEditorWindow window = EditorWindow.GetWindow<KernNetzEditorWindow>();
        window.titleContent = new UnityEngine.GUIContent("KernNetz", "KernNetz Settings");
        window.Show();
        //LoadDatabaseFromXML(DB_NAME);
    }

    void OnEnable()
    {
        selectedModule = -1;

        configuration = Resources.Load<KernNetzConfiguration>("ServerConfig");
        prefabRegistry = Resources.Load<KernNetzContainer>("EntangleContainer");

        ConfigView = new ConfigViewEditor(this);
        WelcomeView = new WelcomeViewEditor(this);
        PrefabRegistryView = new PrefabRegistryViewEditor(this);

        Transport = (TransportLayer)Enum.Parse(typeof(TransportLayer), configuration.Config.TransportLayer);
    }

    private void OnDestroy()
    {
        AssetDatabase.SaveAssets();
    }

    private void OnGUI()
    {
        DisplayMainView();
    }


    void DisplayMainView()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical("box", GUILayout.Width(180));
        GUILayout.Label("Modules", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();



        GUILayout.EndHorizontal();

        ShowList();


       


        GUILayout.Space(15);

        EditorGUILayout.HelpBox("Write changes to disk.", MessageType.Warning);

        if (GUILayout.Button("Save Changes", GUILayout.Width(150)))
        {
            EditorUtility.SetDirty(configuration);
            EditorUtility.SetDirty(prefabRegistry);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        GUILayout.EndVertical();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("KernNetz Settings", EditorStyles.boldLabel, GUILayout.Width(300));

        GUILayout.Space(25);

        switch (view_state)
        {
            case ViewState.Welcome:
                WelcomeView.DisplayView();
                break;
            case ViewState.Config:
                ConfigView.DisplayView();
                break;
            case ViewState.PrefabRegistry:
                PrefabRegistryView.DisplayView();
                break;
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }


    Vector2 scrollCharacters;

    void ShowList()
    {
        //return;
        scrollCharacters = GUILayout.BeginScrollView(scrollCharacters, GUILayout.Width(175));

        for (int i = 1; i < (int)ViewState.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (selectedModule == i) GUI.color = new Color(0.7f, 0.7f, 0.7f);
            if (GUILayout.Button(((ViewState)i).ToString(),GUILayout.ExpandWidth(true)))
            {
                // here display unique stats
                selectedModule = i;

                view_state = (ViewState)i;
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

    }


    public void ShowNotification(string message)
    {
        this.ShowNotification(new GUIContent(message));
    }
}

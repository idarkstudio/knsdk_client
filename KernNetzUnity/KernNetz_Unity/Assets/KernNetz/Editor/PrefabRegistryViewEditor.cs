using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PrefabRegistryViewEditor : ISettingView
{
    public KernNetzEditorWindow ParentView { get; set; }
    EntityType filter = EntityType.All;
    private enum EntityType 
    {
        All,
        Items,
        Agents
    } 
    public PrefabRegistryViewEditor(KernNetzEditorWindow entangleEditor)
    {
        ParentView = entangleEditor;
    }
   
    public void DisplayView()
    {
        GUILayout.BeginHorizontal("box");
        ParentView.prefabRegistry.MyPlayer = (KernNetzView)EditorGUILayout.ObjectField("My player", ParentView.prefabRegistry.MyPlayer, typeof(KernNetzView), false, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal("box");
        ParentView.prefabRegistry.RemotePlayer = (KernNetzView)EditorGUILayout.ObjectField("Remote player", ParentView.prefabRegistry.RemotePlayer, typeof(KernNetzView), false, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        GUILayout.Space(6);
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Network Entities");
        GUILayout.Label("Filter", EditorStyles.boldLabel, GUILayout.Width(75));
        filter = (EntityType)EditorGUILayout.EnumPopup(filter, GUILayout.Width(120));
        GUILayout.EndHorizontal();
        
        DisplayEntities();
        DisplayFooter();
    }
    Vector2 scrollItems;
    List<KernNetzView> entities = new List<KernNetzView>();
    private void DisplayEntities()
    {
        scrollItems = GUILayout.BeginScrollView(scrollItems, GUILayout.ExpandWidth(true));
        try
        {
            entities = ParentView.prefabRegistry.Entities;
            if (filter == EntityType.Items)
            {
                entities = ParentView.prefabRegistry.Entities.FindAll(e => e.EntityType == FigNetCommon.EntityType.Item);
            }
            else if (filter == EntityType.Agents)
            {
                entities = ParentView.prefabRegistry.Entities.FindAll(e => e.EntityType == FigNetCommon.EntityType.Agent);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
       

        for (int i = 0; i < entities.Count; i++)
        {
            //DisplayEntity(entities[i], i + 1);
            int index = i + 1;
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(index.ToString(), GUILayout.Width(50));
            entities[i] = (KernNetzView)EditorGUILayout.ObjectField("", entities[i], typeof(KernNetzView), false, GUILayout.ExpandWidth(true));

            GUI.color = Color.red;
            GUILayout.Space(30);
            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                // if (callBackDelete != null) callBackDelete();
                ParentView.prefabRegistry.Entities.RemoveAt(index - 1);
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

        }
        GUILayout.EndScrollView();
    }

    private void DisplayEntity(KernNetzView entangleView, int index)
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Label(index.ToString(), GUILayout.Width(50));
        entangleView = (KernNetzView)EditorGUILayout.ObjectField("", entangleView, typeof(KernNetzView), false, GUILayout.ExpandWidth(true));

        GUI.color = Color.red;
        GUILayout.Space(30);
        if (GUILayout.Button("-", GUILayout.Width(25)))
        {
            // if (callBackDelete != null) callBackDelete();
            ParentView.prefabRegistry.Entities.RemoveAt(index - 1);
        }
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
    }

    private void DisplayFooter()
    {
        GUILayout.Space(25);

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUI.color = new Color(0.6f,0.6f,0.6f);
        if (GUILayout.Button("Add Entity", GUILayout.Width(200)))
        {
            ParentView.prefabRegistry.Entities.Add(null);
        }
        GUI.color = Color.white;

        GUILayout.EndHorizontal();
    }

    public void ResetView()
    {
        
    }
}
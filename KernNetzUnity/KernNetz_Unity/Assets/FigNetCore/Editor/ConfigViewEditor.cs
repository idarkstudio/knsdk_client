using System;
using FigNet.Core;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using static FigNet.EditorUI.FigNetEditorWindow;

namespace FigNet.EditorUI
{
    public class ConfigViewEditor : ISettingView
    {
        private LoggingLevel Level = LoggingLevel.INFO;
        private static Texture2D BackgroundImage;
        public ConfigViewEditor(FigNetEditorWindow window)
        {
            this.ParentView = window;
            foldouts?.Clear();
            foreach (var config in window.configuration.Config.PeerConfigs)
            {
                foldouts.Add(true);
            }

            Level = (LoggingLevel)Enum.Parse(typeof(LoggingLevel), ParentView.configuration.Config.LoggingLevel, true);
        }
        public FigNetEditorWindow ParentView { get; set; }
        List<bool> foldouts = new List<bool>();
        public void DisplayView()
        {
            GUILayout.Space(15);
            if (BackgroundImage == null)
            {
                string[] paths = AssetDatabase.FindAssets("FigNetGradient t:Texture2D");
                if (paths != null && paths.Length > 0)
                {
                    BackgroundImage = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(paths[0]));
                }
            }
            UiTitleBox("FigNet Core", BackgroundImage);

            GUILayout.Space(6);
            //GUILayout.BeginHorizontal("box");
            //GUILayout.Label("Enable Compression", GUILayout.Width(150));
            //ParentView.configuration.Config.EnableCompression = GUILayout.Toggle(ParentView.configuration.Config.EnableCompression, "", GUILayout.ExpandWidth(true));
            //GUILayout.EndHorizontal();
            GUILayout.Space(6);

            DisplayConfigs();
            DisplayFooter();
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
        public void ResetView()
        {

        }

        Vector2 scrollItems, scrollItems2;
        //     List<EntangleView> entities = new List<EntangleView>();


        private void DisplaySubscribeDebugMessages(Configuration _config) 
        {

            for (int i = _config.SubscribeDebugMessages.Count - 1; i >= 0; i--)
            {
                GUILayout.Space(3);
                
                GUILayout.BeginHorizontal("box");
                
                GUILayout.Label($"{_config.SubscribeDebugMessages.Count - i}", GUILayout.Width(150));
                _config.SubscribeDebugMessages[i].Id = EditorGUILayout.IntField(_config.SubscribeDebugMessages[i].Id, GUILayout.ExpandWidth(true));
                GUI.color = Color.red;
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _config.SubscribeDebugMessages.RemoveAt(i);
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
                
            }

            if (GUILayout.Button("Add", GUILayout.ExpandWidth(true)))
            {
                _config.SubscribeDebugMessages.Add(new MessageInfo() { Id = 0 });
            }

        }
        
        private void DisplayConfigs()
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("AppSecret Key", GUILayout.Width(150));
            ParentView.configuration.Config.AppSecretKey = GUILayout.TextField(ParentView.configuration.Config.AppSecretKey, 120, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Encryption Key", GUILayout.Width(150));
            ParentView.configuration.Config.EncryptionKey = GUILayout.TextField(ParentView.configuration.Config.EncryptionKey, 120, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Tick Rate", GUILayout.Width(150));
            ParentView.configuration.Config.FrameRate = (ushort)EditorGUILayout.IntField(ParentView.configuration.Config.FrameRate, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();



            GUILayout.BeginVertical("box");


            GUILayout.BeginHorizontal("box");
            Level = (LoggingLevel)EditorGUILayout.EnumPopup("Logging Level", Level);
            ParentView.configuration.Config.LoggingLevel = Level.ToString();
            GUILayout.EndHorizontal();

            if (ParentView.configuration.Config.LoggingLevel == "ALL")
            {
                // todo : ahmed
                DisplaySubscribeDebugMessages(ParentView.configuration.Config);
            }
            else
            {
                if (ParentView.configuration.Config.SubscribeDebugMessages.Count > 0) 
                {
                    ParentView.configuration.Config.SubscribeDebugMessages.Clear();
                }
            }

            GUILayout.EndVertical();


            GUILayout.Space(6);

            scrollItems = GUILayout.BeginScrollView(scrollItems, GUILayout.ExpandWidth(true));

            var _config = ParentView.configuration.Config.PeerConfigs;

            for (int i = 0; i < _config.Count; i++)
            {
                GUILayout.Space(6);
                GUILayout.BeginVertical("box");
                DisplayConfig(_config[i], i);
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }

        private void DisplayConfig(PeerConfig config, int index)
        {
            foldouts[index] = EditorGUILayout.Foldout(foldouts[index], config.Name);

            if (foldouts[index])
            {
                Providers transport = Providers.ENet;
                try
                {
                    transport = (Providers)Enum.Parse(typeof(Providers), config.Provider);
                }
                catch (Exception e)
                {
                    transport = Providers.ENet;
                    Debug.LogWarning(e);
                }

                GUILayout.BeginHorizontal("box");
                config.EnableCheckSum = GUILayout.Toggle(config.EnableCheckSum, "Checksum", GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                config.IsMultiThreaded = GUILayout.Toggle(config.IsMultiThreaded, "IsMultiThreaded", GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                if (transport == Providers.WebSockets || transport == Providers.TCP)
                {
                    GUILayout.BeginHorizontal("box");
                    config.EnableDebugMessages = GUILayout.Toggle(config.EnableDebugMessages, "IsEnableDebugMessages", GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Name", GUILayout.Width(150));
                config.Name = GUILayout.TextField(config.Name, 150, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("AppName", GUILayout.Width(150));
                config.AppName = GUILayout.TextField(config.AppName, 150, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Server IP", GUILayout.Width(150));
                config.PeerIp = GUILayout.TextField(config.PeerIp, 150, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Port", GUILayout.Width(150));
                config.Port = (ushort)EditorGUILayout.IntField(config.Port, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Max Channels", GUILayout.Width(150));
                config.MaxChannels = (ushort)EditorGUILayout.IntField(config.MaxChannels, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("MaxSendQueueSize", GUILayout.Width(150));
                config.MaxSendQueueSize = (ushort)EditorGUILayout.IntField(config.MaxSendQueueSize, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("MaxReceiveQueueSize", GUILayout.Width(150));
                config.MaxReceiveQueueSize = (ushort)EditorGUILayout.IntField(config.MaxReceiveQueueSize, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();


                if (config.IsSecure && transport == Providers.TCP)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label("SSL Cert", GUILayout.Width(150));
                    config.Certificate.Name = EditorGUILayout.TextField(config.Certificate.Name, GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label("Password", GUILayout.Width(150));
                    config.Certificate.Password = EditorGUILayout.TextField(config.Certificate.Password, GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();
                   
                }

                if (transport == Providers.TCP || transport == Providers.WebSockets)
                {
                    GUILayout.BeginHorizontal("box");
                    config.IsSecure = GUILayout.Toggle(config.IsSecure, "IsSecure", GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();
                }


                GUILayout.BeginHorizontal("box");
                config.AutoConnect = GUILayout.Toggle(config.AutoConnect, "Auto Connect", GUILayout.Width(120));
                GUILayout.Label("DisconnectTimeout", GUILayout.Width(150));
                config.DisconnectTimeout = EditorGUILayout.IntField(config.DisconnectTimeout, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal("box");

                transport = (Providers)EditorGUILayout.EnumPopup("Provider", transport);
                config.Provider = transport.ToString();

                GUILayout.EndHorizontal();

                GUILayout.Space(3);
                GUILayout.BeginHorizontal("box");             


                GUI.color = GUI.color = new Color(0.6f, 0.6f, 0.6f);//Color.gray;
                var _config = ParentView.configuration.Config.PeerConfigs;
                if (_config.Count == 1) GUI.enabled = false;

                if (GUILayout.Button("Delete Config", GUILayout.ExpandWidth(true)))
                {
                    _config.RemoveAt(index);
                    foldouts.RemoveAt(index);
                }
                GUI.enabled = true;
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
                //GUILayout.Space(6);
            }
        }


        private void DisplayFooter()
        {
            GUILayout.Space(25);

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.color = new Color(0.6f, 0.6f, 0.6f);
            if (GUILayout.Button("Add Config", GUILayout.Width(200)))
            {
                PeerConfig config = new PeerConfig();
                config.AutoConnect = true;
                config.MaxChannels = 1;
                config.Name = "GameServer";
                config.Port = 5555;
                config.PeerIp = "127.0.0.1";
                config.Provider = "ENet";
                config.DisconnectTimeout = 30000;
                foldouts.Add(true);
                ParentView.configuration.Config.PeerConfigs.Add(config);
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();
        }
    }
}


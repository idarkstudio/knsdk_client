using System;
using KernNetz;
using UnityEngine;
using FigNetCommon;
using FigNet.KernNetz;
using System.Collections.Generic;

public static class ListedGames
{
    public static List<RoomInfo> rooms = new List<RoomInfo>();
    private static string _info;
    private static void DisplayRoomInfoSlot(RoomInfo info)
    {
        // name
        // activePlayer/MaxPlayer
        // join

        GUILayout.BeginHorizontal("box");
        GUILayout.Label($"Game Name: {info.RoomName}");

        GUILayout.Label($"Active/Max Players: {info.ActivePlayer}/{info.MaxPlayer}");

        if (info.ActivePlayer == info.MaxPlayer) GUI.enabled = false;

        if (GUILayout.Button("Join Game"))
        {
            if (AdvertiseGame.UserName == "")
            {
                _info = "Please enter user name";
            }
            else
            {
                Lobby.JoinRoom(info.Id, "", -1, (result) => {

                    if (result == RoomResponseCode.Sucess)
                    {
                        UIManager.ActiveUI = UIType.InGameUI;
                    }
                    else
                    {
                        _info = $"Request Status: {result}";
                    }
                });
            }
        }

        GUILayout.Label(_info);

        GUI.enabled = true;



        GUILayout.EndHorizontal();

        GUILayout.Space(6);
    }

    public static void Display()
    {
        int width = 600;
        int height = 800;


        GUILayout.BeginArea(new Rect((Screen.width / 2) - width / 2, Screen.height / 2, width, height));

        GUILayout.BeginVertical("box");

        GUILayout.Label("Enter Your Name:");
        AdvertiseGame.UserName = GUILayout.TextField(AdvertiseGame.UserName);
        
        GUILayout.Space(6);

        foreach (var room in rooms)
        {
            DisplayRoomInfoSlot(room);
        }

        GUILayout.EndVertical();

        if (GUILayout.Button("back to Create Game"))
        {
            UIManager.ActiveUI = UIType.CreateGame;
        }

        GUILayout.EndArea();
    }
}

public static class AdvertiseGame
{
    public static string GameName { get; private set; }
    public static string UserName { get; set; }
    public static int MaxPalyers { get; private set; }
    public static string Password { get; private set; } = "";

    private static string _maxPlayers;

    public static UIType Type { get; private set; } = UIType.CreateGame;

    private static string _info = $"{Type}";

    public static void Display()
    {
        int width = 400;
        int height = 600;


        GUILayout.BeginArea(new Rect((Screen.width / 2) - width / 2, Screen.height / 2, width, height));

        GUILayout.BeginVertical("box");

        // GUILayout.FlexibleSpace();
        GUILayout.Label("Enter Game Name:");
        GameName = GUILayout.TextField(GameName);

        GUILayout.Label("Enter Your Name:");
        UserName = GUILayout.TextField(UserName);

        GUILayout.Label("Enter Max Players:");
        _maxPlayers = GUILayout.TextField(_maxPlayers);
        int maxPlayer;
        bool ok = int.TryParse(_maxPlayers, out maxPlayer);
        if (!ok)
        {
            maxPlayer = 4;
            _maxPlayers = "";
        }
        GUILayout.Label(_info);

        GUILayout.BeginHorizontal("box");
        MaxPalyers = maxPlayer;

        if (GUILayout.Button("Create Game"))
        {
            if (GameName == "" || _maxPlayers == "" || UserName == "")
            {
                _info = "Please fill in all info";
            }
            else
            {
                Lobby.CreateRoom(GameName, (short)MaxPalyers, Password, (sucess, roomId) => {
                    Debug.Log($"Create Room Request {sucess} - {roomId} - max players {(short)MaxPalyers}");
                    if (sucess)
                    {
                        Lobby.JoinRoom(roomId, Password, -1, (response) =>
                        {
                            Debug.Log($"Join Room response {response}");
                            UIManager.ActiveUI = UIType.InGameUI;
                        });
                    }
                });
            }
        }

        if (GUILayout.Button("Join Existing Game"))
        {
            UIManager.ActiveUI = UIType.JoinGame;

            Lobby.GetRooms(RoomQuery.All, (rooms) =>
            {
                ListedGames.rooms = rooms;
            });
        }

        GUILayout.EndHorizontal();

        //   GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}


public static class InGameUI
{
    public static UIType Type { get; private set; } = UIType.InGameUI;

    private static string _info = $"{Type}";

    public static void Display()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(200));

        // GUILayout.FlexibleSpace();
        GUILayout.Label($"Ping: {KN.GetPing()}");
        GUILayout.Label($"User count: {KN.Room.Players.Count}");
        GUILayout.Label($"Spawned Items: {KN.Room.Items.Count}");
        GUILayout.Label($"Spawned NPCs: {KN.Room.Agents.Count}");
        if (KN.Socket != null)
        {
            GUILayout.Label($"Bytes Sent: {BandwidthCalculator.kiloBytesSentPerSec:F3} KB/s");
            GUILayout.Label($"Bytes Received: {BandwidthCalculator.kiloBytesReceivedPerSec:F3} KB/s");
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Spawn Item"))
        {
            NetworkEntity.Instantiate(EntityType.Item, 3, new FNVec3(2, 9, 2));
        }

        GUILayout.EndHorizontal();

        //if (GUILayout.Button("Spawn Agent"))
        //{
        //    NetworkEntity.Instantiate(EntityType.Agent, 10, new FNVec3(4, 0, 0));
        //}


        if (GUILayout.Button("Leave Game"))
        {
            Lobby.LeaveRoom((result) =>
            {
                UIManager.ActiveUI = UIType.CreateGame;
            });

        }

        GUILayout.EndVertical();

    }
}

public enum UIType
{
    None = 0,
    CreateGame,
    JoinGame,
    InGameUI
}

public class UIManager : MonoBehaviour
{
    public static UIType ActiveUI = UIType.CreateGame;

    private Dictionary<UIType, Action> Uis = new Dictionary<UIType, Action>();

    private void Start()
    {
        Uis.Add(UIType.None, () => { });
        Uis.Add(UIType.CreateGame, () => { AdvertiseGame.Display(); });
        Uis.Add(UIType.JoinGame, () => { ListedGames.Display(); });
        Uis.Add(UIType.InGameUI, () => { InGameUI.Display(); });
    }

    void OnGUI()
    {
        if (!KN.IsConnected) return;
        var activeView = Uis[ActiveUI];
        activeView.Invoke();

    }

}

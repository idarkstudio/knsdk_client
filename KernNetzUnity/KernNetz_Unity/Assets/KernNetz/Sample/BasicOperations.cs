using KernNetz;
using FigNet.Core;
using UnityEngine;
using FigNetCommon;
using FigNet.KernNetz;

public class BasicOperations : MonoBehaviour
{
    //[SerializeField] private GameObject _camera = default;

    enum EventCode : ushort
    {
        FireBullet = 1,
        CallDelivery = 4,
        Wisper = 5,
        Killed = 6,
    }

    enum GameStatus
    {
        WaitingForPlayers = 1,
        MatchSettingUp = 2,
        MatchInProgress =3,
        ResultScreen = 4

    }

    private void OnEnable()
    {
        KN.OnConnected += EN_OnConnected;
        Room.OnPlayerJoined += Room_OnPlayerJoined;
        KN.OnNetworkStatusChanged += NetworkStatusChange;


        Room.OnRoomStateChanged += Room_OnRoomStateChanged;
        Room.OnEventReceived += Room_OnEventReceived;

    }

    private void Room_OnEventReceived(uint sender, RoomEventData eventData)
    {
        //param.Add(0, 3);
        //param.Add(1, "hello");
        //param.Add(2, 6.0f);

        if (eventData.EventCode == (ushort)EventCode.FireBullet)
        {
            var p1 = (int)eventData.Data[0];
            var p2 = (string)eventData.Data[1];
            var p3 = (float)eventData.Data[2];
            Debug.Log($"Sender: {sender} :: p1 {p1} | p2 {p2} | p3 {p3}" );

        }

    }

    private void Room_OnRoomStateChanged(int state)
    {
        Debug.Log($"New Status of Room Is {state}");
    }

    private void OnDisable()
    {
        KN.OnConnected -= EN_OnConnected;
        Room.OnPlayerJoined -= Room_OnPlayerJoined;
        KN.OnNetworkStatusChanged -= NetworkStatusChange;
    }
    private int DCCount = 0;
    PeerStatus status;
    private void NetworkStatusChange(PeerStatus peerStatus) 
    {
        status = peerStatus;
        //if (peerStatus == PeerStatus.Timeout)
        //{
        //    EN.ReconnectAndJoin();
        //    DCCount++;
        //}
    }

    private void EN_OnConnected()
    {
        FN.Logger.Info("On Connected!");
        //Invoke(nameof(AutoCreateAndJoinRoom), 0.1f);
    }

    private void Room_OnPlayerJoined(NetPlayer obj)
    {
       
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    CreateRoom();
        //}
        //else if (Input.GetKeyDown(KeyCode.J))
        //{
        //    JoinRoom();
        //}

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    CreateItem();
        //}
        //else if (Input.GetKeyDown(KeyCode.L))
        //{
        //    CreateItem1();
        //}

        //else if (Input.GetKeyDown(KeyCode.M))
        //{
        //    CreateAgent();
        //}
        //else if (Input.GetKeyDown(KeyCode.R))
        //{
        //    REJoinRoom();
        //}

        //else if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    EN.Disconnect();
        //}
        //else if (Input.GetKeyDown(KeyCode.X)) 
        //{
        //    Invoke(nameof(AutoCreateAndJoinRoom), 0.1f);
        //}
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUILayout.Label($"Ping {KN.GetPing()}", GUILayout.Width(150));
        GUILayout.Label($"Status {status}", GUILayout.Width(150));
        GUILayout.Label($"Disconnects {DCCount}", GUILayout.Width(150));
        GUILayout.Label($"IsMasterClient {KN.IsMasterClient}", GUILayout.Width(150));

        if (GUILayout.Button("AutoCreateAndJoinRoom"))
        {
            Invoke(nameof(AutoCreateAndJoinRoom), 0.1f);
        }
        if (GUILayout.Button("Disconnect"))
        {
            KN.Disconnect();
        }
        if (GUILayout.Button("Rejoin"))
        {
            REJoinRoom();
        }
        if (GUILayout.Button("Create Room"))
        {
            CreateRoom();
        }
        if (GUILayout.Button("Join Room"))
        {
            JoinRoom();
        }
        if (GUILayout.Button("Spawn Cube 1"))
        {
            CreateItem();
        }
        if (GUILayout.Button("Spawn Cube 2"))
        {
            CreateItem1();
        }
        if (GUILayout.Button("Create Agent"))
        {
            CreateAgent();
        }

        if (GUILayout.Button("Delete Selected Cube"))
        {
            DeleteSelectedCube();
        }

        if (GUILayout.Button("send event"))
        {
            var data = new RoomEventData();

            data.EventCode = (ushort)EventCode.FireBullet;
            
            var param = new System.Collections.Generic.Dictionary<byte, object>();
            param.Add(0, 3);  // your team || user id
            param.Add(1, "hello"); // messahe
            param.Add(2, 6.0f);

            data.Data = param;

            Room.SendEvent(data, DeliveryMethod.Reliable);
        }


        if (GUILayout.Button("set room State"))
        {
            Room.SetRoomState((int)GameStatus.MatchSettingUp, true);
        }


        if (GUILayout.Button("Log Active Rooms"))
        {
            Lobby.GetRooms( RoomQuery.Avaliable, (result) => {

                foreach (var item in result)
                {
                    Debug.LogError(item.Id);
                }
            });
        }


        if (GUILayout.Button("LEAVE Room"))
        {
            LeaveRoom();
        }


        GUI.Label(new Rect(Screen.width - 110, Screen.height - 25f, 200f, 80f), "Entangle v0.6.2a");
        GUI.color = Color.white;
    }

    public uint roomToJoin = 1;

    [ContextMenu("join em")]
    public void JoinRooom() 
    {
        Lobby.JoinRoom(roomToJoin, "", -1, (response) =>
        {
            FN.Logger.Error($"Join Room Response {response}");
        });
    }

    private void AutoCreateAndJoinRoom() 
    {
        Lobby.GetRooms(RoomQuery.Avaliable, (result) => {
            if (result.Count > 0)
            {
                var room = result.Find(r => r.RoomName == "Test");
                if (room != null)
                {
                    Lobby.JoinRoom(result[0].Id, "", -1, (response) =>
                    {
                        FN.Logger.Info($"Join Room Response {response}");
                    });
                }

            }
            else
            {
                Lobby.CreateRoom("Test", 10, "", (sucess, roomId) => {
                    if (sucess)
                    {
                        Lobby.JoinRoom(roomId, "", -1, (response) =>
                        {
                            FN.Logger.Info($"Join Room Response {response}");
                        });
                    }
                });
            }
        });
    }

    [ContextMenu("CreateRoom")]
    public void CreateRoom()
    {
        Lobby.CreateRoom("Test", 5, "", (sucess, roomId) => {

        });
    }
    [ContextMenu("Join Room")]
    public void JoinRoom()
    {
        Lobby.GetRooms(RoomQuery.Avaliable, (result) => {

            Lobby.JoinRoom(result[0].Id, "", - 1, (response) => {

                FN.Logger.Info($"Join Room Response {response}");
            });
        });
    }

    [ContextMenu("REJoin Room")]
    public void REJoinRoom()
    {
        KN.ReconnectAndJoin();
    }


    [ContextMenu("leave Room")]
    public void LeaveRoom()
    {
        Lobby.LeaveRoom((response => {

            Debug.Log(response);
            UIManager.ActiveUI = UIType.JoinGame;
        }));
    }

    public void CreateItem()
    {
        NetworkEntity.Instantiate(EntityType.Item, 3, new FNVec3(0, 6, 0));
    }
    public void CreateItem1()
    {
        NetworkEntity.Instantiate(EntityType.Item, 6, new FNVec3(1, 25, -1), new FNVec3(0, 45, 0));
    }


    public void CreateAgent()
    {
        NetworkEntity.Instantiate(EntityType.Agent, 0, new FNVec3(0, 3, 0));
    }

    public void DeleteSelectedCube() 
    {
        if (ItemView.SelectedItem != null)
        {
            NetworkEntity.Delete(ItemView.SelectedItem.EntityType, ItemView.SelectedItem.NetworkId);
        }
    }
}

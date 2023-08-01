using System;
using KernNetz;
using FigNet.Core;
using FigNetCommon;
using FigNetCommon.Data;
using FigNet.KernNetz.Operations;
using System.Runtime.CompilerServices;

namespace FigNet.KernNetz
{
    public static class KN_Internals
    {
        public static bool IsMasterClient;
        public static string AppId;
    }

    public static class KN
    {
        public const string Version = "0.7.4f1";
        public static Room Room { private set; get; }
        public static Lobby Lobby { private set; get; }
        public static MatchMaker MatchMaker { private set; get; }
        public static bool IsMasterClient { get { return KN_Internals.IsMasterClient; } }
        public static bool IsConnected { get; private set; }
        public static bool IsInGame { get; set; }
        public static int MyTeamId { get; set; }
        public static uint MyPlayerId { get; set; }

        private static ClientSocketEventListner clientSocketEventListner = new ClientSocketEventListner();
        public static IClientSocket Socket;

        public static event Action OnConnected;
        public static event Action OnDisconnected;
        public static event Action<PeerStatus> OnNetworkStatusChanged;
        public static IClientSocket ClientSocket { get; private set; }
        private static bool isEventAdded = false;

        public static Action<bool> OnMasterClientUpdate;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPing()
        {
            int ping = -1;
            if (ClientSocket != null)
            {
                ping = ClientSocket.Ping;
            }
            return ping;
        }
        private class ClientSocketEventListner : IClientSocketListener
        {
            public void OnConnected()
            {
                KN.IsConnected = true;
                KN.OnConnected?.Invoke();
            }

            public void OnDisconnected()
            {
                KN.IsConnected = false;
                KN.OnDisconnected?.Invoke();
            }

            public void OnInitilize(IClientSocket clientSocket)
            {
                ClientSocket = clientSocket;
            }
            public void OnConnectionStatusChange(PeerStatus peerStatus)
            {
                OnNetworkStatusChanged?.Invoke(peerStatus);
            }
            public void OnNetworkReceive(Message message) 
            {
                bool handled = FN.HandlerCollection.HandleMessage(message, 0);
                // no handler is register for coming message
                if (!handled)
                {
                    // handle it mannually
                }
            }

            public void OnNetworkSent(Message message, DeliveryMethod method, byte channelId = 0)
            {
            }

            public void OnProcessPayloadException(ExceptionType type, ushort messageId, Exception e)
            {
                FN.Logger.Error($"Type {type} Message{(OperationCode)messageId}");
                FN.Logger.Info($"Exception {e.StackTrace}");
            }
        }


        private static void EN_OnNetworkStatusChanged(PeerStatus status)
        {
            if (status == PeerStatus.Timeout)
            {
                ReconnectAndJoin();
            }
        }

        public static void Initialize()
        {
            FN.Logger = new DefaultLogger();
            FN.Logger.SetUp(false,"");

            var serializer = new Default_Serializer();
            serializer.RegisterPayloads();

            FigNetCommon.Utils.Serializer = serializer;
            FigNetCommon.Utils.Logger = FN.Logger;

            Room = new Room();
            Lobby = new Lobby();
            MatchMaker = new MatchMaker();
            Lobby.Initialize();

            OnNetworkStatusChanged += EN_OnNetworkStatusChanged;
            OnConnected += Entangle_OnConnected;
        }

        private static void Entangle_OnConnected()
        {
            Socket.SendMessage(new RegisterAppIdOperation().GetOperation(KN_Internals.AppId), (response) => {

                var sucess = (bool)((response.Payload as OperationData).Parameters[0]);
                if (!sucess)
                {
                    Socket.Disconnect();
                    FN.Logger.Info("#EN Invalid AppKey");
                }
                else
                {
                    FN.Logger.Info("#EN valid AppKey");
                }
            });
        }

        private static KernNetzConfig Config = null;
        public static void SetConfig(KernNetzConfig config)
        {
            Config = config;
        }

        public static void Connect()
        {
            // make peerconfig object with properties
            // add connect & connect 

            if (Socket != null)
            {
                Socket.Reconnect();
            }
            else
            {
                bool isMultiThreaded = Config.TransportLayer == "WebSockets" ? false : Config.IsMultiThreaded;
                bool isSecure = Config.IsSecure;
                var peerConfig = new PeerConfig()
                {
                    Port = (ushort)Config.Port,
                    PeerIp = Config.ServerIp,
                    Provider = Config.TransportLayer,
                    MaxChannels = (ushort)Config.MaxChannels,
                    DisconnectTimeout = Config.DisconnectTimeout,
                    Name = Config.Nmae,
                    AppName = Config.AppName,
                    MaxReceiveQueueSize = (ushort)Config.MaxSendQueueSize,
                    MaxSendQueueSize = (ushort)Config.MaxReceiveQueueSize,
                    IsMultiThreaded = isMultiThreaded,
                    IsSecure = isSecure,
                    EnableCheckSum = Config.EnableCheckSum, 
                    

                };
                FN.Settings.AppSecretKey = Config.AppSecrectKey;
                FN.Settings.LoggingLevel = Config.LogginLevel.ToString();
                FN.Settings.ApplicationType = ApplicationType.Client;
                FN.Settings.EncryptionKey = Config.EncryptionKey;
                Socket = FN.AddConnectionAndConnect(peerConfig, clientSocketEventListner);
            }
        }

        /// <summary>
        /// Auto join the room you are in, on Reconnect
        /// </summary>
        public static void ReconnectAndJoin()
        {
            if (!isEventAdded)
            {
                isEventAdded = true;
                OnConnected += RejoinRoom;
            }
            Connect();
        }

        public static void Disconnect()
        {
            if (IsConnected && Socket != null )
            {
                Socket.Disconnect();
            }
        }

        private static void RejoinRoom()
        {
            if (!KN.IsInGame) return;
            Lobby.ReJoinRoom(KN.Room.RoomId, KN.Room.Password, KN.MyTeamId, true, KN.Room.RoomAuthToken, KN.MyPlayerId, (response) => {
                FN.Logger.Info($"Auto Rejoin Status: {response}");
            });
        }

    }
}

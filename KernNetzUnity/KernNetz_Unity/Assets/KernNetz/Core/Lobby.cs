using System;
using FigNet.Core;
using FigNetCommon;
using KernNetz.Handlers;
using FigNetCommon.Data;
using System.Collections.Generic;
using FigNet.KernNetz.Operations;

namespace FigNet.KernNetz
{
    public class Lobby
    {
        // get all rooms
        // create room or Join
        // Join room
        // DeleteRoom

        private bool init = false;

        readonly List<IHandler> handlers = new List<IHandler>() {
            new PlayerJoinRoomHandler(),
            new PlayerLeftRoomHandler(),
            new OnEntityStateChangeHandler(),
            new MasterClientChangeHandler(),
            new InstantiateEntityHandler(),
            new DeleteEntityHandler(),
            new RoomEventHandler(),
            new RequestOwnershipHandler(),
            new ClearOwnershipHandler(),
            new AgentOwnershipChangeHandler(),
            new RoomStateChangeHandler(),
            new PreRoomStateReceivedHandler(),
            new PostRoomStateReceivedHandler()
        };

        public void Initialize()
        {
            if (init) return;
            init = true;
            RegisterHandlers();
        }



        private void RegisterHandlers()
        {
            handlers.ForEach(handle => FN.HandlerCollection.RegisterHandler(handle));
        }
        private void UnRegisterHandlers()
        {
            handlers.ForEach(handle => FN.HandlerCollection.UnRegisterHandler(handle));
            handlers.Clear();
        }
        /// <summary>
        /// Creates a room on server 
        /// </summary>
        /// <param name="roomName"> Display Name of Room </param>
        /// <param name="maxPlayers"> Maximum numbers of players allowed in a room </param>
        /// <param name="password"> set password to none empty string to create private room </param>
        /// <param name="callBack"> bool: isRoom creation is sucessful, uint: uid of room, it is used to join room  </param>
        public static void CreateRoom(string roomName, short maxPlayers, string password, Action<bool, uint> callBack)
        {
            if (KN.Socket.IsConnected)
            {
                var createRoomOp = new CreateRoomOperation();
                var op = createRoomOp.GetOperation(roomName, maxPlayers, password);

                KN.Socket.SendMessage(op, (response) => {
                    uint RoomId = (uint)((response.Payload as OperationData).Parameters[0]);
                    //Entangle.Room.RoomId = RoomId;
                    //Entangle.Room.RoomName = roomName;
                    //Entangle.Room.MaxPlayer = maxPlayers;
                    FigNet.KernNetz.KN_Internals.IsMasterClient = true;
                    KN.Room.OnRoomCreated();
                    callBack?.Invoke(true, RoomId);
                });
            }
            else
            {
                FN.Logger.Error("Client is not connected to Server");
                callBack?.Invoke(false, 0);
            }

        }
        /// <summary>
        /// Get List of Avaliable rooms/game sessions
        /// </summary>
        /// <param name="query">RoomQuery [All | Avaliable ]</param>
        /// <param name="callBack">list of RoomInfo ( Id | RoomName | MaxPlayer | ActivePlayer | State )</param>
        public static void GetRooms(RoomQuery query, Action<List<RoomInfo>> callBack)
        {
            if (KN.Socket.IsConnected)
            {
                var getRoomOp = new GetRoomListOperation();
                var op = getRoomOp.GetOperation(query);

                KN.Socket.SendMessage(op, (response) => {

                    var data = (byte[])((response.Payload as OperationData).Parameters[0]);
                    var rooms = Default_Serializer.Deserialize2<List<RoomInfo>>(data); // MessagePack.MessagePackSerializer.Deserialize<List<RoomInfo>>(data, MessagePack.Resolvers.ContractlessStandardResolver.Options);

                    callBack?.Invoke(rooms);
                    //FN.Logger.Error($"getRooms SUCESS {rooms?.Count}");
                });
            }
            else
            {
                FN.Logger.Error("Client is not connected to Server");
            }
        }
        
        /// <summary>
        /// Join a room on server
        /// </summary>
        /// <param name="roomId">uid of room to join</param>
        /// <param name="password">provide password if room is password protected else pass empty sting</param>
        /// <param name="onRoomJoin">status code [Sucess|InvalidPassword|RoomLocked|RoomFull|Unknown|Failure]</param>
        public static void JoinRoom(uint roomId, string password, int teamId, Action<RoomResponseCode> onRoomJoin)
        {
            RoomResponseCode roomResponse = RoomResponseCode.Failure;
            if (KN.Socket.IsConnected)
            {
                var joinRoomOp = new JoinRoomOperation();
                var op = joinRoomOp.GetOperation(roomId, password, teamId);

                KN.Socket.SendMessage(op, (response) => {

                    var _opeartion = response.Payload as OperationData;
                    roomResponse = (RoomResponseCode)_opeartion.Parameters[0];
                    KN.Room.MyPlayerId = (uint)_opeartion.Parameters[1];
                    KN.Room.RoomId = roomId;
                    KN.Room.Password = password;

                    KN.Room.Init();

                    if (_opeartion.Parameters.ContainsKey(2))
                    {
                        KN.Room.RoomAuthToken = _opeartion.Parameters[2] as string;
                    }

                    onRoomJoin?.Invoke(roomResponse);
                });
            }
            else
            {
                FN.Logger.Error("Client is not connected to Server");
                onRoomJoin?.Invoke(RoomResponseCode.Failure);
                KN.Room.Deinit();
            }
        }

        public static void ReJoinRoom(uint roomId, string password, int teamId, bool isRejoining = false, string roomAuthToken = "", uint peerId = default, Action<RoomResponseCode> onRoomJoin = default)
        {
            RoomResponseCode roomResponse = RoomResponseCode.Failure;
            if (KN.Socket.IsConnected)
            {
                var joinRoomOp = new JoinRoomOperation();
                var op = joinRoomOp.GetOperation(roomId, password, teamId, isRejoining, roomAuthToken, peerId);

                KN.Socket.SendMessage(op, (response) => {

                    var _operation = response.Payload as OperationData;
                    roomResponse = (RoomResponseCode)(_operation.Parameters[0]);
                    KN.Room.MyPlayerId = (uint)_operation.Parameters[1];
                    KN.Room.RoomId = roomId;
                    KN.Room.Password = password;
                    KN.Room.Init();
                    onRoomJoin?.Invoke(roomResponse);
                });
            }
            else
            {
                FN.Logger.Error("Client is not connected to Server");
                onRoomJoin?.Invoke(RoomResponseCode.Failure);
                KN.Room.Deinit();
            }
        }

        /// <summary>
        /// Leave room
        /// </summary>
        /// <param name="onRoomLeft">status code [Sucess|InvalidPassword|RoomLocked|RoomFull|Unknown|Failure]</param>
        public static void LeaveRoom(Action<RoomResponseCode> onRoomLeft)
        {
            if (KN.Socket.IsConnected)
            {
                var operation = new Message((ushort)OperationCode.LeaveRoom, new OperationData(null));

                KN.Socket.SendMessage(operation, (response) => {

                    FN.Logger.Info($"On Room {KN.Room.RoomId} Leave {response}");
                    onRoomLeft?.Invoke(RoomResponseCode.Sucess);
                    KN.Room.OnRoomDisposed();
                    KN.Room.Deinit();
                });
            }
            else
            {
                FN.Logger.Error("Client is not connected to Server");
            }
        }
    }
}

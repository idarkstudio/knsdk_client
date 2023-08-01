using System;
using KernNetz;
using FigNet.Core;
using FigNetCommon;
using System.Numerics;
using FigNet.KernNetz.Operations;
using System.Collections.Generic;

namespace FigNet.KernNetz
{
	public class Room
	{
		//- roomId: string
		//- roomName: string
		//- clients: Client[]
		//- maxClients: number
		//- patchRate: number
		//- autoDispose: boolean
		//- locked:  (pass protected)
		//- state: T

		//- OnCreate
		//- OnDispose
		//- SetMetaData
		//- Disconnect
		//- OnJoin: peer
		//- OnLeave:peer
		//- OnMessage:IMessage, Peer
		//- send(client, message)
		//- broadcast(type, message)
		//- setSimulationInterval 
		//- setPatchRate

		// uid of rood
		public uint RoomId { get; set; }
		public string Password { get; set; }
		public string RoomAuthToken { get; set; }
		// Display name
		public string RoomName { get; set; }
		// Max allowed players in a session
		public int MaxPlayer { get; set; }
		// my player Id
		public uint MyPlayerId { get; set; }
		// if room is locked entry of new player is banned
		public bool IsLocked { get; set; }
		// used to set the state of room e.g. [waiting for player | Setting up | In progress | Result]
		public int RoomState { get; set; }
		// Gets triggered in CreateRoom response
		public static event Action OnRoomCreate;
		// Gets triggered in LeaveRoom response
		public static event Action OnRoomDispose;
		// Gets Triggered when room state changed
		public static event Action<int> OnRoomStateChanged;
		// Gets Triggered when a player Joins a room
		public static event Action<NetPlayer> OnPlayerJoined;
		// Gets Triggered when a player Left the room
		public static event Action<NetPlayer> OnPlayerLeft;
		// Gets Triggered when a NetworkItem or NetworkAgent is Spawned
		public static event Action<NetworkEntity, Vector3, Vector4, Vector3> OnEntityCreated;
		// Gets Triggered a NetworkItem or NetworkAgent is deleted
		public static event Action<NetworkEntity> OnEntityDeleted;
		// Gets Triggered when a player broadcast an event in a room
		public static event Action<uint, RoomEventData> OnEventReceived;

		// Gets triggered before netowking entinties spawning
		public static Action PreRoomStateReceived;
		// Gets triggered After netowking entinties spawning
		public static Action PostRoomStateReceived;

		public List<NetPlayer> Players = new List<NetPlayer>();

		public List<NetAgent> Agents = new List<NetAgent>();

		public List<NetItem> Items = new List<NetItem>();

		private List<NetworkEntity> networkEntities = new List<NetworkEntity>();

		private static List<IKernNetzRoomListener> _listeners = new List<IKernNetzRoomListener>();


		private const byte MAX_CHANNEL_LIMIT = 8;
		private static byte ItemChannel = 2;
		private static byte AgentChannel = 1;
		private static byte PlayerChannel = 0;

		private bool IsEnabled;

		private float timer;

		private float targetFrameRate;

		private static byte counter;

		private static int _syncRate = 21;
		public static int SYNC_RATE 
		{
			get { return _syncRate; }
			set 
			{
				_syncRate = value;
				if (_syncRate > 30)
				{
					_syncRate = 30;
				}
			} 
		}

		public static void BindListener(IKernNetzRoomListener listener)
		{
			if (!Room._listeners.Contains(listener))
			{
				Room._listeners.Add(listener);
			}
		}

		public static void UnBindListener(IKernNetzRoomListener listener)
		{
			if (Room._listeners.Contains(listener))
			{
				Room._listeners.Remove(listener);
			}
		}

		private void CreateTimer()
		{
			this.targetFrameRate = 1f / (float)SYNC_RATE;
			Room.OnPlayerJoined += delegate (NetPlayer player)
			{
				this.IsEnabled = true;
			};
			Room.OnRoomDispose += delegate ()
			{
				this.IsEnabled = false;
			};
		}

		private void StopTimer()
		{
			this.IsEnabled = false;
		}

		public void Tick(float deltaTime)
		{
			if (!IsEnabled) return;
			this.timer += deltaTime;
			if (this.timer >= this.targetFrameRate)
			{
				this.timer = 0f;
				for (int i = 0; i < this.Players.Count; i++)
				{
					this.Players[i].FlushStateDelta();
				}
				for (int j = 0; j < this.Agents.Count; j++)
				{
					this.Agents[j].FlushStateDelta();
				}
				for (int k = 0; k < this.Items.Count; k++)
				{
					this.Items[k].FlushStateDelta();
				}
			}
		}

		private void SetUpChannelId()
		{
			ItemChannel++;
			if (ItemChannel >= MAX_CHANNEL_LIMIT) ItemChannel = 0;
			AgentChannel++;
			if (AgentChannel >= MAX_CHANNEL_LIMIT) AgentChannel = 0;
			PlayerChannel++;
			if (PlayerChannel >= MAX_CHANNEL_LIMIT) PlayerChannel = 0;
		}

		public void Init()
		{
			this.CreateTimer();
			KN.IsInGame = true;
		}

		public void Deinit()
		{
			this.StopTimer();
			KN.IsInGame = false;
			FigNet.KernNetz.KN_Internals.IsMasterClient = false;

			Players.Clear();
			Agents.Clear();
			Items.Clear();
		}


		public void ClearEntities() 
		{
			Players.Clear();
			Agents.Clear();
			Items.Clear();
		}

		public void OnRoomCreated()
		{
			_listeners?.ForEach(l=>l.OnRoomCreated());
			OnRoomCreate?.Invoke();

			SetUpChannelId();
		}

		public void OnRoomDisposed()
		{
			_listeners?.ForEach(l => l.OnRoomDispose());
			OnRoomDispose?.Invoke();
		}

		public void PlayerJoin(NetPlayer networkPlayer)
		{
			var player = FindPlayer(networkPlayer.NetworkId);
			if (player == null)
			{
				Players.Add(networkPlayer);
				player = networkPlayer;
			}
			else
			{
                foreach (var state in networkPlayer.States)
                {
					player.ApplyStateDelta(state.Key, state.Value);
                }
			}
			
            for (int i = 0; i < _listeners.Count; i++)
            {
				_listeners[i].OnPlayerJoined(player);
            }
			OnPlayerJoined?.Invoke(player);
			player.channelID = PlayerChannel;
		}

		public void PlayerLeft(uint playerId)
		{
			var player = Players.Find(p => p.NetworkId == playerId);
			Players.Remove(player);
			for (int i = 0; i < _listeners.Count; i++)
			{
				_listeners[i].OnPlayerLeft(player);
			}
			OnPlayerLeft?.Invoke(player);
		}

		public void OnPreRoomStateReceive()
		{
            foreach (var listener in _listeners)
            {
				listener.PreRoomStateReceived();
            }
			PreRoomStateReceived?.Invoke();
		}

		public void OnPostRoomStateReceive() 
		{
            foreach (var listener in _listeners)
            {
				listener.PostRoomStateReceived();
			}
			PostRoomStateReceived?.Invoke();
		}

		public void OnEvent(uint sender, RoomEventData eventData)
		{
			for (int i = 0; i < _listeners.Count; i++)
			{
				_listeners[i].OnEventReceived(sender, eventData);
			}
			OnEventReceived?.Invoke(sender, eventData);
		}

		public void OnAgentOwnershipChange(uint networkId, uint ownerId)
		{
			var agent = Agents.Find(a => a.NetworkId == networkId);
			if (agent != null)
			{
				agent.UpdateProperties(ownerId, false, KN.Room.MyPlayerId);
			}
		}

		public void OnOwnershipRequest(uint networkId, uint ownerId, bool isLock)
		{
			var item = Items.Find(i => i.NetworkId == networkId);
			if (item != null)
			{
				item.UpdateProperties(ownerId, isLock, KN.Room.MyPlayerId);
			}
		}

		public void OnClearOwnership(uint networkId)
		{
			var item = Items.Find(i => i.NetworkId == networkId);
			if (item != null)
			{
				item.IsLocked = false;
			}
		}

		public void OnStateChange(uint sender, int state, bool isLocked)
		{
			RoomState = state;
			for (int i = 0; i < _listeners.Count; i++)
			{
				_listeners[i].OnRoomStateChange(state);
			}
			OnRoomStateChanged?.Invoke(state);
		}

		public static void SetRoomState(int state, bool isLock)
		{
			if (KN.Socket.IsConnected)
			{
				Message operation = new RoomStateChangeOperation().GetOperation(KN.Room.RoomId, state, isLock);
				KN.Socket.SendMessage(operation, DeliveryMethod.Reliable, 0);
			}
		}

		public static void SendEvent(RoomEventData eventData, DeliveryMethod deliveryMethod)
		{
			if (KN.Socket.IsConnected)
			{
				byte channelId = AgentChannel;
				var eventROomOp = new RoomEventOperation().GetOperation(KN.Room.RoomId, eventData, (int)deliveryMethod);
				KN.Socket.SendMessage(eventROomOp, deliveryMethod, channelId);
			}
		}

		public void OnEntityDelete(EntityType entityType, uint networkId)
		{
			switch (entityType)
			{
				case EntityType.Player:

					break;
				case EntityType.Agent:
					var agent = Agents.Find(a => a.NetworkId == networkId);
					if (agent != null)
					{
						OnEntityDeleted?.Invoke(agent);
						for (int i = 0; i < _listeners.Count; i++)
						{
							_listeners[i].OnAgentDeleted(agent);
						}
						Agents.Remove(agent);
					}
					break;
				case EntityType.Item:
					var item = Items.Find(a => a.NetworkId == networkId);
					if (item != null)
					{
						OnEntityDeleted?.Invoke(item);
						for (int i = 0; i < _listeners.Count; i++)
						{
							_listeners[i].OnItemDeleted(item);
						}
						Items.Remove(item);
					}
					break;
			}

		}

		public void OnEntityCreate(EntityType entityType, short entityId, uint networkId, uint ownerId, System.Numerics.Vector3 position, System.Numerics.Vector4 rotation, System.Numerics.Vector3 scale, byte[] states = null)
		{
			switch (entityType)
			{
				case EntityType.Player:
					FN.Logger.Error($"{entityType} creation is not supported via Instantiate Function");
					break;
				case EntityType.Agent:
					var entity = FindAgent(networkId);

					if (entity == null)
					{
						entity = NetAgent.CreateAgent(entityType, entityId, networkId);
						Agents.Add(entity);
					}

					entity.channelID = AgentChannel;
					if (states != null)
					{
						var __state = Default_Serializer.Deserialize2<EntityDefaultState>(states); //FN.Serializer.Deserialize<Dictionary<DeliveryMethod, EntangleState>>(new ArraySegment<byte>(states));

						foreach (var item in __state.states)
						{
							entity.ApplyStateDelta((DeliveryMethod)item.Key, item.Value);
						}
					}
					entity.OwnerId = ownerId;
					entity.UpdateOwner();
                    
					for (int i = 0; i < _listeners.Count; i++)
                    {
						_listeners[i].OnAgentCreated(entity, position, rotation, scale);
                    }
					OnEntityCreated?.Invoke(entity, position, rotation, scale);
					break;
				case EntityType.Item:
					NetItem _entity = FindItem(networkId);  // NetItem.CreateItem(entityType, entityId, networkId);

					if (_entity == null)
					{
						_entity = NetItem.CreateItem(entityType, entityId, networkId);
						Items.Add(_entity);
					}

					_entity.channelID = ItemChannel;
					if (states != null)
					{
						var __state = Default_Serializer.Deserialize2<EntityDefaultState>(states); // FN.Serializer.Deserialize<Dictionary<DeliveryMethod, EntangleState>>(new ArraySegment<byte>(states));
						foreach (var item in __state.states)
						{
							_entity.ApplyStateDelta((DeliveryMethod)item.Key, item.Value);
						}
					}
					//_entity.OwnerId = ownerId;
					_entity.UpdateProperties(ownerId, false, MyPlayerId);
					
					
					for (int i = 0; i < _listeners.Count; i++)
					{
						_listeners[i].OnItemCreated(_entity, position, rotation, scale);
					}
					OnEntityCreated?.Invoke(_entity, position, rotation, scale);
					break;
			}

		}

		// todo: implement custom find to avoid gc pressure
		public void OnEntityStateChangeReceive(EntityType type, uint entityId, EntangleState state)
		{
			switch (type)
			{
				case EntityType.Player:
					FindPlayer(entityId)?.ApplyStateDelta(state);
					break;
				case EntityType.Agent:
					FindAgent(entityId)?.ApplyStateDelta(state);
					break;
				case EntityType.Item:
					FindItem(entityId)?.ApplyStateDelta(state);
					break;
			}
		}


		private NetPlayer FindPlayer(uint networkId)
		{
			NetPlayer player = null;
			for (int i = 0; i < Players.Count; i++)
			{
				if (Players[i].NetworkId == networkId)
				{
					player = Players[i];
					break;
				}
			}
			return player;
		}

		private NetItem FindItem(uint networkId)
		{
			NetItem item = null;
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].NetworkId == networkId)
				{
					item = Items[i];
					break;
				}
			}
			return item;
		}

		private NetAgent FindAgent(uint networkId)
		{
			NetAgent agent = null;
			for (int i = 0; i < Agents.Count; i++)
			{
				if (Agents[i].NetworkId == networkId)
				{
					agent = Agents[i];
					break;
				}
			}
			return agent;
		}

	}
}

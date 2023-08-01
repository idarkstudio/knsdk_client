using UnityEngine;
using FigNet.Core;
using FigNetCommon;
using FigNet.KernNetz;

public class RoomManager : MonoBehaviour, IKernNetzRoomListener
{
    //private GameObject players;
    #region IEntangleRoomListener
    public void OnAgentCreated(NetAgent agent, System.Numerics.Vector3 position, System.Numerics.Vector4 rotation, System.Numerics.Vector3 scale)
    {
        KernNetzEntityManager.RecordSpawnedEntity(agent);
        var eView = KernNetzEntityManager.GetEntangleViewById(agent.NetworkId);
        if (eView != null)
        {
            if (position != System.Numerics.Vector3.Zero)
            {
                eView.transform.position = new Vector3(position.X, position.Y, position.Z);
            }
            if (rotation != System.Numerics.Vector4.Zero)
            {
                eView.transform.rotation = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
            }
        }
        else
        {
            var entity = Container.Entities.Find(e => e.EntityId == agent.EntityId && e.EntityType == EntityType.Agent);
            if (entity == null)
            {
                FN.Logger.Error($"#EN Agent {agent.EntityId} prefab is not register in Settings");
                return;
            }
            var agentView = GameObject.Instantiate<KernNetzView>(entity, EntitiesContainer.transform);
            agentView.name = entity.name + $" - {agent.EntityId} - {agent.NetworkId}";
            agentView.transform.position = new Vector3(position.X, position.Y, position.Z);
            agentView.transform.eulerAngles = new Vector3(rotation.X, rotation.Y, rotation.Z);
            //        agentView.transform.localScale = new Vector3(scale.X, scale.Y, scale.Z);
            agentView.SetNetworkEntity(agent, position, rotation);


            KernNetzEntityManager.AddEntity(agent.NetworkId, agentView);
        }
    }

    public void OnAgentDeleted(NetAgent agent)
    {
        KernNetzEntityManager.RemoveEntity(agent.NetworkId);
    }

    public void OnEventReceived(uint sender, RoomEventData eventData)
    {   
    }

    public void OnItemCreated(NetItem item, System.Numerics.Vector3 position, System.Numerics.Vector4 rotation, System.Numerics.Vector3 scale)
    {
        KernNetzEntityManager.RecordSpawnedEntity(item);
        var eView = KernNetzEntityManager.GetEntangleViewById(item.NetworkId);
        if (eView != null)
        {
            if (position != System.Numerics.Vector3.Zero)
            {
                eView.transform.position = new Vector3(position.X, position.Y, position.Z);
            }
            if (rotation != System.Numerics.Vector4.Zero)
            {
                eView.transform.rotation = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
            }
        }
        else
        {
            var entity = Container.Entities.Find(e => e.EntityId == item.EntityId && e.EntityType == EntityType.Item);
            if (entity == null)
            {
                FN.Logger.Error($"#EN Item {item.EntityId} prefab is not register in Settings");
                return;
            }

            if (position == System.Numerics.Vector3.Zero)
                position = new System.Numerics.Vector3(entity.transform.position.x, entity.transform.position.y, entity.transform.position.z);
            if (rotation == System.Numerics.Vector4.Zero)
                rotation = new System.Numerics.Vector4(entity.transform.rotation.x, entity.transform.rotation.y, entity.transform.rotation.z, transform.rotation.w);

            var itemView = GameObject.Instantiate<KernNetzView>(entity, EntitiesContainer.transform);
            itemView.name = entity.name + $" - {item.EntityId} - {item.NetworkId}";
            itemView.transform.position = new Vector3(position.X, position.Y, position.Z);
            itemView.transform.rotation = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
            //       itemView.transform.localScale = new Vector3(scale.X, scale.Y, scale.Z);
            itemView.SetNetworkEntity(item, position, rotation);

            KernNetzEntityManager.AddEntity(item.NetworkId, itemView);
            itemView.gameObject.SetActive(true);
        }
        
    }

    public void OnItemDeleted(NetItem item)
    {
        KernNetzEntityManager.RemoveEntity(item.NetworkId);
    }

    public void OnPlayerJoined(FigNet.KernNetz.NetPlayer player)
    {
        KernNetzEntityManager.RecordSpawnedEntity(player);
        var eView = KernNetzEntityManager.GetEntangleViewById(player.NetworkId);
        if (eView != null)
        {
            Debug.LogError($"Player Already Exist {player.NetworkId} | {player.IsMine}");
        }
        else
        {
            if (Container.MyPlayer == null) return;


            if (player.IsMine)
            {

                var _player = KernNetzEntityManager.GetMyPlayer();
                if (_player != null)
                {
                    var _playerView = KernNetzEntityManager.GetEntangleViewById(_player.NetworkId);
                    KernNetzEntityManager.RemoveEntity(_player.NetworkId, false);

                    _player.NetworkId = player.NetworkId;
                    _player.OwnerId = player.NetworkId;

                    KernNetzEntityManager.AddEntity(_player.NetworkId, _playerView);
                }
                else
                {
                    var myPlayer = GameObject.Instantiate<KernNetzView>(Container.MyPlayer, EntitiesContainer.transform);
                    if (myPlayer == null)
                    {
                        FN.Logger.Error("My Player prefab is not register in Settings");
                        return;
                    }
                    myPlayer.SetNetworkEntity(player, System.Numerics.Vector3.Zero, System.Numerics.Vector4.Zero);
                    KernNetzEntityManager.AddEntity(player.NetworkId, myPlayer);
                }
            }
            else
            {
                if (Container.RemotePlayer == null) return;
                var remotePlayer = GameObject.Instantiate<KernNetzView>(Container.RemotePlayer, EntitiesContainer.transform);

                if (remotePlayer == null)
                {
                    FN.Logger.Error("Remote Player prefab is not register in Settings");
                    return;
                }
                remotePlayer.SetNetworkEntity(
                    player,
                    new System.Numerics.Vector3(remotePlayer.transform.position.x, remotePlayer.transform.position.y, remotePlayer.transform.position.z),
                    new System.Numerics.Vector4(remotePlayer.transform.rotation.x, remotePlayer.transform.rotation.y, remotePlayer.transform.rotation.z, remotePlayer.transform.rotation.w));

                KernNetzEntityManager.AddEntity(player.NetworkId, remotePlayer);
            }
        }

        //    var alradyExists = EntangleEntityManager.AlreadyExists(player.NetworkId);
        //if (alradyExists)
        //{
        //    Debug.LogError($"Player Already Exist {player.NetworkId} | {player.IsMine}");
        //    return;
        //}
    }

    public void OnPlayerLeft(FigNet.KernNetz.NetPlayer player)
    {
        KernNetzEntityManager.RemoveEntity(player.NetworkId);
    }

    public void OnRoomCreated()
    {
        //FN.Logger.Info("EN: On Room Created!");
    }

    public void OnRoomDispose()
    {
        //FN.Logger.Info("EN: On Room Disposed!");
    }

    public void OnRoomStateChange(int status)
    {
        //FN.Logger.Info($"EN: On RoomState Change {status}");
    }

    #endregion

    private KernNetzContainer Container;
    private static GameObject EntitiesContainer;

    // todo: remove it if not needed
    //public void OnMasterClientUpdate(bool isMaster)
    //{
    //    if (!isMaster) return;

    //    var entities = EntangleEntityManager.GetEntangleViewEntitiesByType(EntityType.Item);
    //    foreach (var entity in entities)
    //    {
    //        if (!entity.NetworkEntity.IsLocked)
    //        {
    //            var rb = entity.GetComponent<Rigidbody>();
    //            rb.isKinematic = false;
    //        }
    //    }
    //}


    private void Room_OnRoomDispose()
    {
        KernNetzEntityManager.CleanUp();
    }

    public void PreRoomStateReceived()
    {
        PreNetworkEntitiesSpawn();
    }

    public void PostRoomStateReceived()
    {
        OnAllNetworkEntitiesSpawned();
    }

    private void OnAllNetworkEntitiesSpawned() 
    {
        KernNetzEntityManager.CheckAndRemoveEntitiesFromOldState();
    }

    private void PreNetworkEntitiesSpawn() 
    {
        KernNetzEntityManager.ClearSpawnedEntitiesMeta();
    }

    private static RoomManager instance = null;
    void OnEnable() 
    {
        Room.OnRoomDispose += Room_OnRoomDispose;
    }

    void OnDisable() 
    {
        Room.OnRoomDispose -= Room_OnRoomDispose;
    }
    void Awake() 
    {
        FigNet.KernNetz.Room.BindListener(this);

        // If the instance reference has not been set, yet, 
        if (instance == null)
        {
            // Set this instance as the instance reference.
            instance = this;
        }
        else if (instance != this)
        {
            // If the instance reference has already been set, and this is not the
            // the instance reference, destroy this game object.
            Destroy(gameObject);
        }

        // Do not destroy this object, when we load a new scene.
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        Container = Resources.Load<KernNetzContainer>("EntangleContainer");
        //EN.Room.Init();
        //EN.OnMasterClientUpdate = OnMasterClientUpdate;
        
        if (EntitiesContainer == null)
        {
            EntitiesContainer = new GameObject("__NETWORK_ENTITIES__");
            EntitiesContainer.transform.SetParent(transform);
            EntitiesContainer.transform.position = Vector3.zero;
        }
        
    }

    void FixedUpdate()
    {
        KN.Room.Tick(Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        FigNet.KernNetz.Room.UnBindListener(this);
        KN.Room.Deinit();
    }

   
}

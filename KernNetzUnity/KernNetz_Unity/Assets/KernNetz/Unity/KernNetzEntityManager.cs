using KernNetz;
using FigNet.Core;
using System.Linq;
using FigNetCommon;
using FigNet.KernNetz;
using System.Collections.Generic;

public class KernNetzEntityManager
{
    private static Dictionary<uint, KernNetzView> entities = new Dictionary<uint, KernNetzView>();
    private static Dictionary<uint, NetworkEntity> spawnedEntities = new Dictionary<uint, NetworkEntity>();

    public static void ClearSpawnedEntitiesMeta() 
    {
        spawnedEntities.Clear();
    }
    public static void RecordSpawnedEntity(NetworkEntity networkEntity) 
    {
        spawnedEntities.Add(networkEntity.NetworkId, networkEntity);
    }

    public static void CheckAndRemoveEntitiesFromOldState() 
    {
        var entitinesToRemove = new List<KernNetzView>();
        foreach (var entity in entities)
        {
            if (!spawnedEntities.ContainsKey(entity.Key))
            {
                entitinesToRemove.Add(entity.Value);
            }
        }

        foreach (var entity in entitinesToRemove)
        {
            entity.NetworkEntity.ClearStates();
            entities.Remove(entity.NetworkEntity.NetworkId);
            UnityEngine.GameObject.Destroy(entity.gameObject);
        }
        entitinesToRemove.Clear();
        spawnedEntities.Clear();
    }

    public static bool AlreadyExists(uint networkId)
    {
        return entities.ContainsKey(networkId);
    }

    public static void AddEntity(uint networkId, KernNetzView entangleView)
    {
        if (!entities.ContainsKey(networkId))
        {
            entities.Add(networkId, entangleView);
        }
        else
        {
            FN.Logger.Info($"Entity with Id {networkId} type {entangleView.EntityType} already exists");
        }
    }

    public static KernNetzView GetEntangleViewById(uint id)
    {
        KernNetzView view = null;

        if (entities.ContainsKey(id))
        {
            return entities[id];
        }

        return view;
    }

    public static List<KernNetzView> GetEntangleViewEntitiesByType(EntityType type)
    {
        var _entities = new List<KernNetzView>();

        foreach (var entity in entities)
        {
            if (entity.Value.EntityType == type)
            {
                _entities.Add(entity.Value);
            }
        }

        return _entities;
    }

    public static void RemoveEntity(uint networkId, bool deleteView = true)
    {
        if (entities.ContainsKey(networkId))
        {
            var entity = entities[networkId];
            entities.Remove(networkId);

            if (spawnedEntities.ContainsKey(networkId))
            {
                spawnedEntities.Remove(networkId);
            }

            if (deleteView)
            {
                entity.NetworkEntity?.ClearStates();
                if (entity.gameObject != null)
                {
                    UnityEngine.GameObject.Destroy(entity.gameObject);
                }
            } 
                
        }
        else
        {
            FN.Logger.Info($"Trying to remove Entity with Id {networkId} that does not exists");
        }
    }

    public static uint MyPlayerId()
    {
        uint id = uint.MaxValue;
        foreach (var entity in entities)
        {
            var player = entity.Value.NetworkEntity as NetPlayer;
            if (player != null)
            {
                if (player.IsMine) id = player.NetworkId;
            }
        }
        return id;
    }

    public static NetPlayer GetMyPlayer()
    {
        NetPlayer player = default;
        foreach (var entity in entities)
        {
            var _player = entity.Value.NetworkEntity as NetPlayer;
            if (_player != null)
            {
                if (_player.IsMine) player = _player;
                break;
            }
        }
        return player;
    }


    public static void ClearEntitiesByType(EntityType type)
    {
        var _entities = entities.Values.ToList();
        foreach (var item in _entities)
        {
            if (item.EntityType == EntityType.Player)
            {
                RemoveEntity(item.NetworkEntity.NetworkId);
            }
        }
    }

    public static void ClearEntities()
    {
        try
        {
            var _entities = entities.Values.ToList();

            foreach (var item in _entities)
            {
                RemoveEntity(item.NetworkEntity.NetworkId);
            }

            entities.Clear();
        }
        catch (System.Exception ex)
        {
            FN.Logger.Warning(ex.Message);
        }

    }


    public static void CleanUp() 
    {
        ClearEntities();
        ClearSpawnedEntitiesMeta();
    }
}
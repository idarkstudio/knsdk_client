using KernNetz;
using UnityEngine;
using FigNetCommon;

public class KernNetzView : MonoBehaviour
{
    public EntityType EntityType;
    public short EntityId;

    public NetworkEntity NetworkEntity { get; private set; }

    public T GetNetworkEntity<T>() where T : NetworkEntity
    {
        return NetworkEntity as T;
    }

    public void SetNetworkEntity(NetworkEntity networkEntity, System.Numerics.Vector3 position, System.Numerics.Vector4 rotation)
    {
        NetworkEntity = networkEntity;

        var syncChildComponents = GetComponentsInChildren<IEntangleSync>();

        foreach (var item in syncChildComponents)
        {
            item.Init(networkEntity, position, rotation);
        }
    }


    public void RequestOwnership(bool withLock = false)
    {
        NetworkEntity?.RequestOwnership(withLock);
    }

    public void ClearOwnership() 
    {
        NetworkEntity?.ClearOwnership();
    }

    private void OnDestroy()
    {
        NetworkEntity = null;
    }
}

using FigNet.Core;
using FigNetCommon;
using UnityEngine;
using FigNet.KernNetz;
using System.Collections.Generic;

public class RoomEventsListner : MonoBehaviour, IKernNetzRoomListener
{
    #region Network Room Events
    public void OnRoomCreated()
    {
        FN.Logger.Info($"On_Room_Create event");
        // here load envirnment
    }
    public void OnRoomDispose()
    {
        FN.Logger.Info($"On_Room_Disposed event");
        // here unload envirnment and go to home screen
    }
    public void OnEventReceived(uint sender, RoomEventData eventData)
    {
    }
    public void OnRoomStateChange(int status)
    {
        // here based on status trigger game logic
        FN.Logger.Info($"Room Status Changed to {status}");
    }
    public void OnAgentCreated(NetAgent agent, System.Numerics.Vector3 position, System.Numerics.Vector4 rotation, System.Numerics.Vector3 scale)
    {
    }

    public void OnAgentDeleted(NetAgent agent)
    {
    }

    public void OnItemCreated(NetItem item, System.Numerics.Vector3 position, System.Numerics.Vector4 rotation, System.Numerics.Vector3 scale)
    {
    }

    public void OnItemDeleted(NetItem item)
    {
    }

    public void OnPlayerJoined(NetPlayer player)
    {
    }

    public void OnPlayerLeft(NetPlayer player)
    {
    }
    #endregion

    private void Awake()
    {
        FigNet.KernNetz.Room.BindListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        return;
        // test code
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"On Leave request");
            Lobby.LeaveRoom((response) => {

                Debug.Log($"On Leave response {response}");
            });
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"On Event request");
            var data = new Dictionary<byte, object>()
            {
                { 3, 9 }
            };
            Room.SendEvent(new RoomEventData() { EventCode = 6, Data = data }, DeliveryMethod.Reliable);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log($"On Status Change request");
            Room.SetRoomState(4, true);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log($"On Spawn GameItems");
            NetItem.Instantiate(EntityType.Item, 3);
        }
    }

    public void PreRoomStateReceived()
    {
    }

    public void PostRoomStateReceived()
    {
    }
}

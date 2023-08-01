using FigNetCommon;

namespace FigNet.KernNetz
{
    public interface IKernNetzRoomListener
    {
        // add OnRoomEnter [will be triggered when your player joinned the room]
        // add OnRoomExit [will be called right before on room disposed] 
        void OnRoomCreated();
        void OnRoomDispose();
        void PreRoomStateReceived();
        void PostRoomStateReceived();
        void OnRoomStateChange(int status);
        void OnPlayerJoined(NetPlayer player);
        void OnPlayerLeft(NetPlayer player);
        void OnItemCreated(NetItem item, System.Numerics.Vector3 position, System.Numerics.Vector4 rotation, System.Numerics.Vector3 scale);
        void OnItemDeleted(NetItem item);
        void OnAgentCreated(NetAgent agent, System.Numerics.Vector3 position, System.Numerics.Vector4 rotation, System.Numerics.Vector3 scale);
        void OnAgentDeleted(NetAgent agent);
        void OnEventReceived(uint sender, RoomEventData eventData);

    }
}

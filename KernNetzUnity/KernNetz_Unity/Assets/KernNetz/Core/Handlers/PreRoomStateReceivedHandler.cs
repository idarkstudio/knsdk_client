using FigNet.Core;
using FigNetCommon;

namespace KernNetz.Handlers
{
    internal class PreRoomStateReceivedHandler : IHandler
    {
        public ushort MsgId => (ushort)OperationCode.PreRoomStateReceived;
        public void HandleMessage(Message message, uint PeerId)
        {
            var room = FigNet.KernNetz.KN.Room;
            room.OnPreRoomStateReceive();
        }
    }
}

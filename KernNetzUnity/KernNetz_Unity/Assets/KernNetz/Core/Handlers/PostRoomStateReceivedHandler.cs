using FigNet.Core;
using FigNet.KernNetz;
using FigNetCommon;

namespace KernNetz.Handlers
{
    internal class PostRoomStateReceivedHandler : IHandler
    {
        public ushort MsgId => (ushort)OperationCode.PostRoomStateReceived;
        public void HandleMessage(Message message, uint PeerId)
        {
            var room = FigNet.KernNetz.KN.Room;
            room.OnPostRoomStateReceive();
        }
    }
}

using FigNet.Core;
using FigNetCommon;
using FigNetCommon.Data;
using System.Collections.Generic;

namespace FigNet.KernNetz.Operations
{
	public class JoinRoomOperation
	{
		public Message GetOperation(uint roomId, string password, int teamId , bool rejoin = false, string roomAuthToken = "", uint peerId = default)
		{
			Dictionary<byte, object> parameters = new Dictionary<byte, object>
			{
				{ 0,	roomId	},
				{ 1,	password},
				{ 2,	teamId	},
				{ 3,	rejoin	},
				{ 4,	roomAuthToken	},
				{ 5,	peerId }
			};
			return new Message((ushort)OperationCode.JoinRoom, new OperationData(parameters));
		}

		public Message GetOperation(uint roomId, string password = "", int teamId = -1)
		{
			Dictionary<byte, object> parameters = new Dictionary<byte, object>
			{
				{ 0, roomId },
				{ 1, password },
				{ 2, teamId}
			};
			return new Message((ushort)OperationCode.JoinRoom, new OperationData(parameters));
		}
	}
}

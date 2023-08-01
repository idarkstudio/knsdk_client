using System.Collections.Generic;

namespace KernNetz
{
    [System.Serializable]
    public class KernNetzConfig
    {
        public string AppId;
        public string Nmae = "EntangleClient";
        public string AppName = "entangle";
        public string AppSecrectKey = "123";
        public string EncryptionKey = "MbQeThWmZq4t6w";
        public string ServerIp;
        public int Port;
        public string TransportLayer;
        public int MaxConnections; // optional I guess
        public int MaxChannels;
        public int DisconnectTimeout;
        public int SyncRate;
        public int MaxReceiveQueueSize = 10000;
        public int MaxSendQueueSize = 5000;
        public bool IsSecure = false;
        public bool EnableCheckSum = false;
        public bool IsMultiThreaded = true;
        public LogginLevel LogginLevel;
        public List<NetworkLOD> LODs;
    }

    [System.Serializable]
    public class NetworkLOD
    {
        public LODLevel level;
        public byte SyncPercent; // 0-100
    }

    public enum LODLevel : byte
    {
        Level0,
        Level1,
        Level2
    }


    public enum LogginLevel : byte
    {
        NONE,
        INFO,
        DEBUG,
        ALL
    }
}

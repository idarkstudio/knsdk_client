using FigNet;
using System;
using FigNet.Core;
using UnityEngine;
using Newtonsoft.Json;

[DefaultExecutionOrder(-10)]
public class DBAccessModule : MonoBehaviour, IClientSocketListener
{

    // Settings [later will be replace with Scriptable object]

    [SerializeField] private ushort port = 6002;
    [SerializeField] private string ServerIp = "localhost";
    [SerializeField] private string TransportLayer = "WebSockets";
    [SerializeField] private string AppName = "dbSocket";

    [SerializeField] private string Name = "DB Socket";
    
    
    private static IClientSocket dbSocket;
    

    // keep one Message for DB operations 
    // register Serialize & Deserialze methods & Define them
    // 


    
    void Start()
    {
        FN.RegisterPayload(60010, Serialize, Deserialize);

        Connect();
    }


    public static void Send<T>(byte[] data, Action<T> callback) where T : class 
    {
        var msg = new Message(60010, data, true);
        dbSocket.SendMessage(msg, (response) => {

            var json = response.Payload as string;
            // convert it to T & return
            try
            {
                callback?.Invoke(JsonConvert.DeserializeObject<T>(json));
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            Debug.Log(json);

        });
    }

    public static object Deserialize(ArraySegment<byte> buffer)
    {
        var data = BitBufferPool.GetInstance();
        data.FromArray(buffer);

        var json = data.ReadString();

        data.Clear();
        return json;
    }

    public static ArraySegment<byte> Serialize(object pingOperation)
    {
        var op = pingOperation as byte[];

        return new ArraySegment<byte>(op);
    }

    public void Connect()
    {
        // make peerconfig object with properties
        // add connect & connect 

        if (dbSocket != null)
        {
            dbSocket.Reconnect();
        }
        else
        {
            bool isMultiThreaded = true;
            var peerConfig = new PeerConfig()
            {
                Port = port,
                PeerIp = ServerIp,
                Provider = TransportLayer,
                MaxChannels = 1,
                DisconnectTimeout = 3000,
                Name = Name,
                AppName = AppName,
                MaxReceiveQueueSize = 100,
                MaxSendQueueSize = 100,
                IsMultiThreaded = isMultiThreaded,
                IsSecure = false,
                EnableCheckSum = false,

            };
            FN.Settings.AppSecretKey = "123";
            FN.Settings.LoggingLevel = "DEBUG";
            FN.Settings.EncryptionKey = "MbQeThWmZq4t6w";
            FN.Settings.ApplicationType = ApplicationType.Client;
            dbSocket = FN.AddConnectionAndConnect(peerConfig, this);

        }
    }

    #region IClientSocket Implementation

    public void OnInitilize(IClientSocket clientSocket)
    {
    }

    public void OnConnectionStatusChange(PeerStatus peerStatus)
    {
        if (peerStatus == PeerStatus.Timeout)
            Connect();
    }

    public void OnConnected()
    {
        Debug.Log($"Socket Connected: {dbSocket?.Name}");
    }

    public void OnDisconnected()
    {
    }

    public void OnNetworkReceive(Message message)
    {
    }

    public void OnNetworkSent(Message message, DeliveryMethod method, byte channelId = 0)
    {
    }

    public void OnProcessPayloadException(ExceptionType type, ushort messageId, Exception e)
    {
    }

    #endregion
}

using KernNetz;
using FigNet.Core;
using UnityEngine;
using FigNet.KernNetz;

public interface IEntangleSync
{
    void Init(NetworkEntity networkEntity, System.Numerics.Vector3 position = default, System.Numerics.Vector4 rotation = default, System.Numerics.Vector3 scale = default);
}

public class KernNetzManager : MonoBehaviour
{
    public static KernNetzConfiguration Configuration;

    public bool IsAutoConnect;

    private void Start()
    {
        ENetProvider.Module.Load();

        Configuration = Resources.Load<KernNetzConfiguration>("ServerConfig");

        KN_Internals.AppId = Configuration.Config.AppId;

        KN.OnConnected += Entangle_OnConnected;
        KN.OnDisconnected += Entangle_OnDisconnected;

        Room.SYNC_RATE = Configuration.Config.SyncRate;

        //var settings = Resources.Load<FigNetConfiguration>("FigNet_Configuration").Config;
        //FN.Initilize(settings);

        KN.Initialize();

        //FigNet.Entangle.Entangle.Room.BindListner

        KN.SetConfig(Configuration.Config);
        if (IsAutoConnect) Connect();
    }

    private void Entangle_OnDisconnected()
    {
        
    }

    private void Entangle_OnConnected()
    {
       
    }

    private void OnApplicationQuit()
    {
        FN.Deinitialize();
    }

    private void Update()
    {
        Process();
    }

    public void Process()
    {
        try
        {
            for (int i = 0; i < FN.Connections?.Count; i++)
            {
                FN.Connections[i].Process();
            }
            TimeScheduler.Tick(Time.deltaTime);
        }
        catch (System.Exception ex)
        {
            FN.Logger.Exception(ex, ex.Message);
        }
    }

    [ContextMenu("Connect")]
    public void Connect()
    {
        FigNet.KernNetz.KN.Connect();
    }

    [ContextMenu("Disconnect")]
    public void Disconnect()
    {
        KN.Disconnect();
    }
}

using UnityEngine;

[DefaultExecutionOrder(-10)]
public class FigNetProviderLoader : MonoBehaviour
{
    void Awake()
    {
        FigNet.Core.FN.BeforeInit();
        //TcpProvider.Module.Load();
        ENetProvider.Module.Load();
        WebSocketProvider.Module.Load();
        LiteNetLibProvider.Module.Load();
    }
}

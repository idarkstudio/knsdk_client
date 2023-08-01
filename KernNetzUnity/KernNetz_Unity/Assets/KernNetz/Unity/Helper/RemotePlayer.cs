using UnityEngine;
using FigNetCommon;
using NetPlayer = FigNet.KernNetz.NetPlayer;

public class RemotePlayer : MonoBehaviour
{
    public FNVector3 rotation = null;

    public NetPlayer NetworkPlayer { get; private set; }

    public void AttactToView(NetPlayer netPlayer)
    {
        NetworkPlayer = netPlayer;
        //rotation = NetworkPlayer.GetNetProperty<FNVector3>(0, PropertyType.FNVector3 );

        //NetworkPlayer.GetNetProperty<FNVector3>(0, PropertyType.FNVector3, DeliveryMethod.Unreliable, (rot) => {

        //    rotation = rot;
        //    if (rotation != null)
        //    {
        //        rotation.OnValueChange += Rotation_OnValueChange;
        //        transform.eulerAngles = new Vector3(rotation.Value.X, rotation.Value.Y, rotation.Value.Z);
        //    }
        //    else
        //    {
        //        FN.Logger.Error("Rotation is null");
        //    }
        //});


        //NetworkPlayer.Position.OnValueChange += Position_OnValueChange1;

        transform.position = new Vector3(netPlayer.Position.x, netPlayer.Position.y, netPlayer.Position.z);
    }

    private void Position_OnValueChange1(FNVec3 obj)
    {
        transform.position = new Vector3(obj.X, obj.Y, obj.Z);
    }

    private void Rotation_OnValueChange(FNVec3 obj)
    {
        transform.eulerAngles = new Vector3(obj.X, obj.Y, obj.Z);
    }
}

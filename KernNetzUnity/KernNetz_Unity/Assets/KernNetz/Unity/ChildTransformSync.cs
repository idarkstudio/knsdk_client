using KernNetz;
using FigNet.Core;
using UnityEngine;
using FigNetCommon;
using FigNet.KernNetz;
using System.Threading.Channels;

//[RequireComponent(typeof(EntangleView))]
public class ChildTransformSync : MonoBehaviour, IEntangleSync
{
    [Range(0, 2)]
    public float Smoothness = 1f;

    [Range(0, 255)]
    public int PositionIndex = 255;

    [Range(0, 255)]
    public int RotationIndex = 254;

    protected Quaternion velRot;
    protected Vector3 velPos;

    private FNVector4 rotation = default;
    private FNVector3 position = default;

    NetworkEntity networkEntity;
    private Vector3 itemPosition;
    private Quaternion itemRotation;

    private const float TIME_DELTA = 0.16667f;

    public bool EnableJitterBuffer = false;

    private float sync_rate = 0.1f;

    //private ERingBuffer<FNVec3> positionBuffer = new ERingBuffer<FNVec3>(30);
    //private ERingBuffer<FNVec3> rotationBuffer = new ERingBuffer<FNVec3>(30);

    private Channel<FNVec3> positionQueue;
    private Channel<FNVec4> rotationQueue;

    public FNVector4 Rotation => rotation;
    public FNVector3 Position => position;

    private Rigidbody e_rigidbody = null;
    private void OnOwnerShipChanged(bool isMine)
    {
        if (e_rigidbody != null)
        {
            e_rigidbody.isKinematic = !isMine;
        }
    }

    void OnDestroy()
    {
        if (this.networkEntity != null)
        {
            this.networkEntity.OnOWnershipChange -= OnOwnerShipChanged;
        }
    }

    public void Init(NetworkEntity networkEntity, System.Numerics.Vector3 _position = default, System.Numerics.Vector4 _rotation = default, System.Numerics.Vector3 _scale = default)
    {
        positionQueue = Channel.CreateBounded<FNVec3>(new BoundedChannelOptions(Room.SYNC_RATE) { FullMode = BoundedChannelFullMode.DropOldest, SingleReader = true, SingleWriter = true });
        rotationQueue = Channel.CreateBounded<FNVec4>(new BoundedChannelOptions(Room.SYNC_RATE) { FullMode = BoundedChannelFullMode.DropOldest, SingleReader = true, SingleWriter = true });

        sync_rate = (float)(1f / Room.SYNC_RATE);
        this.networkEntity = networkEntity;
        this.networkEntity.OnOWnershipChange += OnOwnerShipChanged;
        rotation = default;
        position = default;
        e_rigidbody = GetComponent<Rigidbody>();

        this.OnOwnerShipChanged(this.networkEntity.IsMine);

        if (this.networkEntity.EntityType == EntityType.Player)
        {
            if (this.networkEntity.IsMine)
            {
                rotation = new FNVector4();
                rotation.Value.SetValue(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                this.networkEntity.SetNetProperty<FNVector4>((byte)RotationIndex, rotation, FigNet.Core.DeliveryMethod.Unreliable);
            }
            else
            {
                this.networkEntity.GetNetProperty<FNVector4>((byte)RotationIndex, FigNet.Core.DeliveryMethod.Unreliable, (rot) =>
                {
                    rotation = rot;
                    rotation.OnValueChange += Rotation_OnValueChange;
                    itemRotation.x = rot.x;
                    itemRotation.y = rot.y;
                    itemRotation.z = rot.z;
                    itemRotation.w = rot.w;
                    transform.rotation = itemRotation;
                });
            }


            if (this.networkEntity.IsMine)
            {
                position = new FNVector3();
                position.Value.SetValue(transform.position.x, transform.position.y, transform.position.z);
                this.networkEntity.SetNetProperty<FNVector3>((byte)PositionIndex, position, FigNet.Core.DeliveryMethod.Unreliable);
            }
            else
            {
                this.networkEntity.GetNetProperty<FNVector3>((byte)PositionIndex, FigNet.Core.DeliveryMethod.Unreliable, (pos) =>
                {
                    position = pos;
                    position.OnValueChange += Position_OnValueChange;
                    itemPosition.x = pos.x;
                    itemPosition.y = pos.y;
                    itemPosition.z = pos.z;
                    transform.position = itemPosition;
                });
            }
        }
        else
        {

            this.networkEntity.GetNetProperty<FNVector3>((byte)PositionIndex, FigNet.Core.DeliveryMethod.Unreliable, (pos) =>
            {
                position = pos;

                position.OnValueChange += Position_OnValueChange;
                itemPosition.x = pos.x;
                itemPosition.y = pos.y;
                itemPosition.z = pos.z;
                transform.position = itemPosition;
            });

            if (position == default)
            {
                itemPosition.x = _position.X;
                itemPosition.y = _position.Y;
                itemPosition.z = _position.Z;
                position = new FNVector3()
                {
                    Value = new FNVec3(itemPosition.x, itemPosition.y, itemPosition.z)
                };
                transform.position = new Vector3(_position.X, _position.Y, _position.Z);
                this.networkEntity.SetNetProperty<FNVector3>((byte)PositionIndex, position, FigNet.Core.DeliveryMethod.Unreliable);
                position.Value = new FNVec3(itemPosition.x, itemPosition.y, itemPosition.z);
                position.OnValueChange += Position_OnValueChange;
            }

            this.networkEntity.GetNetProperty<FNVector4>((byte)RotationIndex, FigNet.Core.DeliveryMethod.Unreliable, (rot) =>
            {
                rotation = rot;

                rotation.OnValueChange += Rotation_OnValueChange;
                itemRotation.x = rot.x;
                itemRotation.y = rot.y;
                itemRotation.z = rot.z;
                itemRotation.w = rot.w;
                transform.rotation = itemRotation;
            });

            if (rotation == default)
            {
                itemRotation.x = _rotation.X;
                itemRotation.y = _rotation.Y;
                itemRotation.z = _rotation.Z;
                itemRotation.w = _rotation.W;
                transform.rotation = itemRotation;
                rotation = new FNVector4()
                {
                    Value = new FNVec4(itemRotation.x, itemRotation.y, itemRotation.z, itemRotation.w)
                };

                this.networkEntity.SetNetProperty<FNVector4>((byte)RotationIndex, rotation, FigNet.Core.DeliveryMethod.Unreliable);
                rotation.OnValueChange += Rotation_OnValueChange;
            }
        }

    }

    private void Rotation_OnValueChange(FNVec4 obj)
    {
        if (EnableJitterBuffer)
        {
            rotationQueue.Writer.TryWrite(obj);
            //rotationBuffer.Enqueue(obj);
        }
        else
        {
            itemRotation.x = obj.X;
            itemRotation.y = obj.Y;
            itemRotation.z = obj.Z;
            itemRotation.w = obj.W;
        }
    }

    private void Position_OnValueChange(FNVec3 obj)
    {
        if (EnableJitterBuffer)
        {
            //positionBuffer.Enqueue(obj);
            positionQueue.Writer.TryWrite(obj);
        }
        else
        {
            itemPosition.x = obj.X;
            itemPosition.y = obj.Y;
            itemPosition.z = obj.Z;
        }

    }

    private void FixedUpdate()
    {
        if (!EnableJitterBuffer) return;

        if (positionQueue.Reader.Count > 0)
        {
            positionQueue.Reader.TryRead(out FNVec3 pos);
            if (pos != null)
            {
                itemPosition.x = pos.X;
                itemPosition.y = pos.Y;
                itemPosition.z = pos.Z;
            }
        }

        if (rotationQueue.Reader.Count > 0)
        {
            rotationQueue.Reader.TryRead(out FNVec4 rot);
            if (rot != null)
            {
                itemRotation.x = rot.X;
                itemRotation.y = rot.Y;
                itemRotation.z = rot.Z;
                itemRotation.w = rot.W;
            }
        }
    }

    private void Update()
    {
        if (networkEntity != null)
        {
            if (networkEntity.IsMine)
            {
                SendMoveUpdate();
            }
            else
            {
                ApplyMoveUpdate();
            }
        }
    }
    FNVec3 posCache = new FNVec3();
    FNVec4 rotCache = new FNVec4();
    private void SendMoveUpdate()
    {
        posCache.SetValue(transform.position.x, transform.position.y, transform.position.z);
        rotCache.SetValue(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);


        if (position != null)
            position.SetValue(posCache);// new FNVec3(transform.position.x, transform.position.y, transform.position.z);//
        else
            FN.Logger.Info("pos is null");

        if (rotation != null) rotation.SetValue(rotCache);//  = new FNVec3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z); //
        else FN.Logger.Info("rot is null");
    }

    private void ApplyMoveUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, itemPosition, ref velPos, sync_rate * Smoothness);
        transform.rotation = QuaternionUtil.SmoothDamp(transform.rotation, itemRotation, ref velRot, sync_rate * Smoothness);

    }
}

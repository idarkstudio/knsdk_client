using KernNetz;
using UnityEngine;
using FigNetCommon;


// TODO in future it should not be mono-behaviour

[RequireComponent(typeof(KernNetzView))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TransformSync))]
public class RigidbodySync : MonoBehaviour, IEntangleSync
{
    private FNVector3 velocity = default;
    private FNVector3 angularVelocity = default;
    NetworkEntity networkEntity;
    private Vector3 itemVelocity;
    private Vector3 itemAngularVelocity;
    private Rigidbody itemRigidbody;

    public float threshold = 0.6f;

    private ERingBuffer<System.Numerics.Vector3> linearVelocityBuffer = new ERingBuffer<System.Numerics.Vector3>(30);
    private ERingBuffer<System.Numerics.Vector3> angularVelocityBuffer = new ERingBuffer<System.Numerics.Vector3>(30);

    private void Awake()
    {
        if (itemRigidbody == null) itemRigidbody = GetComponent<Rigidbody>();
        itemRigidbody.Sleep();
    }

    public void Init(NetworkEntity networkEntity, System.Numerics.Vector3 position = default, System.Numerics.Vector4 rotation = default, System.Numerics.Vector3 scale = default)
    {
        itemRigidbody = GetComponent<Rigidbody>();
        this.networkEntity = networkEntity;

        velocity = default;
        angularVelocity = default;

        if (this.networkEntity.EntityType == EntityType.Player)
        {
            if (this.networkEntity.IsMine)
            {
                velocity = new FNVector3();
                angularVelocity = new FNVector3();
                this.networkEntity.SetNetProperty<FNVector3>(243, velocity, FigNet.Core.DeliveryMethod.Unreliable);
                this.networkEntity.SetNetProperty<FNVector3>(242, angularVelocity, FigNet.Core.DeliveryMethod.Unreliable);
            }
            else
            {
                this.networkEntity.GetNetProperty<FNVector3>(243, FigNet.Core.DeliveryMethod.Unreliable, (vel) =>
                {

                    velocity = vel;
                    velocity.OnValueChange += Velocity_OnValueChange;
                    itemVelocity.x = vel.Value.X;
                    itemVelocity.y = vel.Value.Y;
                    itemVelocity.z = vel.Value.Z;
                });

                this.networkEntity.GetNetProperty<FNVector3>(242, FigNet.Core.DeliveryMethod.Unreliable, (aVel) =>
                {

                    angularVelocity = aVel;
                    angularVelocity.OnValueChange += AngularVelocity_OnValueChange;
                    itemAngularVelocity.x = aVel.Value.X;
                    itemAngularVelocity.y = aVel.Value.Y;
                    itemAngularVelocity.z = aVel.Value.Z;
                });

            }
        }
        else
        {
            this.networkEntity.GetNetProperty<FNVector3>(243, FigNet.Core.DeliveryMethod.Unreliable, (vel) =>
            {
                velocity = vel;

                velocity.OnValueChange += Velocity_OnValueChange;
                itemVelocity.x = vel.Value.X;
                itemVelocity.y = vel.Value.Y;
                itemVelocity.z = vel.Value.Z;

                itemRigidbody.velocity = itemVelocity;
            });

            if (velocity == default)
            {
                velocity = new FNVector3();
                this.networkEntity.SetNetProperty<FNVector3>(243, velocity, FigNet.Core.DeliveryMethod.Unreliable);
                velocity.OnValueChange += Velocity_OnValueChange;
            }

            this.networkEntity.GetNetProperty<FNVector3>(242,FigNet.Core.DeliveryMethod.Unreliable, (aVel) =>
            {
                angularVelocity = aVel;

                angularVelocity = aVel;
                angularVelocity.OnValueChange += AngularVelocity_OnValueChange;
                itemAngularVelocity.x = aVel.Value.X;
                itemAngularVelocity.y = aVel.Value.Y;
                itemAngularVelocity.z = aVel.Value.Z;

                itemRigidbody.angularVelocity = itemAngularVelocity;
            });

            if (angularVelocity == default)
            {
                angularVelocity = new FNVector3();
                this.networkEntity.SetNetProperty<FNVector3>(242, angularVelocity, FigNet.Core.DeliveryMethod.Unreliable);
                angularVelocity.OnValueChange += AngularVelocity_OnValueChange;
            }
        }
    }

    private void Velocity_OnValueChange(FNVec3 obj)
    {
        if (networkEntity != null)
        {
            itemVelocity.x = obj.X;
            itemVelocity.y = obj.Y;
            itemVelocity.z = obj.Z;

            itemRigidbody.velocity = itemVelocity;
            //linearVelocityBuffer.Enqueue(obj);
        }
    }

    private void AngularVelocity_OnValueChange(FNVec3 obj)
    {
        if (networkEntity != null)
        {
            itemAngularVelocity.x = obj.X;
            itemAngularVelocity.y = obj.Y;
            itemAngularVelocity.z = obj.Z;

            itemRigidbody.angularVelocity = itemAngularVelocity;
            //angularVelocityBuffer.Enqueue(obj);
        }
    }


    private void FixedUpdate()
    {
        if (networkEntity != null)
        {
            if (networkEntity.IsMine)
            {
                SendPhysicsUpdate();
            }
            else
            {
                if (!linearVelocityBuffer.IsEmpty)
                {
                    var pos = linearVelocityBuffer.Dequeue();
                    itemVelocity.x = pos.X;
                    itemVelocity.y = pos.Y;
                    itemVelocity.z = pos.Z;

                    itemRigidbody.velocity = itemVelocity;
                }

                if (!angularVelocityBuffer.IsEmpty)
                {
                    var rot = angularVelocityBuffer.Dequeue();
                    itemAngularVelocity.x = rot.X;
                    itemAngularVelocity.y = rot.Y;
                    itemAngularVelocity.z = rot.Z;

                    itemRigidbody.angularVelocity = itemAngularVelocity;
                }
            }
        }
    }

    // TODO: OPT [optimization cache fnvec3]
    FNVec3 rigCache = new FNVec3();
    FNVec3 angCache = new FNVec3();
    private void SendPhysicsUpdate()
    {
        if (itemRigidbody.velocity.sqrMagnitude > threshold)
        {
            rigCache.SetValue(itemRigidbody.velocity.x, itemRigidbody.velocity.y, itemRigidbody.velocity.z);
            velocity.SetValue(rigCache);// = new FNVec3(itemRigidbody.velocity.x, itemRigidbody.velocity.y, itemRigidbody.velocity.z); //rigCache;  // 
        }
        if (itemRigidbody.angularVelocity.sqrMagnitude > threshold)
        {
            angCache.SetValue(itemRigidbody.angularVelocity.x, itemRigidbody.angularVelocity.y, itemRigidbody.angularVelocity.z);
            angularVelocity.SetValue(angCache); // = new FNVec3(itemRigidbody.angularVelocity.x, itemRigidbody.angularVelocity.y, itemRigidbody.angularVelocity.z);// angCache; //
        }
    }


    private void ApplyPhysicsUpdate()
    {
        itemRigidbody.MovePosition(itemVelocity);
        itemRigidbody.MoveRotation(Quaternion.Euler(itemAngularVelocity));
        itemRigidbody.velocity = itemVelocity;
        itemRigidbody.angularVelocity = itemAngularVelocity;
    }

}

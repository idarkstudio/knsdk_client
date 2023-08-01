using System;
using UnityEngine;

public class Tesst : MonoBehaviour
{

    public Transform master, slave;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var rot = ToQuaternion(new System.Numerics.Vector3(master.rotation.eulerAngles.x, master.rotation.eulerAngles.y, master.rotation.eulerAngles.z));
        slave.transform.rotation = new UnityEngine.Quaternion(rot.X, rot.Y, rot.Z, rot.W);
    }



    private System.Numerics.Vector4 ToQuaternion(System.Numerics.Vector3 v)
    {
        v.X = v.X * 0.01745f;
        v.Y = v.Y * 0.01745f;
        v.Z = v.Z * 0.01745f;

        float cy = (float)Math.Cos(v.Z * 0.5f);
        float sy = (float)Math.Sin(v.Z * 0.5f);
        float cp = (float)Math.Cos(v.Y * 0.5f);
        float sp = (float)Math.Sin(v.Y * 0.5f);
        float cr = (float)Math.Cos(v.X * 0.5f);
        float sr = (float)Math.Sin(v.X * 0.5f);

        return new System.Numerics.Vector4
        {
            W = (cr * cp * cy + sr * sp * sy),
            X = (sr * cp * cy - cr * sp * sy),
            Y = (cr * sp * cy + sr * cp * sy),
            Z = (cr * cp * sy - sr * sp * cy)
        };

    }
}

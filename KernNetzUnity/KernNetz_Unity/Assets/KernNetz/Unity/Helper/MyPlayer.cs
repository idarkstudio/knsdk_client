using FigNet.Core;
using UnityEngine;
using NetPlayer = FigNet.KernNetz.NetPlayer;

public class MyPlayer : MonoBehaviour
{
    //public NetPlayer NetworkPlayer { get; private set; }

    //public FNVector3 rotation = new FNVector3();

    //private Vector3 lastPosition, lastRotation;
    //private Transform myTransform;

    public void AttactToView(NetPlayer netPlayer)
    {
        //NetworkPlayer = netPlayer;

        //NetworkPlayer.SetNetProperty<FNVector3>(0, PropertyType.FNVector3, rotation, FigNet.Core.DeliveryMethod.Unreliable);

        //rotation.OnValueChange += Position_OnValueChange;

        //NetworkPlayer.Position.OnValueChange += Position_OnValueChange1;
    }

    private void Position_OnValueChange1(System.Numerics.Vector3 obj)
    {
        FN.Logger.Info($"{obj}");
    }

    private void Position_OnValueChange(System.Numerics.Vector3 obj)
    {
        FN.Logger.Info($"{obj}");
    }

    void Start()
    {
        //myTransform = transform;
    }

    float yAngle;

    void Update()
    {

        if (Input.GetKey(KeyCode.Q))
        {
            yAngle += Time.deltaTime;
            transform.Rotate(Vector3.up, 1);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            yAngle -= Time.deltaTime;
            transform.Rotate(Vector3.up, -1);
        }

        

        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        if (x > 0.2f || x < 0.2f || y > 0.2f || y < -0.2f)
        {
            transform.position += new Vector3(-y, 0, x) * Time.deltaTime * 3f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 6f, ForceMode.Impulse);
        }
    }
}

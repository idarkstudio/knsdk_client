using KernNetz;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(KernNetzView))]
public class RigidbodyStablizer : MonoBehaviour
{
    private NetworkEntity networkEntity;
    private Rigidbody rbody;
    void Start()
    {
        networkEntity = GetComponent<KernNetzView>().NetworkEntity;
        rbody = GetComponent<Rigidbody>();
        InvokeRepeating(nameof(CheckForOwnerShip), 1f, 0.20f);
    }


    private void OnDestroy()
    {
        CancelInvoke(nameof(CheckForOwnerShip));
    }

    private void CheckForOwnerShip()
    {
        if (!networkEntity.IsMine)
        {
            rbody.isKinematic = true;
        }
    }

}
using KernNetz;
using UnityEngine;
using FigNet.KernNetz;

public class ItemView : MonoBehaviour
{
    public bool isSelected = false;

    Color defaulColor;
    Color selectedColor = Color.cyan;

    public static NetworkEntity SelectedItem;

    private void OnOwnerShipChanged(bool isMine) 
    {
        if (!isMine)
        {
            isSelected = false;
            GetComponent<Renderer>().material.color = defaulColor;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void Awake()
    {
        defaulColor = GetComponent<Renderer>().material.color;
    }

    private void Start()
    {
        entangleView = GetComponent<KernNetzView>();
        entangleView.NetworkEntity.OnOWnershipChange += OnOwnerShipChanged;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && isSelected)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 3.5f, ForceMode.Impulse);
        }
        else if (Input.GetKeyDown(KeyCode.X) && isSelected)
        {
            if (SelectedItem != null )
            {
                NetworkEntity.Delete(FigNetCommon.EntityType.Item, SelectedItem.NetworkId);
            }
        }
    }

    //private void OnGUI()
    //{
    //    if (entangleView == null) return;
    //    GUI.color = Color.black;
    //    GUILayout.Space(20);
    //    GUILayout.Label($"{entangleView.NetworkEntity.IsMine}");
    //    GUILayout.Label($"{entangleView.NetworkEntity.EntityId}");
    //    GUILayout.Label($"{entangleView.NetworkEntity.OwnerId}");
    //    GUILayout.Label($"{entangleView.NetworkEntity.DeltaCounter}");
    //    GUILayout.Label($"{entangleView.NetworkEntity.Position.x}|{entangleView.NetworkEntity.Position.y}|{entangleView.NetworkEntity.Position.z}");
    //}
    KernNetzView entangleView = null;
    private void OnMouseDown()
    {
        isSelected = !isSelected;

        if (isSelected)
        {
            SelectedItem = GetComponent<KernNetzView>().NetworkEntity;

            bool ok = (SelectedItem as NetItem).RequestOwnership();
            GetComponent<Renderer>().material.color = selectedColor;

        }
        else
        {
            GetComponent<Renderer>().material.color = defaulColor;
            SelectedItem = null;
        }
    }

    [ContextMenu("AForce")]
    public void AddForce()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 6f, ForceMode.Impulse);
    }
}

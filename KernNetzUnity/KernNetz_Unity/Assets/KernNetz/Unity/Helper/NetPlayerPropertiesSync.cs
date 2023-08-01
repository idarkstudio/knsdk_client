using KernNetz;
using UnityEngine;
using FigNet.Core;
using FigNetCommon;
using FigNet.KernNetz;
using TMPro;

public class NetPlayerPropertiesSync : MonoBehaviour
{
    // define NetworkProperties indexes for better readability
    private enum NetworkProperties : byte
    {
        UserName = 0,
        SelectedHero = 6,
        Health = 7,
        Stamina = 8,
        Name = 9,
    }

    // get reference to NetworkEntity it contains EntangleState
    private NetworkEntity networkEntity;


    // our selected hero
    public int MySelectedHero;
    // Define EntangleProperty it will sync the above property across
    public FNShort selectedHero = null;
    public FNString UserName = default;

    public TMP_Text user_name;

    public 

    
    void Start()
    {
        
        PlayerGlobalSettings.SelectedHero = (short)MySelectedHero;
        networkEntity = GetComponent<KernNetzView>().NetworkEntity;
        if (networkEntity.IsMine)
        {
            selectedHero = new FNShort();
            networkEntity.SetNetProperty<FNShort>((byte)NetworkProperties.SelectedHero, selectedHero, FigNet.Core.DeliveryMethod.Reliable);

            // set my selected hero [so that it will be broadcasted to others]
            selectedHero.Value = (short)MySelectedHero;

        }
        else
        {
            Room.OnPlayerJoined += Room_OnPlayerJoined;

            networkEntity.GetNetProperty<FNShort>((byte)NetworkProperties.SelectedHero, FigNet.Core.DeliveryMethod.Reliable, (value) =>
            {
                selectedHero = value;
                selectedHero.OnValueChange += SelectedHero_OnValueChange;

            });
        }



        if (networkEntity.IsMine)
        {
            UserName = new FNString();
            networkEntity.SetNetProperty<FNString>((byte)NetworkProperties.UserName, UserName, FigNet.Core.DeliveryMethod.Reliable);
            UserName.Value = AdvertiseGame.UserName;
        }
        else
        {
            networkEntity.GetNetProperty<FNString>((byte)NetworkProperties.UserName, FigNet.Core.DeliveryMethod.Reliable, (value) =>
            {
                UserName = value;
                UserName.OnValueChange += UserName_OnValueChange;
                UserName_OnValueChange(value.Value);
            });
        }

    }

    private void UserName_OnValueChange(string obj)
    {
        FN.Logger.Info($"USerName Arrived: {obj}");
        user_name.text = obj;
    }

    private void Room_OnPlayerJoined(NetPlayer obj)
    {
        if (!obj.IsMine)
        {
            // using this logic to get the selected hero of other players (who are already in the room)
            var player = KernNetzEntityManager.GetEntangleViewById(obj.NetworkId);

            player.NetworkEntity.GetNetProperty<FNShort>((byte)NetworkProperties.SelectedHero, FigNet.Core.DeliveryMethod.Reliable, (val) => {

                // here you can change the skin of hero
                Debug.Log($"Selected Hero: {val.Value} :: PlayerId: {obj.NetworkId}");
            });
        }
    }

    private void SelectedHero_OnValueChange(short obj)
    {
        // using this logic to get the selected hero of other players (who just entered the room)
        if (!networkEntity.IsMine)
        {
            // here you can change the skin of hero
            Debug.Log($"Selected Hero: {obj} :: PlayerId: {networkEntity.NetworkId}");
        }
    }
}

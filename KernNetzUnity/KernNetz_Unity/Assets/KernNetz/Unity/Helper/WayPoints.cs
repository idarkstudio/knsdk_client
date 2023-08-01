using FigNet.KernNetz;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    public Transform[] waypoints;

    void Start()
    {
        Room.OnPlayerJoined += Room_OnPlayerJoined;
    }

    private void Room_OnPlayerJoined(NetPlayer player)
    {
        if (player.IsMine)
        {
            // here call the get player logic
        }
    }
}

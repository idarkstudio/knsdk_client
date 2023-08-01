using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KernNetzContainer", menuName = "KernNetz/KernNetzContainer", order = 1)]
public class KernNetzContainer : ScriptableObject
{
    public KernNetzView MyPlayer;
    public KernNetzView RemotePlayer;
    public List<KernNetzView> Entities;
}


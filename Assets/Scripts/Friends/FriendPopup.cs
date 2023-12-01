using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class FriendPopup : Popup
{
    [SerializeField] private FriendWindow _friendWindow;
    public override void Setup()
    {
        base.Setup();
        _friendWindow.Setup();
    }
}

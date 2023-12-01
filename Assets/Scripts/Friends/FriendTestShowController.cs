using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class FriendTestShowController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var popup =PopupManager.ShowPopup(PopupManager.PopupName.Friend);
    }

}

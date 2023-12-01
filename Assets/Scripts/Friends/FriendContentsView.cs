using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendContentsView : MonoBehaviour
{
    [SerializeField] private TMP_Text _friendName;
    [SerializeField] private TMP_Text _friendMessage;

    public void Setup(FriendData friendData)
    {
        _friendName.text = friendData.FriendName;
        _friendMessage.text = friendData.FriendMessage;
        
    }
}

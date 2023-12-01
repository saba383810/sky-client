using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendWindow : MonoBehaviour
{
    [SerializeField] private Transform _friendcontentTransform;
    
    public void Setup()
    {
        var friendList = DataManager.Instance.FriendDataManager.FriendDatas;
        GameObject contents_obj = (GameObject)Resources.Load("Prefabs/Friend/FriendList");
        foreach(var friendData in friendList)
        {
            var obj =　Instantiate(contents_obj, _friendcontentTransform.transform);
            obj.GetComponent<FriendContentsView>().Setup(friendData);
        }
    }
}

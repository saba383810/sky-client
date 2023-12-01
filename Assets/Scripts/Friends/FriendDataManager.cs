using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendDataManager
{
    public readonly List<FriendData> FriendDatas = new()
    {
        new FriendData(777,"ツムツムマニアりゅうちゃん","刑務作業",888),
        new FriendData(999,"北斗の拳マニア田中","目指せ毎日万枚",111),
        new FriendData(999,"眼鏡マニア松本","眼鏡には魂が宿る",111),
    };
}

public class FriendData 
{
    public FriendData(int friendId,string friendName,string friendMessage,int hakoniwaId)
    {
        FriendId = friendId;
        FriendName = friendName;
        FriendMessage = friendMessage;
        HakoniwaId = hakoniwaId;
    }

    public int FriendId;
    public string FriendName;
    public string FriendMessage;
    public int HakoniwaId;
}


using PlayerData;
using Sabanogames.AudioManager;
using UnityEngine;

/// <summary>
/// すべてのローカルデータを保持するシングルトンクラス
/// </summary>
public class DataManager : SingletonMonoBehaviour<DataManager>
{
    public PlayerDataManager PlayerDataManager { get; set; }
    public MissionDataManager MissionDataManager { get; set; }
    public FriendDataManager FriendDataManager { get; set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        new GameObject("DataManager", typeof(DataManager));
    }
    
    protected override void Init()
    {
        base.Init();
        Debug.Log("DataManager 初期化");
        PlayerDataManager = new PlayerDataManager();
        MissionDataManager = new MissionDataManager();
        FriendDataManager = new FriendDataManager();
        DontDestroyOnLoad(gameObject);
    }
}


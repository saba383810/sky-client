using UnityEngine;
using UnityEngine.UI;

public class MissionWindow : MonoBehaviour
{
    [SerializeField] private MissionContentView FishfirstMission =default;
    [SerializeField] private MissionContentView AutoRunfirstMission =default;
    [SerializeField] private MissionContentView FishingTotalpoint =default;
    [SerializeField] private MissionContentView AutoRunTotalpoint =default;

    private int FishfirstMission_maxvalue = 1;
    private int AutoRunfirstMission_maxvalue = 1;
    private int FishingTotalpoint_maxvalue_1 = 10000;
    private int AutoRunTotalpoint_maxvalue_1 = 500;

    public enum MissionClearStatus
    {
        Get,
        Clear,
        sinkoutyu
    }
    // Start is called before the first frame update
    public void Setup()
    {
        var missionDataManager = DataManager.Instance.MissionDataManager;
        FishfirstMission.Setup("初めて釣りゲームをプレイした","1000",missionDataManager.FishingCount,FishfirstMission_maxvalue,missionDataManager.ClearStatusArray[0]);
        AutoRunfirstMission.Setup("フルーツピッキングを初めてプレイした","iphone",missionDataManager.AutorunCount,AutoRunfirstMission_maxvalue,missionDataManager.ClearStatusArray[1]);
        FishingTotalpoint.Setup("釣りで累計スコア10000点を達成する","ペタグー",missionDataManager.FishingTotalpoint,FishingTotalpoint_maxvalue_1,missionDataManager.ClearStatusArray[2]);
        AutoRunTotalpoint.Setup("フルーツピッキングで累計500点を達成する","ピングー",missionDataManager.AutorunTotalpoint,AutoRunTotalpoint_maxvalue_1,missionDataManager.ClearStatusArray[3]);
    }
}

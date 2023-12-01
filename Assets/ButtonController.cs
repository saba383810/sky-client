using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private GameObject _clearmission;
    private MissionDataManager _missionDataManager;
    
    // Start is called before the first frame update
    // Update is called once per frame
    public void MissionButtonPush()
    {
        _missionDataManager = DataManager.Instance.MissionDataManager;
        var status = MissionWindow.MissionClearStatus.Clear;
        Debug.Log(status);
        Destroy(_clearmission.gameObject);

    }
}

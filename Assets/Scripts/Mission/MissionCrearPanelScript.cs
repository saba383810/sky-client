using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionCrearPanelScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _reward;
    // Start is called before the first frame update
    void Start()
    {
        _reward = MissionContentView.missionReward;              
    }
    
    public void CompleteButtonPush()
    {
        Destroy(this.gameObject);
    }
}

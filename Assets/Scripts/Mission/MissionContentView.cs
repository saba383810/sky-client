using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class MissionContentView : MonoBehaviour
{
   [SerializeField] private TMP_Text _descriptionText;
   [SerializeField] private TMP_Text _missionScoText;
   [SerializeField] private TMP_Text _mission_reward;
   [SerializeField] private Slider _slider;
   [SerializeField] private Button _missionstatus;
   public static TMP_Text missionReward;

   public void Setup(string text,string missionReward,int num,int maxvalue,MissionWindow.MissionClearStatus status)
   {
      _descriptionText.text = text;
      _mission_reward.text = "報酬: "+missionReward;
      _missionScoText.text = num + "/" + maxvalue;
      _slider.maxValue = maxvalue;
      _slider.value = num;
      _missionstatus.enabled = true;
      switch (status)//Missionの状態を管理
      {
         case MissionWindow.MissionClearStatus.Clear:
            
            break;
         case MissionWindow.MissionClearStatus.sinkoutyu:
            
            break;
         case MissionWindow.MissionClearStatus.Get:
            break;
      }
      
      if (maxvalue <= num)
      {
         _missionstatus.enabled = true;
         _missionstatus.gameObject.SetActive(true);
      }
      
   }
}

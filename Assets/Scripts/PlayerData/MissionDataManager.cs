using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDataManager

{
   public MissionDataManager()
   {
      Debug.Log("MissionDataの初期化");
      FishingCount = 1;
      AutorunCount = 0;
      FishingTotalpoint = 10000; //釣りゲーム累計スコア
      AutorunTotalpoint = 300; //オートラン累計スコア
      ClearStatusArray = new [] { MissionWindow.MissionClearStatus.sinkoutyu,MissionWindow.MissionClearStatus.sinkoutyu,MissionWindow.MissionClearStatus.sinkoutyu,MissionWindow.MissionClearStatus.Clear};
   }

   public int FishingCount { get; set; }
   public int AutorunCount { get; set; }
   public int FishingTotalpoint { get; set; }
   public int AutorunTotalpoint { get; set; }
   public readonly MissionWindow.MissionClearStatus[] ClearStatusArray;
}

using UnityEngine;

namespace Common.PopupWindow
{
    public class MissionPopup : Popup
    {
        [SerializeField] private MissionWindow missionWindow;
        public override void Setup()
        {
            base.Setup();
            missionWindow.Setup();
        }
    }
}

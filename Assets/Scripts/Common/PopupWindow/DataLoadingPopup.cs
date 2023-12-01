using UnityEngine;

namespace Common.PopupWindow
{
    public class DataLoadingPopup : Popup
    {
        public override void Setup()
        {
            Debug.Log("DataLoadingPopup表示");
            base.Setup();
            Show();
        }
    }
}

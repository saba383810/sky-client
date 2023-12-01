using Common;
using UnityEngine;

namespace Title
{
    public class LoginPopup : Popup
    {
        public override void Setup()
        {
            base.Setup();
            Debug.Log("LoginPopup");
            Show();
        }
    }
}

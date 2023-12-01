using System;
using Common;
using Common.Loading;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.SideBar
{
    public class SettingPopup : Popup
    {
        [SerializeField] private Button gotoTitleButton = default;
        public override void Setup()
        {
            base.Setup();
            Bind();
        }

        private void Bind()
        {
            gotoTitleButton.onClick.AddListener(()=>
            {
                LoadingManager.ChangeScene("Title");
            });
        }
    }
}

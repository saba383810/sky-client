using Common.Loading;
using Playfab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Common.PopupWindow
{
    public class RegisterPlayerNamePopup : Popup
    {
        [SerializeField] private Button registerButton = default;
        [SerializeField] private TMP_InputField nameText = default;

        public override void Setup()
        {
            base.Setup();
            Bind();
        }

        private void Bind()
        {
            registerButton.onClick.AddListener(RegisterName);
        }

        private async void RegisterName()
        {
            var loading = PopupManager.ShowPopup(PopupManager.PopupName.DataLoading);
            var status = await PlayFabLogin.RegisterUserName(nameText.text);

            if (status == PlayFabLogin.LoginStatusType.Success)
            {
                var nameStatus = await PlayFabPlayerData.UpdateStatusData();
                if (nameStatus == PlayFabPlayerData.PlayerDataStatus.Error)
                {
                    PopupManager.ShowErrorPopup("名前を登録できませんでした");
                }
                LoadingManager.ChangeScene("OutGame");
            }
            else if (status == PlayFabLogin.LoginStatusType.Error)
            {
                loading.Hide();
                Debug.Log("エラー");
                PopupManager.ShowErrorPopup("名前を登録できませんでした");
            }
        }
    }
}

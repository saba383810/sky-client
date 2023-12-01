using Common;
using Common.Loading;
using Playfab;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class LoginCheckPopup : Popup
    {
        [SerializeField] private Button newGameStartButton = default;
        [SerializeField] private Button loginButton = default;
        public override void Setup()
        {
            base.Setup();
            Debug.Log("login check Window show");
            Bind();
            Show();
        }

        private void Bind()
        {
            newGameStartButton.onClick.AddListener(DataLoading);
            loginButton.onClick.AddListener(()=> PopupManager.ShowPopup(PopupManager.PopupName.Login));
        }

        public async void DataLoading()
        {
            Debug.Log("新しいデータで始める");
            var loading = PopupManager.ShowPopup(PopupManager.PopupName.DataLoading);
            var status = await PlayFabLogin.CustomIdLogin();
            loading.Hide();
            switch (status)
            {
                //通常のログイン処理
                case PlayFabLogin.LoginStatusType.Success:
                    LoadingManager.ChangeScene("OutGame");
                    break;
                // 名前が登録されていない場合
                case PlayFabLogin.LoginStatusType.Rename:
                    PopupManager.ShowPopup(PopupManager.PopupName.RegisterPlayerName);
                    break;
            }
        }

    }
}

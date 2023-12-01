using System;
using Common;
using Common.Loading;
using Cysharp.Threading.Tasks;
using Playfab;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private FloatingIslandView floatingIslandView = default;
        [SerializeField] private Button touchArea = default;
    
        private　void Start()
        {
            BGMManager.Instance.Play(BGMPath.TITLE);
            floatingIslandView.Setup();
            Bind();
        }

        private void Bind()
        {
            touchArea.onClick.AddListener(LoginCheck);
        }

        /// <summary>
        /// ログインチェック処理
        /// </summary>
        private async void LoginCheck()
        {
            Debug.Log("ログインチェック");
            var loading = PopupManager.ShowPopup(PopupManager.PopupName.DataLoading);
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerData-PlayerCustomId")))
            {
                loading.Hide();
                PopupManager.ShowPopup(PopupManager.PopupName.LoginCheck);
            }
            else
            {
                Debug.Log($"ログイン開始\nPlayerId : {PlayerPrefs.GetString("PlayerData-PlayerCustomId")}");
                var status = await PlayFabLogin.CustomIdLogin();
                if (status == PlayFabLogin.LoginStatusType.Success)
                {
                    LoadingManager.ChangeScene("OutGame");
                }
                else
                {
                    if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerData-PlayerName")))
                    {
                        loading.Hide();
                        PopupManager.ShowPopup(PopupManager.PopupName.RegisterPlayerName);
                    }
                    else
                    {
                        //Error Popup
                        loading.Hide();
                        PopupManager.ShowErrorPopup("ログインできませんでした");
                    }
                }
            }
        }
    }
}

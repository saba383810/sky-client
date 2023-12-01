using System;
using Common;
using Cysharp.Threading.Tasks;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using UnityEngine;

namespace OutGame
{
    public class OutGameManager : MonoBehaviour
    {
        [SerializeField] private BottomMenuBar bottomMenuBar = default;
        [SerializeField] private UserInformationView userInformationView;
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private async void Start()
        {
            if (!DataManager.Instance.PlayerDataManager.InitDone)
            {
                PopupManager.ShowGoToTitleError("データを初期化していません");
                return;
            }
            BGMManager.Instance.Play(BGMPath.OUT_GAME);

            userInformationView.Setup();
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
            bottomMenuBar.Setup();
        }
    }
}

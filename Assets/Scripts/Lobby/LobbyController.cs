using Common;
using Common.Loading;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using UniRx;
using UnityEngine;

namespace Lobby
{
    public class LobbyController : MonoBehaviour
    {
        [SerializeField] private CommonButton joinButton;
        [SerializeField] private CommonButton exitButton;
        [SerializeField] private BasicSpawner spawner;
        [SerializeField] private GameObject worldSelectWindow;
        [SerializeField] private GameObject chatUI;
        
        private void Start()
        {
            BGMManager.Instance.Play(BGMPath.LOBBY);
            Setup();
            chatUI.gameObject.SetActive(false);
            worldSelectWindow.SetActive(true);
        }

        public void Setup()
        {
            Bind();
        }

        private void Bind()
        {
            exitButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(_ => LoadingManager.ChangeScene("OutGame"));
            joinButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(_ => WorldInit());
        }

        private void WorldInit()
        {
            var loading = PopupManager.ShowPopup(PopupManager.PopupName.DataLoading);
            spawner.StartGame();
            
            worldSelectWindow.SetActive(false);
            chatUI.gameObject.SetActive(true);
            loading.Hide();
            

        }
    }
}

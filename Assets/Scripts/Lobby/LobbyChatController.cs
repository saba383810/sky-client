using Fusion;
using TMPro;
using UniRx;
using UnityEngine;

namespace Lobby
{
    public class LobbyChatController : NetworkBehaviour
    { 
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private CommonButton sendButton;
        [SerializeField] private GameObject chatCellPrefab;
        [SerializeField] private Transform chatCellRoot;
        private GameObject _player;
        private void Start()
        { 
            Bind();
        }

        public void Bind()
        {
            sendButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(_ => SendChat());
        }

        private void SendChat()
        {
            var playerId = DataManager.Instance.PlayerDataManager.PlayerPhotonId;
            var playerName = DataManager.Instance.PlayerDataManager.PlayerName;
            var inputMessage = inputField.text;
            if(string.IsNullOrEmpty(inputMessage)) return;
            inputField.text = "";
            if (_player == null) _player = GameObject.Find($"Player_{playerId}");
            _player.GetComponent<PlayerController>().RPC_SendChat(playerId,playerName,inputMessage);
        }

        public void GenerateChat(string playerName,string message)
        {
            var chatCellView = Instantiate(chatCellPrefab, Vector3.zero, Quaternion.identity, chatCellRoot).GetComponent<ChatCellView>();
            chatCellView.Setup(playerName,message);
        }
    }
}

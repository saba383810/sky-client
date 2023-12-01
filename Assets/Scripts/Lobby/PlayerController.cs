using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using TMPro;
using UnityEngine;

namespace Lobby
{
    public class PlayerController : NetworkBehaviour
    {
        public enum MoveDirection
        {
            Right = 0,
            Left = 1,
        }
        [SerializeField] private TMP_Text nameText;
        [SerializeField] public ChatCellView headChatCellView;
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private DressUpSprite playerSprite;
        private LobbyChatController _chatController;
        private CancellationTokenSource _cts;
        private MoveDirection _currentMoveDirection;
        private const float MaxSpeed = 6.0f;
        private const float Force = 30f;
        private float _tmpPlayerPosX;
        private float _deltaPlayerPosX;
        private bool _isWalkAnimation;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            _currentMoveDirection = MoveDirection.Right;
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                Move();
            }
            CheckMoveAnimation();
        }

        private void Move()
        {
            var horizontal = UltimateJoystick.GetHorizontalAxis("Movement");
            if (rigidbody.velocity.x < MaxSpeed && rigidbody.velocity.x > -MaxSpeed) rigidbody.AddForce(horizontal * Force, 0, 0);
            if (horizontal < 0.1f && horizontal > -0.1f) rigidbody.velocity /= 1.1f;
            CheckMoveDirection();
        }

        private void CheckMoveDirection()
        {
            //向きのアニメーション
            if (rigidbody.velocity.x > 0.2f&& _currentMoveDirection != MoveDirection.Right)
            {
                _currentMoveDirection = MoveDirection.Right;
                playerSprite.transform.localRotation = Quaternion.Euler(0,0,0);
                RPC_ChangeDirection((int)MoveDirection.Right,DataManager.Instance.PlayerDataManager.PlayerPhotonId);
            }

            if (rigidbody.velocity.x < -0.2f && _currentMoveDirection != MoveDirection.Left)
            {
                _currentMoveDirection = MoveDirection.Left;
                playerSprite.transform.localRotation = Quaternion.Euler(0,180,0);
                RPC_ChangeDirection((int)MoveDirection.Left,DataManager.Instance.PlayerDataManager.PlayerPhotonId);
            }
        }

        private void CheckMoveAnimation()
        {
            //歩きモーション
            var currentPosX = transform.position.x;
            _deltaPlayerPosX += _tmpPlayerPosX - currentPosX;
            Debug.Log($"deltaPosX{_deltaPlayerPosX}");
            _tmpPlayerPosX = currentPosX;
            if (Mathf.Abs(_deltaPlayerPosX) > 0.6f)
            {
                WalkAnimation();
            }
        }
        
        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_ChangeDirection(int direction,string playerId)
        {
            if (playerId == DataManager.Instance.PlayerDataManager.PlayerCustomId) return;
            if (direction == (int)MoveDirection.Right)
            { 
                playerSprite.transform.localRotation = Quaternion.Euler(0,0,0);
            }
            if (direction == (int) MoveDirection.Left)
            {
                playerSprite.transform.localRotation = Quaternion.Euler(0,180,0);
            }
            
        }


        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_SetUserData(string playerId,string playerName,int hairId,int dressId,int accId)
        {
            gameObject.GetComponent<NetworkObject>().name = $"Player_{playerId}";
            nameText.text = playerName;
            playerSprite.Setup(hairId,dressId,accId);
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_SendChat(string playerId,string userName,string message)
        {
            if(_chatController==null) _chatController = GameObject.Find("LobbyChatController").GetComponent<LobbyChatController>();
            _chatController.GenerateChat(userName,message);
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            ShowChatWindow(playerId,userName,message,_cts.Token).Forget();
        }

        private async UniTaskVoid ShowChatWindow(string playerId,string userName,string message,CancellationToken token)
        {
            var cellView = GameObject.Find($"Player_{playerId}").GetComponent<PlayerController>().headChatCellView;
            // Layoutを更新するために何回かEnableにしたりSetしたりしています
            cellView.Setup(userName,message);
            cellView.gameObject.SetActive(true);
            cellView.gameObject.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            cellView.Setup(userName,message+"\n");
            DOTween.Sequence()
                .OnStart(() =>
                {
                    cellView.transform.localScale = Vector3.zero;
                    cellView.gameObject.SetActive(true);
                })
                .Append(cellView.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBounce));
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f),cancellationToken:token);
            cellView.Setup(userName,message);
            await UniTask.Delay(TimeSpan.FromSeconds(3),cancellationToken:token);
            if (!token.IsCancellationRequested)
            {
                cellView.gameObject.SetActive(false);
            }
        }

        private void WalkAnimation()
        {
            if (_isWalkAnimation) return;
            _isWalkAnimation = true;
            DOTween.Sequence()
                .Append(playerSprite.transform.DOLocalMoveY(0.1f, 0.2f))
                .Append(playerSprite.transform.DOLocalMoveY(0f, 0.1f))
                .OnComplete(()=>
                {
                    _isWalkAnimation = false;
                    _deltaPlayerPosX = 0;
                });
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
        }
    }
}

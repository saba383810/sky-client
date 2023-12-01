using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;

namespace MiniGame.NeoAutoRun
{
    public class NeoAutoRunCharacterController : MonoBehaviour
    {
        [SerializeField] private NeoAutoRunGameManager neoAutoRunGameManager;
        [SerializeField] private float characterJump;
        [SerializeField] private float characterAirJump;
        [SerializeField] private float characterAnimationJump;
        [SerializeField] private float characterAnimationDash;
        [SerializeField] private Transform characterSprite; 
        private Rigidbody2D _characterRb;
        private bool _isAirFlag;
        private bool _isAirJumpFlag;
        private bool _collisionFlag;

        private void Awake()
        {
            //初期化
            _collisionFlag = false;
            _isAirFlag = false;
        }

        private void Start()
        {
            StartCoroutine($"CharacterAnimation");
        }
        
        private IEnumerator CharacterAnimation()
        {
            while (true)
            {
                _characterRb = gameObject.GetComponent<Rigidbody2D>();
                if (Input.GetMouseButtonDown(0))
                {
                    var phase = neoAutoRunGameManager.GetAutoRunPhase();
                    if (phase == NeoAutoRunGameManager.AutoRunPhase.Started)
                    {
                        switch (_isAirFlag)
                        {
                            //地面にいるとき
                            case false:
                                _characterRb.velocity = new Vector2(0, characterJump);
                                _isAirFlag = true;
                                SEManager.Instance.Play(SEPath.AUTO_RUN_CHARACTER_JUMP_VOICE);
                                break;
                            //空中にいてジャンプをしてない時(2段ジャンプ)
                            case true when !_isAirJumpFlag:
                                _characterRb.velocity = new Vector2(0, characterAirJump);
                                transform.DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.FastBeyond360);   
                                _isAirFlag = true;
                                _isAirJumpFlag = true;
                                SEManager.Instance.Play(SEPath.AUTO_RUN_CHARACTER_JUMP_VOICE2);
                                break;
                        }
                    }
                    yield return new WaitUntil(() => !Input.GetMouseButton(0));
                }
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// スタート前に走るアニメーション
        /// </summary>
        /// <returns></returns>
        public async UniTask<bool> StartDashAnimation()
        {
            var position = characterSprite.transform.localPosition;
            //初期定位置まで移動（X:1Y:-3.8）
            DOTween.Sequence()
                .Append(characterSprite.DOLocalMoveX(1f, 1.6f));
            var positionY = position.y;
            for (var i = 0; i < 4;i++)
            {
               await characterSprite.DOLocalMoveY(position.y+characterAnimationJump, 0.2f).SetEase(Ease.OutQuad);
               await characterSprite.DOLocalMoveY(positionY, 0.2f).SetEase(Ease.OutQuad);
            }
            return true;
        }
        
        private async void OnCollisionStay2D(Collision2D col)
        {
            var phase = neoAutoRunGameManager.GetAutoRunPhase();
            if(phase!=NeoAutoRunGameManager.AutoRunPhase.Started) return;
            //キャラクターアニメーション
            if (!col.gameObject.CompareTag($"Ground")) return;
            if(_collisionFlag) return;
            var position = characterSprite.transform.localPosition;
            _collisionFlag = true;
            _isAirFlag = false;
            _isAirJumpFlag = false;
            await CharacterRun(position);
        }
        
        /// <summary>
        /// 果物か障害物かの判定
        /// </summary>
        /// <param name="col"></param>
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Fruit"))
            {
                
                neoAutoRunGameManager.AddScore(100);
                Destroy(col.gameObject);
            }else if (col.gameObject.CompareTag($"Obstacle"))
            {
                Destroy(col.gameObject);
                neoAutoRunGameManager.SubLife();
            }
        }

        /// <summary>
        /// キャラクターが走る処理
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private async UniTask<bool> CharacterRun(Vector3 position)
        {
            var positionY = position.y;
            await characterSprite.DOLocalMove(new Vector3(position.x, position.y+characterAnimationJump, 0), 0.2f).SetEase(Ease.OutQuad);
            await characterSprite.DOLocalMove(new Vector3(position.x, positionY, 0),0.2f).SetEase(Ease.OutQuad);
            _collisionFlag = false;
            return true;
        }

    }
}

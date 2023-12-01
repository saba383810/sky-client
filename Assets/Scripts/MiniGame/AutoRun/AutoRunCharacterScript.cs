using System.Collections;
using UnityEngine;
using DG.Tweening;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;

namespace MiniGame.AutoRun
{
    public class AutoRunCharacterScript : MonoBehaviour
    {
        [SerializeField] private AutoRunGameManager autoRunGameManager;
        [SerializeField] private PrefabsGenerator prefabsGenerator;
        [SerializeField] private MoveBackGround moveBackGround;
        [SerializeField] private float characterJump;
        [SerializeField] private float characterAirJump;
        [SerializeField] private float characterAnimationJump;
        private bool _isAirFlag;
        private bool _isAirJumpFlag;
        private Rigidbody2D _characterRb;
        private void Start()
        {
            //初期化
            _isAirFlag = false;
            StartCoroutine($"CharacterAnimation");
        }
        
        //キャラクター操作（ジャンプアニメーション）
        private IEnumerator CharacterAnimation()
        {
            while (true)
            {
                _characterRb = gameObject.GetComponent<Rigidbody2D>();
                if (Input.GetMouseButtonDown(0))
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

                    yield return new WaitUntil(() => !Input.GetMouseButton(0));
                }
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
        private bool _isAnimationStopFlag;//ゲーム終了flag
        private void OnCollisionEnter2D(Collision2D col)
        {
            if(_isAnimationStopFlag) return;
            _characterRb = gameObject.GetComponent<Rigidbody2D>();
            //オブジェクト生成（フルーツと障害物）
            if (col.gameObject.CompareTag($"Fruit"))
            {
                autoRunGameManager.AddScore(100);
                Destroy(col.gameObject);
            }else if (col.gameObject.CompareTag($"Obstacle"))
            {
                //ゲーム終了処理
                Destroy(col.gameObject);
                _isAnimationStopFlag = true;
                AnimationStop();
                ResultAnnouncement();
            }
            
            //キャラクターアニメーション
            if (!col.gameObject.CompareTag($"Ground")) return;
            _characterRb.velocity = new Vector2(0, characterAnimationJump);
            _isAirFlag = false;
            _isAirJumpFlag = false;
            
        }

        //アニメーション終了
        private void AnimationStop()
        {
            //動いてるオブジェくを止める
            prefabsGenerator.EnabledPrefabGeneratorStopFlag();
            moveBackGround.EnableMoveBackGroundStopFlag();
        }
        
        //結果表示
        private  void ResultAnnouncement()
        {
            autoRunGameManager.AutoRunGameEnd();
        }
    }
}

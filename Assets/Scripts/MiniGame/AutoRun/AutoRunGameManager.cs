using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MiniGame.NeoAutoRun;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace MiniGame.AutoRun
{
    public class AutoRunGameManager : MonoBehaviour
    {
        private int _score;//得点
        [SerializeField] private TMP_Text scoreText;//スコア
        [SerializeField] private GameObject canvas,gameEnd,reStartButton;
        [SerializeField] private PrefabsGenerator prefabsGenerator;
        //修正日(7/6)山口
        [SerializeField] private CountdownAnimation[] countdownAnimation; //countdownAnimationNo3, countdownAnimationNo2, countdownAnimationNo1,startAnimation
        [SerializeField] private MoveBackGround moveBackGround;

        private async void Start()
        {
            //初期化
            _score = 0;
            await CountdownStart();
            moveBackGround.EnableMoveBackGroundStopFlag();
            prefabsGenerator.PrefabGeneratorStart();
        }

        private async UniTask<bool> CountdownStart()
        {
            //カウントダウンのアニメーション
            foreach (var i in countdownAnimation)
            {
                await i.CountdownAnimationStart();
            }
            return true;
        }
        
        //スコア計算
        public void AddScore(int addScore)
       {
           _score += addScore;
           scoreText.text = (_score).ToString();
       }
        
        //ゲーム終了アニメーション
        public async void AutoRunGameEnd()
        {
            //ゲーム終了アニメーション
            await GameEndAnimation();
            //スコア表示アニメーション
            await ScoreResultAnnouncementAnimation();
            //Restart表示アニメーション
            await ReStartButtonAnimation();
        }

        private async UniTask<bool> GameEndAnimation()
        {
            SEManager.Instance.Play(SEPath.AUTO_RUN_WHISTLE);
            //画面真ん中に移動するアニメーション x軸-13
            await gameEnd.transform.DOMove(new Vector2(0,0),1);
            //画面左に移動するアニメーション x軸-2000
            await gameEnd.transform.DOMove(new Vector2(-13, 0), 1);
         
            gameEnd.GetComponent<Image>().DOFade(
                0f,     // フェード後のアルファ値
                1f      // 演出時間
            );      
            return true;
        }

        private async UniTask<bool> ScoreResultAnnouncementAnimation()
        {
            //スコアボードの生成
            var scoreResult = (GameObject)Resources.Load("Prefabs/AutoRun/ScoreResultWindow");
            var scoreResultObj= Instantiate(scoreResult, new Vector3(0, 1800, 0),Quaternion.identity) ;
            scoreResultObj.transform.SetParent(canvas.transform,false);
            //SCOREが降りてくるアニメーション
            await ScoreAnimation(scoreResultObj);
            return true;
        }

        //スコアアニメーションのシーケンス
        private Sequence ScoreAnimation(GameObject scoreResultObj)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(scoreResultObj.transform.DOLocalMove(new Vector3(0, 0, 0), 1f).SetDelay((0.5f))
                .OnComplete(() => SEManager.Instance.Play(SEPath.AUTO_RUN_RESULT_SE)));
            sequence.Append(scoreResultObj.transform.DOLocalMove(new Vector3(0, 10, 0), 0.1f));
            sequence.Append(scoreResultObj.transform.DOLocalMove(new Vector3(0, 0, 0), 0.1f));
            sequence.Append(scoreResultObj.transform.DOLocalMove(new Vector3(0, 10, 0), 0.1f));
            sequence.Append(scoreResultObj.transform.DOLocalMove(new Vector3(0, 0, 0), 0.1f));
            //スコア数値アニメーション(SCOREの数値を出す)
            var scoreResultText = scoreResultObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
            scoreResultText.text = scoreText.text;
            var startScore = 0;
            Tween scoreTween = null;
            DOTween.Kill(scoreTween);
            scoreTween = DOTween.To(() => startScore, (val) =>
            {
                startScore = val;
                scoreResultText.text = $"{val:#,0}";
            }, _score, 3f);
            return sequence;
        }

        private async UniTask<bool> ReStartButtonAnimation()
        {
            await reStartButton.transform.DOLocalMove(new Vector3(950, -100, 0), 1f);
            return true;
        }
    }
}

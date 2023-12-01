using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.NeoAutoRun
{
    public class NeoAutoRunGameManager : MonoBehaviour
    {
        [SerializeField] private GameObject canvas,restartButton,exitButton;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private BackGroundController backGroundController;
        [SerializeField] private NeoAutoRunCharacterController neoAutoRunCharacterController;
        [SerializeField] private FruitsOrObstaclesController fruitsOrObstaclesController;
        [SerializeField] private LifeController lifeController;
        [SerializeField] private NeoAutoRunPrefabsGenerator neoAutoRunPrefabsGenerator;
        private AutoRunPhase _autoRunPhase;

        private void Awake()
        {
            //初期化
            scoreText.text = "0";
        }

        /// <summary>
        /// ゲーム開始前、開始中、開始後でわける
        /// </summary>
        public enum AutoRunPhase
        {
            NotStart,
            Started,
        }
        
        public AutoRunPhase GetAutoRunPhase()
        {
            return _autoRunPhase;
        }
        
        private async void Start()
        {
            //データの初期化
            //カウントダウンの処理
            await neoAutoRunCharacterController.StartDashAnimation();
            //await CountdownStart();
            //スタートしたら背景を動かす
            _autoRunPhase = AutoRunPhase.Started;
            backGroundController.BackGroundMoveStart();
            RunAddScore();
            //障害物とフルーツ生成スタート
            //fruitsOrObstaclesController.FruitsOrObstaclesControllerStart();
            neoAutoRunPrefabsGenerator.NeoAutoRunPrefabsGeneratorStart();
            //TODO  BGMスタート
            BGMManager.Instance.Play(BGMPath.AUTO_RUN);
            //TODO  ここにもタスクがあるよ
        }
        
        /// <summary>
        /// カウントダウンの生成と呼び出し
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> CountdownStart()
        {
            // カウントダウンの画像を読み込む
            var countdownNo3 = (GameObject)Resources.Load("Prefabs/NeoAutoRun/Countdown/No.3");
            var countdownNo2 = (GameObject)Resources.Load("Prefabs/NeoAutoRun/Countdown/No.2");
            var countdownNo1 = (GameObject)Resources.Load("Prefabs/NeoAutoRun/Countdown/No.1");
            var countdownStart = (GameObject)Resources.Load("Prefabs/NeoAutoRun/Countdown/Start");

            // カウントダウンを生成
            var countdownList = new GameObject[]
            {
                Instantiate(countdownNo3, new Vector3(1800, -115, 0), Quaternion.identity),
                Instantiate(countdownNo2, new Vector3(1800, -115, 0), Quaternion.identity),
                Instantiate(countdownNo1, new Vector3(1800, -115, 0), Quaternion.identity),
                Instantiate(countdownStart, new Vector3(1800, -115, 0), Quaternion.identity),
            };

            // カウントダウンの親子関係を設定
            foreach (var countdownObj in countdownList)
            {
                countdownObj.transform.SetParent(canvas.transform, false);
            }

            // カウントダウンのアニメーション開始
            foreach (var countdownObj in countdownList)
            {
                var countdownAnimation = countdownObj.GetComponent<CountdownAnimation>();
                await countdownAnimation.CountdownAnimationStart();
            }

            Debug.Log("カウントダウンの処理が完了しました");
            return true;
        }


        /// <summary>
        /// スコアの計算
        /// </summary>
        private int _score;
        public void AddScore(int addScore)
        {
            _score += addScore;
            scoreText.text = (_score).ToString();
        }
        
        private async void RunAddScore()
        {
            while (_autoRunPhase == AutoRunPhase.Started)
            {
                AddScore(10);
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// ライフ管理、死んだらゲーム終了
        /// </summary>
        public void SubLife()
        {
            //falseだったばあいゲーム終了
            if(lifeController.IsGameEnd()) 
                NeoAutoRunGameEnd();
        }

        /// <summary>
        /// ゲーム終了の処理
        /// </summary>
        private async void NeoAutoRunGameEnd()
        {
            //ゲーム終了
            _autoRunPhase = AutoRunPhase.NotStart;
            fruitsOrObstaclesController.GameEndPrefabsDestroy();
            //ゲーム終了アニメーション
            await GameEndAnimation();
            //スコア表示アニメーション
            await ScoreResultAnnouncementAnimation();
            //Restart表示アニメーション
            await ReStartOrGoHomeButtonAnimation();
            Debug.Log("ゲームが終了しました");
            await ChangeScoreIntoCoin();

        }
        
        /// <summary>
        /// ゲーム終了時のアニメーション
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> GameEndAnimation()
        {
            //一度点滅が入る
            SEManager.Instance.Play(SEPath.AUTO_RUN_WHISTLE);
            var gameEnd = (GameObject)Resources.Load("Prefabs/NeoAutoRun/GameEnd");
            var gameEndObj = Instantiate(gameEnd, new Vector3(1800, -115, 0), Quaternion.identity);
            gameEndObj.transform.SetParent(canvas.transform,false);
            //画面真ん中に移動するアニメーション x軸-13
            await gameEndObj.transform.DOMove(new Vector2(0,0),1);
            //画面左に移動するアニメーション x軸-2000
            await gameEndObj.transform.DOMove(new Vector2(-13, 0), 1);
            
            gameEnd.GetComponent<Image>().DOFade(
                0f,     // フェード後のアルファ値
                1f      // 演出時間
            );      
            Destroy(gameEndObj);
            return true;
        }

        /// <summary>
        /// スコアアニメーション
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> ScoreResultAnnouncementAnimation()
        {
            //スコアボードの生成
            var scoreResult = (GameObject)Resources.Load("Prefabs/NeoAutoRun/ScoreResultWindow");
            var scoreResultObj= Instantiate(scoreResult, new Vector3(0, 1800, 0),Quaternion.identity);
            scoreResultObj.transform.SetParent(canvas.transform,false);
            //SCOREが降りてくるアニメーション
            await ScoreAnimation(scoreResultObj);
            return true;
        }

        //スコアアニメーションのシーケンス
        private async UniTask<bool> ScoreAnimation(GameObject scoreResultObj)
        {
            // スコアアニメーションのシーケンス
            DOTween.Sequence()
                .Append(scoreResultObj.transform.DOLocalMove(new Vector3(0, 0, 0), 1f).SetDelay((0.5f))
                    .OnComplete(() => SEManager.Instance.Play(SEPath.AUTO_RUN_RESULT_SE)))
                .Append(scoreResultObj.transform.DOLocalMove(new Vector3(0, 10, 0), 0.1f))
                .Append(scoreResultObj.transform.DOLocalMove(new Vector3(0, 0, 0), 0.1f))
                .Append(scoreResultObj.transform.DOLocalMove(new Vector3(0, 10, 0), 0.1f))
                .Append(scoreResultObj.transform.DOLocalMove(new Vector3(0, 0, 0), 0.1f));

            // スコア数値アニメーション
            var scoreResultText = scoreResultObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
            var startScore = 0;
            Tween scoreTween = null;
            DOTween.Kill(scoreTween);
            scoreTween = DOTween.To(() => startScore, (val) =>
            {
                startScore = val;
                scoreResultText.text = $"{val:#,0}";
            }, _score, 3f);

            return true;
        }
        
        /// <summary>
        /// ゲームリスタートボタン
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> ReStartOrGoHomeButtonAnimation()
        {
            DOTween.Sequence()
                .Append(restartButton.transform.DOLocalMove(new Vector3(950, 0, 0), 1f))
                .Append(exitButton.transform.DOLocalMove(new Vector3(950, -350, 0), 1f));
            return true;
        }
        
        /// <summary>
        /// ポーズの処理
        /// </summary>
        public void PauseTime()
        {
            if (_autoRunPhase == AutoRunPhase.Started)
            {
                _autoRunPhase = AutoRunPhase.NotStart;
                Time.timeScale = 0;
            }
            else
            {
                _autoRunPhase = AutoRunPhase.Started;
                Time.timeScale = 1;
                backGroundController.BackGroundMoveStart();
            }
        }
        
        /// <summary>
        /// スコアをコインに変更する
        /// </summary>
        private async UniTask<bool> ChangeScoreIntoCoin()
        {
            //スコアを取得
            var score =  int.Parse(scoreText.text);
            Debug.Log("コインを追加しました");
            return true;
        }
    }
}

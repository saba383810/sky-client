using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace MiniGame.NeoAutoRun
{
    public class BackGroundController : MonoBehaviour
    {
        [SerializeField] private NeoAutoRunGameManager neoAutoRunGameManager;
        //スピードと位置
        [SerializeField] private float moveSpeedTree = -0.15f;
        [SerializeField] private float moveSpeedGround = -0.3f;
        [SerializeField] private float moveSpeedBand = -0.1f;
        [SerializeField] private float moveSpeedCloud = 0.1f;
        [SerializeField] private Transform treeTransform;
        [SerializeField] private Transform treeTransform2;
        [SerializeField] private Transform groundTransform;
        [SerializeField] private Transform groundTransform2;
        [SerializeField] private Transform bandTransform;
        [SerializeField] private Transform bandTransform2;
        //繰り返す位置
        [SerializeField] private float treeReStartTransformX = 34;
        [SerializeField] private float groundReStarTransformX = 34;
        [SerializeField] private float bandReStartTransformX = 34;
        //最後尾
        [SerializeField] private float treeEnd = -21;
        [SerializeField] private float groundEnd = -21;
        [SerializeField] private float bandEnd = -21;
        //スクロールでまわす(RawImage)
        [SerializeField] private RawImage cloud = null;

        //スピード配列
        private float[] _moveSpeeds;
        //背景座標配列
        private Transform[] _treeTransforms;
        private Transform[] _groundTransforms;
        private Transform[] _bandTransforms;

        private bool _isMoveBackGroundStop; 
        public void BackGroundMoveStart()
        {
            //スピード配列(木の速さ、地面の速さ、山の速さ)
            _moveSpeeds = new float[] { moveSpeedTree, moveSpeedGround, moveSpeedBand };
            //座標配列(木、地面、山)
            _treeTransforms = new Transform[] {treeTransform, treeTransform2};
            _groundTransforms = new Transform[] {groundTransform, groundTransform2};
            _bandTransforms = new Transform[] { bandTransform, bandTransform2 };
            BackGroundMove(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask BackGroundMove(CancellationToken token)
        {
            var phase = neoAutoRunGameManager.GetAutoRunPhase();
            Debug.Log(phase);
            while (phase == NeoAutoRunGameManager.AutoRunPhase.Started && !token.IsCancellationRequested)
            {
                //座標を移動
                _treeTransforms.ToList().ForEach(treeTransformsList => treeTransformsList.Translate(_moveSpeeds[0], 0, 0));
                _groundTransforms.ToList().ForEach(groundTransformList => groundTransformList.Translate(_moveSpeeds[1], 0, 0));
                _bandTransforms.ToList().ForEach(bandTransformsList => bandTransformsList.Translate(_moveSpeeds[2], 0, 0));
            
                //画面外にでたか
                if (treeTransform.position.x < treeEnd)
                    treeTransform.position = new Vector2(treeReStartTransformX, treeTransform.position.y);
                if (treeTransform2.position.x < treeEnd)
                    treeTransform2.position = new Vector2(treeReStartTransformX, treeTransform2.position.y);
                if (groundTransform.position.x < groundEnd)
                    groundTransform.position = new Vector2(groundReStarTransformX, groundTransform.position.y);
                if (groundTransform2.position.x < groundEnd)
                    groundTransform2.position = new Vector2(groundReStarTransformX, groundTransform2.position.y);
                if (bandTransform.position.x < bandEnd)
                    bandTransform.position = new Vector2(bandReStartTransformX, bandTransform.position.y);
                if (bandTransform2.position.x < bandEnd)
                    bandTransform2.position = new Vector2(bandReStartTransformX, bandTransform2.position.y);
                await UniTask.Yield(token);
                
                //rawImageの処理
                

                phase = neoAutoRunGameManager.GetAutoRunPhase();
            }
        }


        
    }
}

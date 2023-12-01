using System;
using System.Linq;
using MiniGame.NeoAutoRun;
using Unity.VisualScripting;
using UnityEngine;


namespace MiniGame.AutoRun
{
    public class MoveBackGround : MonoBehaviour
    {
        [SerializeField] private float moveSpeedTree = -0.15f;
        [SerializeField] private float moveSpeedGround = -0.3f;
        [SerializeField] private float moveSpeedBand = -0.1f;
        [SerializeField] private Transform treeTransform;
        [SerializeField] private Transform treeTransform2;
        [SerializeField] private Transform groundTransform;
        [SerializeField] private Transform groundTransform2;
        [SerializeField] private Transform bandTransform;
        [SerializeField] private Transform bandTransform2;
        //繰り返す位置
        [SerializeField] private float treeReStartTransformX;
        [SerializeField] private float groundReStarTransformX;
        [SerializeField] private float bandReStartTransformX;
        //最後尾
        [SerializeField] private float treeEnd;
        [SerializeField] private float groundEnd;
        [SerializeField] private float bandEnd;
        
        //スピード配列
        private float[] _moveSpeeds;
        //背景座標配列
        private Transform[] _treeTransforms;
        private Transform[] _groundTransforms;
        private Transform[] _bandTransforms;
        private bool _isMoveBackGroundStop;
        private void Start()
        {
            //修正日(7/6)山口
            //スピード配列(木の速さ、地面の速さ、山の速さ)
            _moveSpeeds = new float[] { moveSpeedTree, moveSpeedGround, moveSpeedBand };
            //座標配列(木、地面、山)
            _treeTransforms = new Transform[] {treeTransform, treeTransform2};
            _groundTransforms = new Transform[] {groundTransform, groundTransform2};
            _bandTransforms = new Transform[] { bandTransform, bandTransform2 };
            //オブジェクト配列（木、背景の山、）
            _isMoveBackGroundStop = true;
        }

        private void Update()
        { 
            Move();
        }
        private void Move()
        {
            if(_isMoveBackGroundStop) return;
            //座標を移動　修正日(7/6)山口
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
            
            
        }

        private bool _isGameStartFlag;
        private bool _isGameEndFlag;
        public void EnableMoveBackGroundStopFlag()
        {
            if (!_isGameStartFlag)
            {
                _isMoveBackGroundStop = !_isMoveBackGroundStop;
                _isGameStartFlag = !_isGameStartFlag;
            }else if (!_isGameEndFlag)
            {
                _isMoveBackGroundStop = !_isMoveBackGroundStop;
                _isGameEndFlag = !_isGameEndFlag;
            }
        }
    }
}

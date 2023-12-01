using DG.Tweening;
using UnityEngine;

namespace Hakoniwa
{
    public class HakoniwaObjData : MonoBehaviour
    {
        //配列番号を保存
        public int PosX { get; set; }
        public int PosY { get; set; }
        
        // 家具ID
        public int FurnitureID { get; set; }
        //家具の向き
        public int Direction { get; set; }
        
        //アニメーションを実行中かを確認。
        private bool _isAnimation = false;
        public void ClickedAnimation()
        {
            if (_isAnimation) return;
            _isAnimation = true;
            var tmpTransform = transform;
            var currentPosY = tmpTransform.localPosition.y;
            var currentScale = tmpTransform.localScale;
            DOTween.Sequence()
                .Append(transform.DOLocalMoveY(currentPosY + 0.1f, 0.2f))
                .Join(transform.DOScale(currentScale + new Vector3(0.1f, 0.1f, 0.1f), 0.2f))
                .Append(transform.DOLocalMoveY(currentPosY, 0.2f))
                .Join(transform.DOScale(currentScale, 0.2f))
                .OnComplete(()=>_isAnimation = false);
        }
    }
}

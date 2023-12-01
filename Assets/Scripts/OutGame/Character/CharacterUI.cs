using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OutGame.Character
{
    public class CharacterUI : MonoBehaviour
    {
        /// <summary>目を閉じたobj</summary>
        [SerializeField] private CanvasGroup eyesCloseCanvasGroup = default;

        private Sequence _jumpSequence;
        private Sequence _eyeSequence;
        private void Start()
        {
            var charaTransform = transform;
            var charaLocalPos = charaTransform.localPosition;
            var charaLocalScale = charaTransform.localScale;
        
            //ジャンプアニメーション
             _jumpSequence = DOTween.Sequence()
                .Append(transform.DOLocalMoveY(charaLocalPos.y + 150, 0.3f))
                .Join(transform.DOScale(charaLocalScale + new Vector3(0, 0.1f, 0), 0.3f))
                .Join(transform.DORotate(new Vector3(0,520,0),0.3f,RotateMode.FastBeyond360))
                .Append(transform.DOLocalMoveY(charaLocalPos.y, 0.2f))
                .Join(transform.DOScale(charaLocalScale, 0.2f))
                .Join(transform.DORotate(new Vector3(0,0,0),0.2f))
                .AppendInterval(Random.Range(5,10)).SetLoops(-1);
        
            //瞬きアニメーション
            _eyeSequence =  DOTween.Sequence()
                .Append(eyesCloseCanvasGroup.DOFade(1, 0.2f))
                .AppendInterval(Random.Range(0.05f, 0.5f))
                .Append(eyesCloseCanvasGroup.DOFade(0, 0.2f))
                .AppendInterval(Random.Range(2f, 4f))
                .SetLoops(-1);

        }

        private void OnDestroy()
        {
            _jumpSequence.Kill();
            _eyeSequence.Kill();
        }
    }
}

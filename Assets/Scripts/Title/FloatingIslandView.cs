using System;
using DG.Tweening;
using UnityEngine;

namespace Title
{
    public class FloatingIslandView : MonoBehaviour
    {
        private Sequence _sequence;
        // Start is called before the first frame update
        public void Setup()
        {
           _sequence =  DOTween.Sequence()
                .Append(transform.DOLocalMoveY(transform.localPosition.y + 60, 2))
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDestroy()
        {
            _sequence.Kill();
        }
    }
}

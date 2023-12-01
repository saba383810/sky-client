using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Blink : MonoBehaviour
{
    private Sequence _sequence;
    void Start()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        _sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                canvasGroup.alpha = 0;
            })
            .Append(canvasGroup.DOFade(1, 1))
            .AppendInterval(0.5f)
            .Append(canvasGroup.DOFade(0,0.5f))
            .SetLoops(-1);
    }

    private void OnDestroy()
    {
        _sequence.Kill();
    }
}

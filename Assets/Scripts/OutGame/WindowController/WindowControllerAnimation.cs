using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WindowControllerAnimation : MonoBehaviour
{
    
    /// <summary>
    /// アニメーション
    /// </summary>
    /// <param name="animTransform"></param>
    public static void SlideInAnimation(Transform animTransform)
    {
        var tmpLocalPos = animTransform.localPosition;
        DOTween.Sequence()
            .OnStart(() =>
            {
                animTransform.localPosition += new Vector3(1000, 0, 0);
                animTransform.gameObject.SetActive(true);
            })
            .Append(animTransform.DOLocalMove(tmpLocalPos, 0.3f));
    }
}

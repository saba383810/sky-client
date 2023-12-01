using System;
using Common.Loading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Splash
{
    public class SplashController : MonoBehaviour
    {
        /// <summary>skyGamesSplash</summary>
        [SerializeField] private CanvasGroup splashImgCanvasGroup = default;
        /// <summary>アニメーションのスピード</summary>
        private const float AnimTime = 0.5f;
        /// <summary>スプラッシュの画像を表示している間の時間</summary>
        private const float SplashWaitTime = 1.5f;

        private async void Start()
        {
            BGMManager.Instance.Play(BGMPath.TITLE);
            Application.targetFrameRate = 60; 
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            await DOTween.Sequence()
                .OnStart(() =>
                {
                    splashImgCanvasGroup.alpha = 0;
                    splashImgCanvasGroup.gameObject.SetActive(true);

                })
                .Append(splashImgCanvasGroup.DOFade(1, AnimTime))
                .AppendInterval(SplashWaitTime)
                .Append(splashImgCanvasGroup.DOFade(0, AnimTime))
                .OnComplete(() => splashImgCanvasGroup.gameObject.SetActive(false));

            LoadingManager.ChangeScene("Title");
        }
    }
}

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// Sceneのロードを管理
    /// </summary>
    public class LoadingWindow : MonoBehaviour
    {
        [SerializeField] private CanvasGroup loadingWindow = default;
        private const float AnimSpeed = 0.6f;

        private void Start()
        {
            Show();
        }

        public void Show()
        {
            loadingWindow.alpha = 0;
            loadingWindow.gameObject.SetActive(true);
            Debug.Log("Loading開始");
            loadingWindow.DOFade(1, AnimSpeed).SetEase(Ease.InQuint);
        }

        public async UniTask<bool> Hide()
        {
            await loadingWindow.DOFade(0, AnimSpeed).SetEase(Ease.InQuint);
            loadingWindow.alpha = 1;
            loadingWindow.gameObject.SetActive(false);
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sabanogames.AudioManager;
using UnityEngine;

namespace ScreenManager
{
    public class ScreenManager : SingletonMonoBehaviour<ScreenManager>
    {

        public Transform uiScreen =null;
        private readonly Dictionary<ScreenUI, GameObject> _instanceDic = new Dictionary<ScreenUI, GameObject>();
        private const float AnimUISpeed = 0.3f;
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            new GameObject("ScreenManager", typeof(ScreenManager));
        }

        protected override void Init()
        {
            base.Init();
            DontDestroyOnLoad(gameObject);
        }

        public enum ScreenUI
        {
            Splash_Logo,
            Popup_EquipmentList,
            PopUp_Training
        }

        public enum ShowType
        {
            splashFadeIn,
            AlphaIn,
            SlideInRight,
            SlideInLeft
        }
        public enum HideType
        {
            splashFadeOut,
            AlphaOut,
            SlideOutRight,
            SlidOutLeft
        }

        public void ChangeScreen(Screen screen, ShowType fadeType)
        {
        
        }

        public async UniTask<bool> AddUIScreen(ScreenUI screen , ShowType animType)
        {
            //screenの読み込み
            var screenPrefab =  (GameObject)  Resources.Load($"Screen/{screen}");
        
            //読み込みに失敗した場合
            if (screenPrefab == null)
            {
                Debug.LogError("screenが読み込めませんでした。");
                return false;
            }
        
            //生成
            var instanceScreen = Instantiate(screenPrefab, uiScreen, true);
            instanceScreen.transform.localPosition = new Vector3(0, 0, 0);
            _instanceDic.Add(screen,instanceScreen);
            instanceScreen.SetActive(false);
        
            //fade
            _instanceDic[screen].SetActive(false);
            var isAnimDone = false;
            var canvasGroup = _instanceDic[screen].GetComponent<CanvasGroup>();
            var instanceTransform = _instanceDic[screen].gameObject.transform;
        
            switch (animType)
            {
                case ShowType.splashFadeIn:
                    break;
                case ShowType.AlphaIn:
                    break;
                case ShowType.SlideInRight:
                    break;
                case ShowType.SlideInLeft:
                default:
                    Debug.LogError("fadeError");
                    return false;
            }

            await UniTask.WaitUntil(() => isAnimDone);

            return true;
        }

        public void RemoveUIScreen(ScreenUI screen, HideType animType)
        {
            switch (animType)
            {
                case HideType.splashFadeOut:
                    break;
                case HideType.AlphaOut:
                    DOTween.Sequence()
                        .Append(_instanceDic[screen].GetComponent<CanvasGroup>().DOFade(0, 0.3f))
                        .OnComplete(() =>
                        {
                            Destroy(_instanceDic[screen].gameObject);
                            _instanceDic.Remove(screen);
                        });
                    break;
                case HideType.SlideOutRight:
                    DOTween.Sequence()
                        .Append(_instanceDic[screen].transform.DOLocalMoveX(2000, AnimUISpeed))
                        .OnComplete(() =>
                        {
                            Destroy(_instanceDic[screen].gameObject);
                            _instanceDic.Remove(screen);
                        });
                    break;
                case HideType.SlidOutLeft:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animType), animType, null);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Loading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OutGame
{
    public class BottomMenuBar : MonoBehaviour
    {
        /// <summary>Button</summary>
        [SerializeField] public Button homeButton = default;
        [SerializeField] public Button dressButton = default;
        [SerializeField] public Button hakoniwaButton = default;
        [SerializeField] public Button shopButton = default;
        [SerializeField] public Button craftButton = default;
        [SerializeField] public Button miniGameButton = default;
        [SerializeField] public Button gachaButton = default;
        
        /// <summary>ステート</summary>
        private readonly HomeState _homeState = new HomeState();
        private readonly DressState _dressState = new DressState();
        private readonly HakoniwaState _hakoniwaState = new HakoniwaState();
        private readonly ShopState _shopState = new ShopState();
        private readonly LobbyState _craftState = new LobbyState();
        private readonly MiniGameState _miniGameState = new MiniGameState();
        private readonly GachaState _gachaState = new GachaState();

        /// <summary>現在開かれているwindowを入れておく</summary>
        public List<GameObject> windowList = new List<GameObject>();
        /// <summary>windowを入れるための親</summary>
        private Transform _stateUIParent;
        /// <summary>初期のステート</summary>
        private const BottomMenuStateType InitStateType = BottomMenuStateType.Home;

        private float _animSpeed = 0.3f;

        /// <summary>ステートの種類</summary>
        public enum BottomMenuStateType
        {
            Home,
            Dress,
            Hakoniwa,
            Shop,
            Craft,
            Gacha,
            MiniGame
        }

        /// <summary>ステートのトリガー</summary>
        private enum TriggerType
        {
            HomeButtonClick,
            DressButtonClick,
            HakoniwaButtonClick, 
            ShopButtonClick,
            CraftButtonClick,
            GachaButtonClick
        }
        /// <summary>ステートマシン</summary>
        private StateMachine<BottomMenuStateType, TriggerType> _stateMachine;

        public async void Setup()
        {
            var menuBarPosition = transform.localPosition;
            

            await DOTween.Sequence()
                .OnStart(() =>
                {
                    transform.localPosition += new Vector3(0, -300, 0);
                })
                .AppendInterval(0.2f)
                .Append(transform.DOLocalMoveY(menuBarPosition.y, _animSpeed));

            //親を取得
            _stateUIParent = GameObject.Find("State_UI").transform;
            
            // StateMachineを生成
            _stateMachine = new StateMachine<BottomMenuStateType, TriggerType>();

            //stateの登録
            _stateMachine.SetupState(BottomMenuStateType.Home,()=> _homeState.OnEnter(this) , () => _homeState.OnExit(this), deltaTime => _homeState.OnUpdate(this));
            _stateMachine.SetupState(BottomMenuStateType.Dress,()=> _dressState.OnEnter(this) , () => _dressState.OnExit(this), deltaTime => _dressState.OnUpdate(this));
            _stateMachine.SetupState(BottomMenuStateType.Hakoniwa,()=> _hakoniwaState.OnEnter(this) , () => _hakoniwaState.OnExit(this), deltaTime => _hakoniwaState.OnUpdate(this));
            _stateMachine.SetupState(BottomMenuStateType.Shop,()=> _shopState.OnEnter(this) , () => _shopState.OnExit(this), deltaTime => _shopState.OnUpdate(this));
            _stateMachine.SetupState(BottomMenuStateType.Craft,()=> _craftState.OnEnter(this) , () => _craftState.OnExit(this), deltaTime => _craftState.OnUpdate(this));
            _stateMachine.SetupState(BottomMenuStateType.Gacha,()=> _gachaState.OnEnter(this) , () => _gachaState.OnExit(this), deltaTime => _gachaState.OnUpdate(this));
            _stateMachine.SetupState(BottomMenuStateType.MiniGame ,()=> _miniGameState.OnEnter(this) , () => _miniGameState.OnExit(this), deltaTime => _miniGameState.OnUpdate(this));
            
            //Button click時の処理
            homeButton.onClick.AddListener(() => _stateMachine.ChangeState(BottomMenuStateType.Home));
            dressButton.onClick.AddListener(() => _stateMachine.ChangeState(BottomMenuStateType.Dress));
            hakoniwaButton.onClick.AddListener(() => _stateMachine.ChangeState(BottomMenuStateType.Hakoniwa));
            shopButton.onClick.AddListener(() => _stateMachine.ChangeState(BottomMenuStateType.Shop));
            craftButton.onClick.AddListener(() => _stateMachine.ChangeState(BottomMenuStateType.Craft));
            miniGameButton.onClick.AddListener(() =>_stateMachine.ChangeState(BottomMenuStateType.MiniGame));
            gachaButton.onClick.AddListener(() => _stateMachine.ChangeState(BottomMenuStateType.Gacha));
            
            _stateMachine.ChangeState(InitStateType);
            ManualUpdate().Forget();
            
        }

        private async UniTask ManualUpdate()
        {
            // ステートマシンを更新
            while (true) 
            {
                await UniTask.Yield();
                _stateMachine.Update(Time.deltaTime);
            }
           
        }
        
        
        /// <summary>
        /// ボタンをクリックしたときのボタンアニメーション
        /// </summary>
        /// <param name="button"></param>
        public void ButtonClickAnimation(Button button)
        {
            Debug.Log("ButtonClickAnimation");
            if (!button.enabled) return;
            button.enabled = false;
            
            button.transform.GetChild(0).gameObject.SetActive(true);
            var buttonTransform = button.transform.GetChild(1);
            var posY = button.transform.GetChild(1).position.y;
            DOTween.Sequence()
                .Append(buttonTransform.DOScale(new Vector3(1.2f, 1.3f, 1.2f), 0.2f))
                .Join(buttonTransform.DOMoveY(posY + 30, 0.2f))
                .Append(buttonTransform.DOScale(new Vector3(1.1f, 1.2f, 1.1f), 0.3f))
                .Join(buttonTransform.DOMoveY(posY, 0.3f).SetEase(Ease.OutBounce));
        }
        /// <summary>
        /// 別のボタンを押したときのアニメーション
        /// </summary>
        /// <param name="button"></param>
        public void ButtonExitAnimation(Button button)
        {
            button.transform.GetChild(0).gameObject.SetActive(false);
            DOTween.Sequence()
                .Append(button.transform.GetChild(1).DOScale(new Vector3(1f, 1f, 1f), 0.6f))
                .OnComplete(() => button.enabled = true);
        }

        private const string BottomMenuPath = "Prefabs/OutGame/BottomMenuWindow/";
        
        /// <summary>
        /// windowを生成し、リストに追加する関数
        /// </summary>
        /// <param name="bottomMenuStateType">生成したいwindowのステートタイプ</param>
        /// <exception cref="Exception"></exception>
        public void AddWindow(BottomMenuStateType bottomMenuStateType)
        {
            GameObject loadWindow;
            switch (bottomMenuStateType)
            {
                case BottomMenuStateType.Home:
                    loadWindow = (GameObject)Resources.Load ($"{BottomMenuPath}HomeWindow");
                    break;
                case BottomMenuStateType.Dress:
                    loadWindow = (GameObject)Resources.Load ($"{BottomMenuPath}DressWindow");
                    break;
                case BottomMenuStateType.Hakoniwa:
                    loadWindow = (GameObject)Resources.Load ($"{BottomMenuPath}HakoniwaWindow");
                    break;
                case BottomMenuStateType.Shop:
                    loadWindow = (GameObject)Resources.Load ($"{BottomMenuPath}ShopWindow");
                    break;
                case BottomMenuStateType.Craft:
                    loadWindow = (GameObject)Resources.Load ($"{BottomMenuPath}CraftWindow");
                    break;
                case BottomMenuStateType.Gacha:
                    loadWindow = (GameObject)Resources.Load ($"{BottomMenuPath}GachaWindow");
                    break;
                case BottomMenuStateType.MiniGame:
                    loadWindow = (GameObject)Resources.Load ($"{BottomMenuPath}MiniGameWindow");
                    break;
                default:
                    Debug.LogError("読み込みエラー");
                    throw new Exception();
            }
            loadWindow.GetComponent<CanvasGroup>().alpha = 0;
            var instanceWindow = Instantiate(loadWindow, _stateUIParent);
            
            windowList.Add(instanceWindow);

            // 表示fadeアニメーション
            DOTween.Sequence()
                .Append(instanceWindow.GetComponent<CanvasGroup>().DOFade(1, 0.3f));
        }
        
        /// <summary>
        /// WindowListの先頭をfadeさせて削除する
        /// </summary>
        /// <param name="bottomMenuStateType"></param>
        public void RemoveListTopWindow()
        {
            //windowListの先頭を取得
            var firstObj = windowList[0];
            
            // 非表示fadeアニメーション
            DOTween.Sequence()
                .Append(firstObj.GetComponent<CanvasGroup>().DOFade(0, 0.3f))
                .OnComplete(() =>
                {
                    windowList.Remove(firstObj);
                    Destroy(firstObj);
                    _stateMachine._isChangingState = false;
                });
        }
        
    }

    /// ------------------------------------------------------
    /// MenuBarState関連
    /// ------------------------------------------------------
    
    /// <summary>
    /// StateMachineの本体
    /// </summary>
    public abstract class BottomMenuBarState
    {
        public abstract void OnEnter(BottomMenuBar owner);
        public abstract void OnExit(BottomMenuBar owner);
        public abstract void OnUpdate(BottomMenuBar owner);
    }

    /// <summary>
    /// ホームメニューの画面表示
    /// </summary>
    public class HomeState : BottomMenuBarState
    {
        public override void OnEnter(BottomMenuBar owner)
        {
            BGMManager.Instance.Play(BGMPath.OUT_GAME);
            Debug.Log("OnEnter: Home");
            owner.ButtonClickAnimation(owner.homeButton);
            owner.AddWindow(BottomMenuBar.BottomMenuStateType.Home);
        }

        public override void OnExit(BottomMenuBar owner)
        {
            Debug.Log("OnExit ; Home");
            owner.RemoveListTopWindow();
            owner.ButtonExitAnimation(owner.homeButton);
        }

        public override void OnUpdate(BottomMenuBar owner)
        {
        }
    }
    
    /// <summary>
    /// 着替えメニューの画面表示
    /// </summary>
    public class DressState :  BottomMenuBarState
    {
        public override void OnEnter(BottomMenuBar owner)
        {
            Debug.Log("OnEnter: Dress");
            owner.ButtonClickAnimation(owner.dressButton);
            owner.AddWindow(BottomMenuBar.BottomMenuStateType.Dress);
        }

        public override void OnExit(BottomMenuBar owner)
        {
            Debug.Log("OnExit: Dress");
            owner.RemoveListTopWindow();
            owner.ButtonExitAnimation(owner.dressButton);
            
        }

        public override void OnUpdate(BottomMenuBar owner)
        {
        }
    }
    
    /// <summary>
    /// ハコニワメニューの画面表示
    /// </summary>
    public class HakoniwaState :  BottomMenuBarState
    {
        public override void OnEnter(BottomMenuBar owner)
        {
            Debug.Log("OnEnter: Hakoniwa");
            // owner.ButtonClickAnimation(owner.hakoniwaButton);
            // owner.AddWindow(BottomMenuBar.BottomMenuStateType.Hakoniwa);
            
            LoadingManager.ChangeScene("Hakoniwa");
        }

        public override void OnExit(BottomMenuBar owner)
        {
            Debug.Log("OnExit: Hakoniwa");
            owner.RemoveListTopWindow();
            owner.ButtonExitAnimation(owner.hakoniwaButton);
            
        }

        public override void OnUpdate(BottomMenuBar owner)
        {
        }
    }
    
    /// <summary>
    /// ショップメニューの画面表示
    /// </summary>
    public class ShopState :  BottomMenuBarState
    {
        public override void OnEnter(BottomMenuBar owner)
        {
            Debug.Log("OnEnter: Shop");
            BGMManager.Instance.Play(BGMPath.OUT_GAME);
            owner.ButtonClickAnimation(owner.shopButton);
            owner.AddWindow(BottomMenuBar.BottomMenuStateType.Shop);
        }

        public override void OnExit(BottomMenuBar owner)
        {
            Debug.Log("OnExit: Shop");
            owner.RemoveListTopWindow();
            owner.ButtonExitAnimation(owner.shopButton);
           
        }

        public override void OnUpdate(BottomMenuBar owner)
        {
        }
    }
    
    /// <summary>
    /// クラフトメニューの画面表示
    /// </summary>
    public class LobbyState :  BottomMenuBarState
    {
        public override void OnEnter(BottomMenuBar owner)
        {
            Debug.Log("OnEnter: Lobby");
            LoadingManager.ChangeScene("Lobby");
            
        }

        public override void OnExit(BottomMenuBar owner)
        {
            owner.RemoveListTopWindow();
            owner.ButtonExitAnimation(owner.craftButton);
        }

        public override void OnUpdate(BottomMenuBar owner)
        {
        }
    }
    
    
    /// <summary>
    /// ミニゲームメニューの画面表示
    /// </summary>
    public class MiniGameState : BottomMenuBarState
    {
        public override void OnEnter(BottomMenuBar owner)
        {
            Debug.Log("OnEnter: MiniGame");
            BGMManager.Instance.Play(BGMPath.MINI_GAME);
            owner.ButtonClickAnimation(owner.miniGameButton);
            owner.AddWindow(BottomMenuBar.BottomMenuStateType.MiniGame);
        }

        public override void OnExit(BottomMenuBar owner)
        {
            Debug.Log("OnExit:MiniGame");
            owner.ButtonExitAnimation(owner.miniGameButton);
            owner.RemoveListTopWindow();
        }

        public override void OnUpdate(BottomMenuBar owner)
        {
        }
    }
    
    /// <summary>
    /// ガチャメニューの画面表示
    /// </summary>
    public class GachaState : BottomMenuBarState
    {
        public override void OnEnter(BottomMenuBar owner)
        {
            Debug.Log("OnEnter: Gacha");
            LoadingManager.ChangeScene("Gacha");
        }

        public override void OnExit(BottomMenuBar owner)
        {
            Debug.Log("OnExit:Gacha");
        }

        public override void OnUpdate(BottomMenuBar owner)
        {
        }
    }
    
}

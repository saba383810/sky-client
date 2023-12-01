using System.Diagnostics.Tracing;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class ScreenTapScripts : MonoBehaviour
{
    [SerializeField] private GameObject _tapscreen;
    [SerializeField] private GameObject _fishobj;
    [SerializeField] private GameObject _logoImage;
    [SerializeField] private Image _damageImage;
    [SerializeField] private Sprite _damage_ok;
    [SerializeField] private Sprite _damage_no;
    [SerializeField] private Slider _hpSlider;
    private GameObject Hari,Hariobj;
    private int Ram_status;
    private bool DamageFlag = false;//３秒ごとにダメージの判定に必要な要素をランダムで変更する
    private float FishHP = 50;
    private float mousePosition;
    private CancellationTokenSource cts;
    private  FishGameState _fishGameState;

    private enum FishGameState
    {
        DamegeOk,
        DamegeNo,
    }

    private void Start()
    {
        cts = new CancellationTokenSource();
        Hari = (GameObject)Resources.Load("Prefabs/Fishing/Turibari");
        _fishGameState = FishGameState.DamegeNo;
        _damageImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        HitFish();
        DamageStatus();
        if (Input.GetMouseButton(0) && FishCnt.fishingStatus == FishCnt.GameStatus.GamePlaynow)//バトル中のクリック処理
        {
            if (_fishGameState == FishGameState.DamegeOk)
            {
                FishHP += 0.3f;
            }
            else
            {
                FishHP -= 0.1f;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            var Hoge = GameObject.FindWithTag("Fruit");
            mousePosition = Input.mousePosition.x;
            //todo 生成座標を直接入力しているので注意
            if (null == Hoge)
            {
                Hariobj = Instantiate(Hari, new Vector2(850f,650f), Quaternion.identity);
                Hariobj.transform.parent = _tapscreen.transform;
                Tapthrow();   
            }
            Tapback();
        }
    }

    async UniTask Tapback()
    {
        var Hoge = GameObject.FindWithTag("Fruit");
        await UniTask.WaitUntil(() => Input.GetMouseButtonUp(0), cancellationToken: cts.Token);
        if (FishCnt.fishingStatus == FishCnt.GameStatus.Default && Hoge != null )
        {
            await Hariobj.transform.DOMove(new Vector2(850f, 650f), 1.5f);
            FishCnt.fishingStatus = FishCnt.GameStatus.failed;
            Destroy(Hariobj);
            Debug.Log("gm");
        }
        else if (FishCnt.fishingStatus == FishCnt.GameStatus.Hit )
        {
            FishCnt.fishingStatus = FishCnt.GameStatus.Empty;
            await _logoImage.transform.DOMoveY(400f,2.5f).SetEase(Ease.OutBounce);
            await _logoImage.transform.DOMoveY(1200f,1.0f);
            FishCnt.fishingStatus = FishCnt.GameStatus.HitGame;
        }
    }
    async UniTask Tapthrow()
    {
        await Hariobj.transform.DOMoveX(mousePosition, 1.5f);
    }
    async UniTask HitFish()//魚とのバトル中の処理
    {
        await UniTask.WaitUntil(() => (FishCnt.fishingStatus == FishCnt.GameStatus.HitGame), cancellationToken: cts.Token);
        FishHP = 50;
        FishCnt.fishingStatus = FishCnt.GameStatus.GamePlaynow;
        _damageImage.gameObject.SetActive(true);//ダメージ判定を表示
        while (FishCnt.fishingStatus == FishCnt.GameStatus.GamePlaynow && FishHP > 0 && FishHP <= 100)
        {
            FishHP -= 1;
            await UniTask.Delay(250, cancellationToken: cts.Token);
        }
        if (FishHP >= 100)
        {
            GameClear();
        }
        else
        {
            FishCnt.fishingStatus = FishCnt.GameStatus.failed;
        }
        //FishCnt.fishingStatus = FishCnt.GameStatus.Default;
        await Hariobj.transform.DOMove(new Vector2(850f, 650f), 1.5f);
        FishCnt.HitLotteryFlag = true;
        _damageImage.gameObject.SetActive(false);
        Destroy(Hariobj);
    }

    async UniTask DamageStatus()//ダメージ判定、スライダー操作
    {
        _hpSlider.value = FishHP;
        await UniTask.WaitUntil((() => FishCnt.fishingStatus == FishCnt.GameStatus.GamePlaynow　&& DamageFlag == false),
            cancellationToken: cts.Token);
        DamageFlag = true;
        Ram_status = Random.Range(1,4);
        if (Ram_status != 1)
        {
            _fishGameState = FishGameState.DamegeOk;
            _damageImage.sprite = _damage_ok;
        }
        else
        {
            _fishGameState = FishGameState.DamegeNo;
            _damageImage.sprite = _damage_no;
        }
        await UniTask.Delay(1500);
        DamageFlag = false;
    }

    async UniTask GameClear()
    {
        await _logoImage.transform.DOMoveY(400f,2.5f).SetEase(Ease.OutBounce);
        await _logoImage.transform.DOMoveY(1200f,1.0f);
        FishCnt.fishingStatus = FishCnt.GameStatus.GameClear;
        Debug.Log("EZG");
    }
    private void OnDestroy()
    {
        cts.Cancel();
    }
}

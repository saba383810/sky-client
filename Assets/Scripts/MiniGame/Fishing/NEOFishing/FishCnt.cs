using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishCnt : MonoBehaviour
{
    [SerializeField] private GameObject _tapscreen;
    [SerializeField]private GameObject Bikkuri;
    private bool FishMoveTimespan = true;
    private bool FishMoveTimespan2 = true;
    private bool FishGetChanceFlag = false;
    private bool FishHitFlag = false;
    private int HitLottery;
    public static bool HitLotteryFlag = true;
    private Tweener tweener;
    private CancellationTokenSource cts;
    private GameObject Bikkuriobj;
    public static GameStatus fishingStatus;

    public enum GameStatus
    {
        Default,
        Hit,
        HitGame,
        GamePlaynow,
        GameClear,
        Empty,
        failed,
    }
    private async void Start()
    {
        cts = new CancellationTokenSource();
        await FishmoveAsync(this.GetCancellationTokenOnDestroy());
        fishingStatus = GameStatus.Default;
        int tagId = int.Parse(this.gameObject.tag);
        FishEscape();
    }

    private void Update()
    {
        FishmoveAsync();
    }

    private async UniTask FishmoveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (fishingStatus == GameStatus.GamePlaynow)
        {
            if (FishMoveTimespan2 == true && FishGetChanceFlag == false)
            {
                tweener = this.transform.DOPath(new[]
                    {
                        new Vector3(Random.Range(250f,1700f),Random.Range(100f,450f) , 0f),
                        new Vector3(Random.Range(250f,1700f),Random.Range(100f,450f) , 0f),
                        new Vector3(Random.Range(250f,1700f),Random.Range(100f,450f) , 0f)
                    },
                    2f,PathType.CatmullRom,PathMode.Sidescroller2D).SetLookAt(0.05f);
                FishMoveTimespan2 = false;
                await UniTask.Delay(1800,cancellationToken:cancellationToken);
                FishMoveTimespan2 = true;
            }
        }
        else if (FishMoveTimespan == true && FishGetChanceFlag == false)
        {
            tweener = this.transform.DOPath(new[]
            {
              new Vector3(Random.Range(250f,1700f),Random.Range(100f,450f) , 0f),
              new Vector3(Random.Range(250f,1700f),Random.Range(100f,450f) , 0f),
              new Vector3(Random.Range(250f,1700f),Random.Range(100f,450f) , 0f)
            },
            7f,PathType.CatmullRom,PathMode.Sidescroller2D).SetLookAt(0.05f);
            FishMoveTimespan = false;
            await UniTask.Delay(7000,cancellationToken:cancellationToken);
            FishMoveTimespan = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)//魚が範囲内に入った時
    {
        if (other.CompareTag("Fruit")&& fishingStatus != GameStatus.GamePlaynow)
        {
            GameObject turibari =GameObject.FindWithTag("Fruit");
            FishGetChanceFlag = true;
            tweener.Kill();
            this.transform.DOPath(new []
                {
                    new Vector3(this.transform.position.x,this.transform.position.y),
                    new Vector3(turibari.transform.position.x,200)
                },3f,PathType.CatmullRom,PathMode.Sidescroller2D).SetLookAt(1f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        FishGetChanceFlag = false;
    }

    private async void OnTriggerStay2D(Collider2D other)
    {
        if (HitLotteryFlag)//魚がかかるか待つ時
        {
            HitLotteryFlag = false;
            if (fishingStatus == GameStatus.Default)
            {
                HitLottery = Random.Range(1, 4);
                if (HitLottery == 2)
                {
                    Bikkuriobj = Instantiate(Bikkuri, new Vector2(this.transform.position.x, this.transform.position.y),
                        Quaternion.identity);
                    Bikkuriobj.transform.parent = _tapscreen.transform;
                    fishingStatus = GameStatus.Hit;
                    await UniTask.Delay(2000, cancellationToken: cts.Token);
                    Destroy(Bikkuriobj.gameObject);
                }
                else
                {
                    fishingStatus = GameStatus.Default;
                    await UniTask.Delay(1000, cancellationToken: cts.Token);
                }
                HitLotteryFlag = true;
            }
        }
    }

    private async UniTask FishEscape(CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => fishingStatus == GameStatus.failed, cancellationToken: cts.Token);
        cancellationToken.ThrowIfCancellationRequested();
        tweener.Kill();
        //await this.transform.DOMoveX(5000f,2f);
        await UniTask.Delay(500,cancellationToken:cancellationToken);
        OnDestroy();
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        cts.Cancel();
    }
}

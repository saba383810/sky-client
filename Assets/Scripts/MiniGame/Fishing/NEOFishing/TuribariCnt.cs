using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class TuribariCnt : MonoBehaviour
{
    // Start is called before the first frame update
    private async void Start()
    {
        GetComponent<Collider2D>().enabled = false;
        await UniTask.Delay(2400);
        GetComponent<Collider2D>().enabled = true;
        Collider_delete();

    }

    private async UniTask Collider_delete()
    {
        await UniTask.WaitUntil(() => FishCnt.fishingStatus == FishCnt.GameStatus.failed);
        GetComponent<Collider2D>().enabled = false;
    }
}

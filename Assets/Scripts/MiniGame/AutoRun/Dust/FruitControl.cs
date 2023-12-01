using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
namespace MiniGame.AutoRun
{
    public class FruitControl : MonoBehaviour
    {
        private bool _gameEnd;
        private CancellationTokenSource cts = new CancellationTokenSource();
        [SerializeField] private float miniY;
        [SerializeField] private float maxY;

        private async void Start()
        {
            var fruit = (GameObject)Resources.Load("Prefabs/AutoRun/Fruit");
            await CreateFruit(fruit,cts.Token);
        }
        
        private async UniTask CreateFruit(GameObject fruit, CancellationToken token)
        {
            while (!_gameEnd)
            {
                try
                {
                    fruit.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    //TODO　位置を決定させる　y軸は-1.5と3.2　が良き
                    var randomY = Random.Range(0, 2);
                    switch (randomY)
                    {
                        case 0:
                            Instantiate(fruit, new Vector3(20f,miniY,1f),Quaternion.identity);
                            break;
                        case 1:
                            Instantiate(fruit, new Vector3(20f,maxY,1f),Quaternion.identity);
                            break;
                    }
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
                }
                catch (OperationCanceledException e)
                {
                    Debug.Log("Canceled");
                    throw;
                }
               
            } 
        }

        private void OnDestroy()
        {
             cts.Cancel();
        }
    }
}

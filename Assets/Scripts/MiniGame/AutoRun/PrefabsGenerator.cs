using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame.AutoRun
{
    public class PrefabsGenerator : MonoBehaviour
    {
        private bool _isPrefabGeneratorStop;
        private CancellationTokenSource _cts;
        [SerializeField] private float miniPositionY;
        [SerializeField] private float maxPositionY;
        [SerializeField] private int timeSpan;

        
        private void Start()
        {
            //初期化
            _isPrefabGeneratorStop = false;
        }

        public void PrefabGeneratorStart()
        {
            //オブジェクト自動生成
            var fruit = (GameObject)Resources.Load("Prefabs/AutoRun/Fruit");
            var obstacle = (GameObject)Resources.Load("Prefabs/AutoRun/Obstacle");
            _cts = new CancellationTokenSource();
            CreatePrefabs(fruit,obstacle,_cts.Token).Forget();
        }

        private async UniTaskVoid CreatePrefabs(GameObject fruit, GameObject obstacle, CancellationToken token)
        {
            while (!_isPrefabGeneratorStop)
            {
                //フルーツの時(0)、障害物の時(1)
                var randomPrefab = Random.Range(0, 2);
                switch (randomPrefab)
                {
                    case 0:
                        await CreateFruit(fruit);
                        break;
                    case 1:
                        await CreateObstacle(obstacle);
                        break;
                }
                await UniTask.Delay(TimeSpan.FromSeconds(timeSpan), cancellationToken: token);
            }
        }

        //フルーツの生成
        private async UniTask<bool> CreateFruit(GameObject fruit)
        {
            fruit.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //TODO　位置を決定させる　y軸は-1.5と3.2　が良き
            var randomPositionY = Random.Range(0, 2);
            switch (randomPositionY)
            {
                case 0:
                    Instantiate(fruit, new Vector3(20f,miniPositionY,1f),Quaternion.identity);
                    break;
                case 1:
                    Instantiate(fruit, new Vector3(20f,maxPositionY,1f),Quaternion.identity);
                    break;
            }
            await UniTask.Yield(); 
            return true;
        }
        
        //障害物の生成
        private async UniTask<bool> CreateObstacle(GameObject obstacle)
        {
            Instantiate(obstacle, new Vector3(20f,miniPositionY,1f),Quaternion.identity);
            await UniTask.Yield();
            return true;
        }
        
        private void OnDestroy()
        { 
            _cts.Cancel();
        }
        
        //ゲーム終了
        public void EnabledPrefabGeneratorStopFlag()
        {
            _isPrefabGeneratorStop = true;
        }
    }
}

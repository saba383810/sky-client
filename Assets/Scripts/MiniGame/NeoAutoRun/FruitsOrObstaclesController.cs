using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PlayFab.MultiplayerModels;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame.NeoAutoRun
{
    public class FruitsOrObstaclesController : MonoBehaviour
    {
        [SerializeField] private NeoAutoRunGameManager neoAutoRunGameManager;
        [SerializeField] private float miniPositionY;
        [SerializeField] private float maxPositionY;
        [SerializeField] private int timeSpan;
        private CancellationTokenSource _cts;
        private List<GameObject> _objects = new List<GameObject>();
        
        public void FruitsOrObstaclesControllerStart()
        {
            //オブジェクト自動生成
            var fruit = (GameObject)Resources.Load("Prefabs/NeoAutoRun/ObjectPrefabs/Fruit");
            var obstacle = (GameObject)Resources.Load("Prefabs/NeoAutoRun/ObjectPrefabs/Obstacle");
            _cts = new CancellationTokenSource();
            CreatePrefabs(fruit,obstacle,_cts.Token).Forget();
        }

        // 生成したオブジェクトを追加するメソッド
        private void AddObject(GameObject obj)
        {
            _objects.Add(obj);
        }
        
        private async UniTaskVoid CreatePrefabs(GameObject fruit, GameObject obstacle, CancellationToken token)
        {
            var phase = neoAutoRunGameManager.GetAutoRunPhase();
            while (phase == NeoAutoRunGameManager.AutoRunPhase.Started)
            { 
                //フルーツの時(0)、障害物の時(1)
                var randomPrefab = Random.Range(0, 2);
                GameObject createdFruit;
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
                phase = neoAutoRunGameManager.GetAutoRunPhase();
            }
        }

        //フルーツの生成
        private async UniTask<bool> CreateFruit(GameObject fruit)
        {
            fruit.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            GameObject createPrefabs = null;
            //位置を決定させる　y軸は-1.5と3.2　が良き
            var randomPositionY = Random.Range(0, 2);
            switch (randomPositionY)
            {
                case 0: 
                    createPrefabs = Instantiate(fruit, new Vector3(20f,miniPositionY,1f),Quaternion.identity);
                    break;
                case 1:
                    createPrefabs = Instantiate(fruit, new Vector3(20f,maxPositionY,1f),Quaternion.identity);
                    break;
            }
            _objects.Add(createPrefabs);
            await UniTask.Yield(); 
            return true;
        }
        
        //障害物の生成
        private async UniTask<bool> CreateObstacle(GameObject obstacle)
        {
            GameObject createPrefabs = null;
            createPrefabs = Instantiate(obstacle, new Vector3(20f,miniPositionY,1f),Quaternion.identity);
            _objects.Add(createPrefabs);
            await UniTask.Yield();
            return true;
        }
        
        /// <summary>
        /// ゲーム終了時にフルーツと障害物を破壊するよ
        /// </summary>
        public void GameEndPrefabsDestroy()
        {
            foreach (var obj in _objects)
            {
                Destroy(obj);
            }
        }
        
        private void OnDestroy()
        {
            _cts.Cancel();
        }
    }
}

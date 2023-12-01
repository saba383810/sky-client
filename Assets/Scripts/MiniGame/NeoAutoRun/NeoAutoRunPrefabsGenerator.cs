using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

namespace MiniGame.NeoAutoRun
{
    public class NeoAutoRunPrefabsGenerator : MonoBehaviour
    {
        [SerializeField] private NeoAutoRunGameManager neoAutoRunGameManager;
        [SerializeField] private float miniPositionY;
        [SerializeField] private float maxPositionY;
        [SerializeField] private int timeSpan;
        private CancellationTokenSource _cts;
        private List<GameObject> _objects = new List<GameObject>();
        
        public void NeoAutoRunPrefabsGeneratorStart()
        {
            var fruit = (GameObject)Resources.Load("Prefabs/NeoAutoRun/ObjectPrefabs/Fruit");
            var obstacle = (GameObject)Resources.Load("Prefabs/NeoAutoRun/ObjectPrefabs/Obstacle");
            var grass = (GameObject)Resources.Load("Prefabs/NeoAutoRun/ObjectPrefabs/Grass_S_C_Sky");
            _cts = new CancellationTokenSource();
            CreateFruits(fruit,_cts.Token).Forget();
            CreateObstacle(obstacle,_cts.Token).Forget();
            CreateGrass(grass,_cts.Token).Forget();
        }

        // 生成したオブジェクトを追加するメソッド
        private void AddObject(GameObject obj)
        {
            _objects.Add(obj);
        }
        
        /// <summary>
        /// フルーツの生成
        /// 
        /// </summary>
        /// <param name="fruit"></param>
        /// <param name="token"></param>
        private async UniTaskVoid CreateFruits(GameObject fruit,CancellationToken token)
        {
            var phase = neoAutoRunGameManager.GetAutoRunPhase();
            while (phase == NeoAutoRunGameManager.AutoRunPhase.Started)
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
                AddObject(createPrefabs);
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
                phase = neoAutoRunGameManager.GetAutoRunPhase();
            }
        }

        /// <summary>
        /// 障害物の生成
        /// </summary>
        /// <param name="obstacle"></param>
        /// <param name="token"></param>
        private async UniTaskVoid CreateObstacle(GameObject obstacle, CancellationToken token)
        {
            var phase = neoAutoRunGameManager.GetAutoRunPhase();
            while (phase == NeoAutoRunGameManager.AutoRunPhase.Started)
            {
                GameObject createPrefabs = null;
                var randomPositionY = Random.Range(0, 2);
                switch (randomPositionY)
                {
                    case 0: 
                        createPrefabs = Instantiate(obstacle, new Vector3(20f,-0.8f,1f),Quaternion.identity);
                        break;
                    case 1:
                        createPrefabs = Instantiate(obstacle, new Vector3(20f,2.8f,1f),Quaternion.identity);
                        break;
                }
                AddObject(createPrefabs);
                await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
                phase = neoAutoRunGameManager.GetAutoRunPhase();
            }
        }


        /// <summary>
        /// 空中の地面自動生成
        /// </summary>
        /// <param name="grass"></param>
        /// <param name="cts"></param>
        private async UniTaskVoid CreateGrass(GameObject grass, CancellationToken token)
        {
            var phase = neoAutoRunGameManager.GetAutoRunPhase();
            while (phase == NeoAutoRunGameManager.AutoRunPhase.Started)
            {
                var createPrefabs = Instantiate(grass, new Vector3(20f, 1.3f, 1f), Quaternion.identity);
                AddObject(createPrefabs);
                await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: token);
                phase = neoAutoRunGameManager.GetAutoRunPhase();
            }
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

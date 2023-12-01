using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Playfab;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        public static async void ChangeScene(string nextScene)
        {
            
            //LoadingSceneを読み込み
            await SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
            
            //前のsceneを取得
            var scenes = new List<Scene>();
            for (var i = 0; i < SceneManager.sceneCount ; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Common" && scene.name !="Loading") scenes.Add(scene);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            var isSplash = false;
            
            //前のSceneをアンロード
            foreach (var scene in scenes)
            {
                if (scene.name == "Splash") isSplash = true;
                await SceneManager.UnloadSceneAsync(scene);
            }
            // プレイヤーデータの取得
            if (!isSplash)
            {
                var status = await PlayFabPlayerData.GetAllPlayerData();
                if (status == PlayFabPlayerData.PlayerDataStatus.Error)
                {
                    return;
                }
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            
            //次のSceneの読み込み
            await SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);

            //LoadingWindowを隠す
            await GameObject.Find("LoadingCanvas").GetComponent<LoadingWindow>().Hide();
            SceneManager.UnloadSceneAsync("Loading");
            Debug.Log("Loadingがめ終了");
        }
    }
}

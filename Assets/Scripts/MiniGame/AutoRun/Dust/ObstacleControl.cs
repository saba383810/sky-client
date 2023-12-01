using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniGame.AutoRun
{
    public class ObstacleControl : MonoBehaviour
    {
        private bool _gameEnd;
        private async void Start()
        {
            var obstacle = (GameObject)Resources.Load("Prefabs/AutoRun/Obstacle");
            await CreateObstacle(obstacle);
        }

        private async UniTask CreateObstacle(GameObject obstacle)
        {
            while (!_gameEnd)
            {
                obstacle.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                //Instantiate(obstacle, new Vector3(20f,miniY,1f),Quaternion.identity);
            }
        }
    }
}

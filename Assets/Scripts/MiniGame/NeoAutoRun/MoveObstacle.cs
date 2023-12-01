using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame.NeoAutoRun
{
    public class MoveObstacle : MonoBehaviour
    {
        [SerializeField] private float moveSpeedObstacle;
        [SerializeField] private float obstacleTransformEnd;

        private void Awake()
        { 
            //石かむしでわける
            var obstacleImages = Resources.LoadAll<Sprite>("Sprites/MiniGame/AutoRun/QuiteMonster2");
            var obstacleSpriteRender = gameObject.GetComponent<SpriteRenderer>();
            var obstacleRandom = Random.Range(0, obstacleImages.Length);
            obstacleSpriteRender.sprite = obstacleImages[obstacleRandom];
        }

        private void FixedUpdate()
        {
            //移動
            transform.Translate(moveSpeedObstacle,0,0);
            //削除
            if (transform.position.x < obstacleTransformEnd)
                Destroy(gameObject);
        }
    }
}

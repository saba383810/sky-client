using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame.NeoAutoRun
{
    public class MoveFruit : MonoBehaviour
    {
        [SerializeField] private float moveSpeedFruit;
        [SerializeField] private float fruitTransformEnd;
        
        private void Awake()
        {
            var fruitImages  = Resources.LoadAll<Sprite>("Sprites/MiniGame/AutoRun/Fruits");
            var fruitSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            var fruitRandom = Random.Range(0, fruitImages.Length);
            //1%の確率でゴールデンフルーツが生成させる
            fruitSpriteRenderer.sprite = Random.Range(0, 100) == 0 ? Resources.Load<Sprite>("Sprites/MiniGame/AutoRun/PremiumApple") : fruitImages[fruitRandom];
        }

        private void FixedUpdate()
        {
            //移動
            transform.Translate(moveSpeedFruit,0,0);
            //削除
            if (transform.position.x < fruitTransformEnd)
                Destroy(gameObject);
        }
    }
}

using UnityEngine;

namespace MiniGame.NeoAutoRun
{
   public class MoveSkyGrass : MonoBehaviour
   {
      [SerializeField] private float moveSpeedGrass = -0.5f;
      [SerializeField] private float fruitTransformEnd;
      
      private void FixedUpdate()
      {
         //移動
         transform.Translate(moveSpeedGrass,0,0);
         //削除
         if (transform.position.x < fruitTransformEnd)
            Destroy(gameObject);
      }
   }
}

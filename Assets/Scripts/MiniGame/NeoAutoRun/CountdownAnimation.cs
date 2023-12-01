using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.NeoAutoRun
{
   public class CountdownAnimation : MonoBehaviour
   {
      [SerializeField] private Image image;
      
      public async UniTask<bool> CountdownAnimationStart()
      {
         //画面真ん中に移動するアニメーション x軸-13
         await gameObject.transform.DOMove(new Vector2(0,0),1);
         //画面左に移動するアニメーション x軸-2000
         await gameObject.transform.DOMove(new Vector2(-13, 0), 1);
         
         image.DOFade(
            0f,     // フェード後のアルファ値
            1f      // 演出時間
         ).SetLink(gameObject);
         
         Destroy(gameObject);
         return true;
      }
   }
}

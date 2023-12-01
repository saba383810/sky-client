using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

namespace MiniGame.NeoAutoRun
{ 
    public class LifeController : MonoBehaviour
    {
        [SerializeField] private Image lifeImage,lifeImage2,lifeImage3;
        [SerializeField] private int lifeCount;

        private void Start()
        {
            lifeCount = 3;
        }

        /// <summary>
        ///ライフ計算を行う
        /// </summary>
        public bool IsGameEnd()
        {
            switch (lifeCount)
            {
                //TODO ダメージを受けた時ライフを一つ減らす、ライフが１だった場合Deadにする
                case 3:
                    lifeImage3.color = new Color(0.4f, 0.4f, 0.4f);
                    break;
                case 2:
                    lifeImage2.color = new Color(0.4f, 0.4f, 0.4f);
                    break;
                case 1:
                    lifeImage.color = new Color(0.4f, 0.4f, 0.4f);
                    break;
                default:
                    return false;
                    break;
            }

            lifeCount--;
            return lifeCount == 0;
        }
    }
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gacha
{
    public class GachaAnimationController : MonoBehaviour
    {
        [SerializeField] private GameObject gachaText,presentBox,backButton,getGachaItem;
        [SerializeField] private Image presentBoxImage,backgroundImage,getGachaItemImage;
        [SerializeField] private Sprite[] sprites = new Sprite[2];
        [SerializeField] private Transform characterMannequin; 
        public void GachaAnimation(Sprite gachaImage)
        {
            //プレゼントボックスが飛んでくるアニメーション
            DOTween.Sequence()
                .OnStart(() =>
                {
                    //初期化
                    presentBoxImage.sprite = sprites[0];
                    characterMannequin.localScale = Vector3.zero;
                })
                .Append(backgroundImage.DOFade(0.8f, 1f))
                .Append(presentBox.transform.DOLocalMoveY(-390, 2f).SetEase(Ease.OutBounce))
                .Join(presentBox.transform.DOLocalMoveX(0, 2f))
                //プレゼントボックス切り替え
                .AppendCallback((() => presentBoxImage.sprite = sprites[1]))
                .Join(presentBox.transform.DOLocalMove(new Vector3(0, -250, 0), 1f).SetEase(Ease.OutBack))
                .Join(presentBox.transform.DOScale(new Vector3(2, 2, 2), 0.2f))
                .OnComplete((() => backButton.SetActive(true)))
                //ガチャ演出をけす
                .Append(backgroundImage.DOColor(Color.white, 0.5f))
                .Join(backgroundImage.DOFade(1f, 0.5f))
                .Join(presentBoxImage.DOFade(0, 0.5f))
                .AppendCallback((() =>
                {
                    presentBox.SetActive(false);
                    backButton.SetActive(true);
                }))
                .Append(backgroundImage.DOColor(Color.black, 0))
                .OnComplete((() =>
                {
                    backgroundImage.color = Color.black;
                    var c = backgroundImage.color;
                    backgroundImage.color = new Color(c.r, c.g, c.b, 0.8f);
                }))
                .Join(backgroundImage.DOFade(0.8f, 0))
                //選出したアイテムを表示
                .OnComplete(() =>
                {
                    getGachaItem.SetActive(true);
                    getGachaItemImage.sprite = gachaImage;
                    GachaResult();
                });
        }
        
        //ガチャの結果のアニメーション
        private void GachaResult()
        {
            DOTween.Sequence()
                .Append(characterMannequin.DOScale(Vector3.one,1f).SetEase(Ease.OutBack));
            getGachaItemImage.DOColor(Color.white, 1f).SetEase(Ease.InOutQuart).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
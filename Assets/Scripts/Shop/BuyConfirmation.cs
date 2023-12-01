using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class BuyConfirmation : MonoBehaviour
    {
        [SerializeField] private ShopDao shopDao;
        [SerializeField] private GameObject purchaseConfirmation;
        [SerializeField] private Button closeButton,buyButton;
        [SerializeField] private TMP_Text buyItemName,buyItemPrice;
        [SerializeField] private Image buyItemImage;
        
        //ボタン関連のセットアップ
        private void Awake()
        {
            //閉じるボタン
            closeButton.onClick.AddListener((() =>   purchaseConfirmation.SetActive(false)));
        }
        
        /// <summary>
        /// 購入確認画面のセットアップ
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="itemId"></param>
        /// <param name="itemPrice"></param>
        /// <param name="itemImage"></param>
        public void BuyItemSetup(string itemName,int itemId ,string itemPrice, Sprite itemImage)
        {
            //初期化
            buyItemName.text = null;
            buyItemPrice.text = null;
            buyItemImage.sprite = null;
            buyButton.onClick.RemoveAllListeners();

            //購入商品名
            buyItemName.text = itemName;
            //購入商品値段
            buyItemPrice.text = itemPrice+" FP";
            //購入商品画像
            buyItemImage.sprite = itemImage;
            //購入決定ボタン
            buyButton.onClick.AddListener(() =>
            {
                //画面を閉じる
                purchaseConfirmation.SetActive(false);
                //テキストから数字のみを取得
                var buyPrice =int.Parse(Regex.Replace (buyItemPrice.text, @"[^0-9]", ""));
                shopDao.BuyDecision(buyPrice,itemId);
            });
            
        }
        
        
        
    }
}

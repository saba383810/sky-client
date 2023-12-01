using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Shop
{
    public class ShopItemListController : MonoBehaviour
    {
        [SerializeField] private GameObject buttonGemPrice,soldOutImage;
        [SerializeField] private TMP_Text name, price;
        [SerializeField] private Image image;
        [SerializeField] private Button buyButton;

        /// <summary>
        ///  名前、値段、画像、ボタンの処理を購入確認画面へセットする
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="itemPrice"></param>
        /// <param name="itemImage"></param>
        /// <param name="itemStatus"></param>
        public void ItemSetUp(string itemName,int itemId,int itemPrice,Sprite itemImage,bool itemStatus)
        {
            name.text = itemName;
            price.text = itemPrice.ToString();
            image.sprite = itemImage;
            
            //購入済みかどうかの確認 購入済み:true
            if (itemStatus)
            {
                SoldOutSetup();
            }
            else
            {
                //購入ボタン配置
                buyButton.onClick.AddListener((() =>
                {
                    var purchaseConfirmationObj = GameObject.Find("ShopController"); 
                    var shopController = purchaseConfirmationObj.GetComponent<ShopController>();
                    shopController.PurchaseConfirmation(name.text,itemId,price.text,image.sprite);
                }));
            }
        }

        private void SoldOutSetup()
        {
            //ボタン非表示
            buttonGemPrice.SetActive(false);
            soldOutImage.SetActive(true);
        }
    }
}

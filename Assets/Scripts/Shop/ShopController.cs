using UnityEngine;

namespace Shop
{
    public class ShopController : MonoBehaviour
    {
        [SerializeField] private ShopDao shopDao;
        [SerializeField] private BuyConfirmation buyConfirmation;
        [SerializeField] private GameObject purchaseConfirmation;

        /// <summary>
        /// ショップ画面の処理スタート
        /// </summary>
        private void Start()
        {
            //アイテムリストをショップに反映
            shopDao.ShopItemGeneral();
        }
        
        /// <summary>
        /// 購入確認画面を表示する
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="itemPrice"></param>
        /// <param name="itemImage"></param>
        public void PurchaseConfirmation(string itemName, int itemId,string itemPrice,Sprite itemImage)
        {
            purchaseConfirmation.SetActive(true);
            buyConfirmation.BuyItemSetup(itemName,itemId,itemPrice,itemImage);
        }
        
        
    }
}

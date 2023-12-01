using System.Linq;
using Common;
using Common.Database;
using Playfab;
using Unity.VisualScripting;
using UnityEngine;

namespace Shop
{
    //購入の処理、アイテムの反映を行う
    public class ShopDao : MonoBehaviour
    { 
        [SerializeField] private GameObject shopItemGameObject; 
        [SerializeField] private Transform shopItemGeneralTransformLeft,shopItemGeneralTransformCenter,shopItemGeneralTransformRight;
        
        /// <summary>
        /// ショップアイテムの生成を行う
        /// </summary>
        public void ShopItemGeneral()
        {
            //リストを取得
            var shopDataList = FurnitureDatabase.GetFurnitureList();
            foreach (var shopData in shopDataList)
            {
                //生成位置を取得
                var shopItemGeneralTransform = GetShopItemGeneralTransform(shopData.FurnitureID);
                var shopItemObj = Instantiate(shopItemGameObject, shopItemGeneralTransform.transform);
                var shopItemSprite = GetShopItemSprite(shopData.FurnitureID);
                //購入しているかの確認 true:購入している
                var buyStatus = IsBought(shopData.FurnitureID);
                //家具の情報をセットアップ（名前、値段、画像、購入済みかどうか）
                var shopItemListController = shopItemObj.GetComponent<ShopItemListController>();
                shopItemListController.ItemSetUp(shopData.FurnitureName,shopData.FurnitureID,shopData.FurniturePrice,shopItemSprite,buyStatus);
            }
        }
        
        
        
        /// <summary>
        /// 家具の種類によって生成する場所を決める popupの左：bedsとDevices　中：seats（ソファー）とstairs（階段）　右：tablesとWardrobes(クローゼット)
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        private Transform GetShopItemGeneralTransform(int shopId)
        {
            var typeNum = shopId / 10000;
            //10000の位を確認し家具の種類を取得する
            var shopItemTypeName = (shopId / 10000) switch
            {
                1 => "Beds",
                2 => "Devices",
                3 => "Seats",
                4 => "Stairs",
                5 => "Tables",
                6 => "Wardrobes",
                _ => ""
            };
            
            //生成する場所を決める
            switch (shopItemTypeName)
            {
                case "Beds":
                    return shopItemGeneralTransformLeft;
                case "Devices":
                    return shopItemGeneralTransformLeft;
                case "Seats":
                    return shopItemGeneralTransformCenter;
                case "Stairs":
                    return shopItemGeneralTransformCenter;
                case "Tables":
                    return shopItemGeneralTransformRight;
                case "Wardrobes":
                    return shopItemGeneralTransformRight;
            }
            Debug.LogWarning($"種類が登録されていない家具です。\nID:{shopId}");
            return null;
        }
        
        /// <summary>
        /// 家具番号を調べて画像の挿入をする
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        private Sprite GetShopItemSprite(int shopId)
        {
            //10000の位を確認し家具の種類を取得する
            var shopItemTypeName = (shopId / 10000) switch
            {
                1 => "Beds",
                2 => "Devices",
                3 => "Seats",
                4 => "Stairs",
                5 => "Tables",
                6 => "Wardrobes",
                _ => ""
            };
            //100以下の値を確認し家具の番号を取得
            var furnitureNum = (shopId % 10000) % 1000 % 100;
            return Resources.Load<Sprite>($"Sprites/Furniture/{shopItemTypeName}/{furnitureNum}");
        }

        /// <summary>
        /// すでに購入しているか
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        private bool IsBought(int shopId)
        {
            
            //データマネージャーからFurnitureItemsを取得
            var furnitureItems = DataManager.Instance.PlayerDataManager.FurnitureItems;
            return furnitureItems.Any(furnitureItem => shopId == furnitureItem);
        }
        
        
        /// <summary>
        /// 購入処理
        /// </summary>
        /// <param name="itemPrice"></param>
        /// <param name="itemId"></param>
        public async void BuyDecision(int itemPrice, int itemId)
        {
            Debug.Log($"現在の仮想通貨: {DataManager.Instance.PlayerDataManager.Coin}");
            //ローディング処理
            var loading = PopupManager.ShowPopup(PopupManager.PopupName.DataLoading);
            if (DataManager.Instance.PlayerDataManager.Coin <= itemPrice)
            {
                PopupManager.ShowErrorPopup("コインが足りません");
                //ローディングから抜ける
                loading.Hide();
                return;
            }
            
            //コイン処理
            var status = await PlayFabPlayerData.ChangeCoinMinus(itemPrice);
            //コイン消費のエラー処理
            if (status == PlayFabPlayerData.PlayerDataStatus.Error)
            {
                PopupManager.ShowErrorPopup("コイン処理が失敗しました");
                //ローディングから抜ける
                loading.Hide();
                return;
            }
            Debug.Log($"仮想通貨: {DataManager.Instance.PlayerDataManager.Coin}");
            
            //アイテムリストの保存
            Debug.Log($"追加する家具: {itemId}");
            var furnitureItems = DataManager.Instance.PlayerDataManager.FurnitureItems;
            furnitureItems.Add(itemId);
            //PlayFabを更新する処理
            status = await PlayFabPlayerData.UpdateItemData();
            if (status == PlayFabPlayerData.PlayerDataStatus.Error)
            {
                PopupManager.ShowErrorPopup("アイテムの追加に失敗しました");
                //ローディングから抜ける
                loading.Hide();
                return;
            }

            //購入処理後アイテムリストを再生成する
            ShopItemRemove();
            ShopItemGeneral();

            //ローディングから抜ける
            loading.Hide();
        }

        private void ShopItemRemove()
        {
            //位置
            var leftContainer = shopItemGeneralTransformLeft;
            var centerContainer = shopItemGeneralTransformCenter;
            var rightContainer = shopItemGeneralTransformRight;
            
            //生成されているアイテムを全て削除
            for (var i = 0; i < leftContainer.childCount; i++)
            {
                Destroy(leftContainer.GetChild(i).gameObject);
            }
            for (var i = 0; i < centerContainer.childCount; i++)
            {
                Destroy(centerContainer.GetChild(i).gameObject);
            }
            for (var i = 0; i < rightContainer.childCount; i++)
            {
                Destroy(rightContainer.GetChild(i).gameObject);
            }

        }
    }
}

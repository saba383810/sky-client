using System.Linq;
using Common;
using Playfab;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Gacha
{
    public class GachaController : MonoBehaviour
    {
        [SerializeField] private GachaAnimationController gachaAnimationController;
        [SerializeField] private Gacha gacha;
        [SerializeField] private GameObject gachaAnimationGameObjectObject;
        [SerializeField] private Button closeButton;
        private void Awake()
        {
            closeButton.onClick.AddListener((() => Destroy(gachaAnimationGameObjectObject)));
        }

        private async void Start()
        { 
            //ガチャの抽選を行う
            var gachaSprite = gacha.DressGacha();
            //TODO データの更新を行う
            PlayerDataUpdate(gachaSprite.name);
            //ガチャのアニメーションを開始する
            gachaAnimationController.GachaAnimation(gachaSprite);
        }

        
        /// <summary>
        /// データ追加し更新処理を行う
        /// </summary>
        /// <param name="gachaSpriteName"></param>
        private async void PlayerDataUpdate(string gachaSpriteName)
        {
            //アイテムリストの保存
            Debug.Log($"追加するドレス: {gachaSpriteName}");
            var dressId = int.Parse(gachaSpriteName); 
            //所持しているかの確認
            var dressItems = DataManager.Instance.PlayerDataManager.DressItems;
            foreach (var VARIABLE in dressItems)
            {
                Debug.Log("所持しているドレス" + VARIABLE);
            }

            var isHaveDress = dressItems.Any(dressItem => dressId == dressItem);
            if (!isHaveDress)
            {
                dressItems.Add(int.Parse(gachaSpriteName));
                //PlayFabを更新する処理
                var status = await PlayFabPlayerData.UpdateItemData();
                if (status != PlayFabPlayerData.PlayerDataStatus.Error) return;
                PopupManager.ShowErrorPopup("アイテムの追加に失敗しました");
            }
            else
            {
                //PlayFabを更新する処理
                await PlayFabPlayerData.UpdateItemData();
                Debug.Log("すでに所持しているアイテム");
            }
        }
    }
}

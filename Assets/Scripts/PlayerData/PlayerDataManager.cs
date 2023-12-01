using System;
using System.Collections.Generic;
using System.Linq;
using Hakoniwa;
using UnityEngine;

namespace PlayerData
{
    public class PlayerDataManager 
    { 
        /// =====================================================
        /// プレイヤーデータ関連
        /// ====================================================
        
        /// <summary>プレイヤーID</summary>
        public string PlayerCustomId { get; set; }
        public string PlayerPhotonId { get; set; }
        /// <summary>プレイヤー名</summary>
        public string PlayerName { get; set; }
        /// <summary>プレイヤーランク</summary>
        public int Rank { get; set; }
        /// <summary>経験値</summary>
        public int Exp { get; set; }
       
        
        /// =====================================================
        /// ドレスアップ関連
        /// ====================================================
        
        ///<summary>髪型の番号</summary>
        public int HairId { get; set; }
        /// <summary>ドレスの番号</summary>
        public int DressId { get; set; }
        /// <summary>アクセサリーの番号</summary>
        public int AccId { get; set; }
        
        /// =====================================================
        /// 所持品関連
        /// ====================================================
        
        /// <summary>コイン</summary>
        public int Coin { get; set; }
        /// <summary>所持しているアイテム</summary>
        public List<int> Items { get; set; }
        /// <summary>所持ヘアスタイル</summary>
        public List<int> HairItems { get; set; }
        /// <summary>所持ドレス</summary>
        public List<int> DressItems { get; set; }
        /// <summary>所持アクセサリー</summary>
        public List<int> AccItems { get; set; }
        /// <summary>所持している家具</summary>
        public List<int> FurnitureItems { get; set; }

        /// <summary>ハコニワのデータ</summary>
        public HakoniwaData HakoniwaData { get; set; }
        public bool InitDone { get; set; }

        public enum ItemType
        {
            Hair,
            Dress,
            Acc,
            Furniture
        }
        
        public string GetItemsData(ItemType itemType)
        {
            List<int> items;
            switch (itemType)
            {
                case ItemType.Hair:
                    items = HairItems;
                    break;
                case ItemType.Dress:
                    items = DressItems;
                    break;
                case ItemType.Acc:
                    items = AccItems;
                    break;
                case ItemType.Furniture:
                    items = FurnitureItems;
                    break;
                default:
                    Debug.LogError("想定していない値が入力されました");
                    throw new Exception();
            }
            return items.Aggregate("", (current, item) => current + (item + ","));
        }
    }
}

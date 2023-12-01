using System.Collections.Generic;

namespace Common.Database
{
    public static class FurnitureDatabase 
    {
        /// <summary>
        /// 家具のデータベース
        /// </summary>
        private static readonly List<FurnitureInfo> FurnitureDataList = new List<FurnitureInfo>()
        {
            //ID:10000 Bedsの情報
            new FurnitureInfo("ダブルベッド",14001,5000),
            new FurnitureInfo("ダブルベッド(高級)",14002,6000),
            new FurnitureInfo("ダブルベッド(赤)",14003,5000),
            new FurnitureInfo("ダブルベッド(赤)(高級)",14004,6000),
            new FurnitureInfo("ダブルベッド(黄)",14005,5000),
            new FurnitureInfo("ダブルベッド(黄)(高級)",14006,6000),
            new FurnitureInfo("ベッド",12007,2500),
            new FurnitureInfo("ベッド(高級)",12008,3000),
            new FurnitureInfo("ベッド(赤)(高級)",12009,3000),
            //ID:20000 Devicesの情報
            new FurnitureInfo("バスタブ",22001,3000),
            new FurnitureInfo("バスタブ(紫)", 22002, 3000),
            new FurnitureInfo("バスタブ(ピンク)", 22003, 3000),
            new FurnitureInfo("バスタブ(白)", 22004, 3000),
            new FurnitureInfo("ジャグジー", 22005, 5000),
            new FurnitureInfo("ジャグジー(オレンジ)", 22006, 5000),
            new FurnitureInfo("ジャグジー(赤)", 22007, 5000),
            
            //ID:30000 Seatsの情報
            new FurnitureInfo("ソファ",31001,1000),
            
            //ID:40000
            //ID:50000 Tableの情報
            new FurnitureInfo("テーブル",52001,2000),
            //ID：60000
        };

        /// <summary>
        /// 家具のリストを全て取得する
        /// </summary>
        /// <returns></returns>
        public static List<FurnitureInfo> GetFurnitureList()
        {
            return FurnitureDataList;
        }
    }

    /// <summary>
    /// 家具のステータス（名前、ID、値段）
    /// IDは６桁ある
    /// [10000の位：種類]10000:Bed 20000:Device 30000:Seats 40000:Stairs 50000:Tables 60000:Wardrobes
    /// [1000の位：サイズ] 1又は2又は4
    /// [100の位：向き]ここでは全て0
    /// [10と1の位：家具の番号] 1~99
    /// </summary>
    public class FurnitureInfo
    {
        /// <summary>
        /// 家具情報
        /// </summary>
        /// <param name="furnitureName"></param>
        /// <param name="furnitureID"></param>
        /// <param name="furniturePrice"></param>
        public FurnitureInfo(string furnitureName,int furnitureID,int furniturePrice)
        {
            FurnitureName = furnitureName;
            FurnitureID = furnitureID;
            FurniturePrice = furniturePrice;
        }
        
        /// <summary>家具の名前</summary>
        public string FurnitureName { get; }
        
        /// <summary>家具のID</summary>
        public int FurnitureID { get; }
        
        /// <summary>家具の値段</summary>
        public int FurniturePrice { get; }
    }
}

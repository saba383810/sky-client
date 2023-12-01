using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Hakoniwa;
using PlayerData;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using UnityEngine;

namespace Playfab
{
    public static class PlayFabPlayerData
    {
        //=================================================================================
        //ユーザーデータの取得
        //=================================================================================
        private static PlayerDataStatus _getAllPlayerDataStatus;
        private static PlayerDataStatus _getUserDataStatus;
        private static PlayerDataStatus _updateUserDataStatus;
        private static PlayerDataStatus _getUserInventoryStatus;
        private static PlayerDataStatus _changeCoinStatus;
        private static PlayerDataManager _tmpPlayerDataManager;
        
        public enum PlayerDataStatus
        {
            Loading,
            Success,
            Error
        }
        
        /// <summary>
        /// すべてのユーザーデータを取得
        /// </summary>
        /// <returns></returns>
        public static async UniTask<PlayerDataStatus> GetAllPlayerData()
        {
            Debug.Log("すべてのプレイヤーデータの更新");
            
            var status = await GetPlayerData();
            if (status == PlayerDataStatus.Error)
            {
                return PlayerDataStatus.Error;
            }
            
            status = await GetUserInventory();
            if (status == PlayerDataStatus.Error)
            {
                return PlayerDataStatus.Error;
            }


            DataManager.Instance.PlayerDataManager.InitDone = true;

            return PlayerDataStatus.Success;
        }

        public static async UniTask<PlayerDataStatus> UpdateAllPlayerData()
        {
            //ステータスデータ
            var status = await UpdateStatusData();
            if (status == PlayerDataStatus.Error) return PlayerDataStatus.Error;
            Debug.Log("ステータスデータの更新 完了");
            
            //アイテムデータ
            status = await UpdateItemData();
            if (status == PlayerDataStatus.Error) return PlayerDataStatus.Error;
            Debug.Log("アイテムデータの更新 完了");
            
            //ドレスデータ
            status = await UpdateDressData();
            if (status == PlayerDataStatus.Error) return PlayerDataStatus.Error;
            Debug.Log("ドレスデータの更新 完了");
            
            //ハコニワデータ
            status = await UpdateHakoniwaData();
            if (status == PlayerDataStatus.Error) return PlayerDataStatus.Error;
            Debug.Log("ハコニワデータの更新 完了");
            
            return PlayerDataStatus.Success;
        }


        /// <summary>
        /// ユーザー(プレイヤー)データの取得
        /// </summary>
        public static async UniTask<PlayerDataStatus> GetPlayerData()
        {
            //GetUserDataRequestのインスタンスを生成
            var request = new GetUserDataRequest();
            
            _getUserDataStatus = PlayerDataStatus.Loading;
            //ユーザー(プレイヤー)データの取得
            Debug.Log($"プレイヤー(ユーザー)データの取得開始");
            PlayFabClientAPI.GetUserData(request, OnSuccessGettingPlayerData, OnLoadingError);
            await UniTask.WaitUntil(() => _getUserDataStatus!= PlayerDataStatus.Loading);
            return _getUserDataStatus;
        }

        //=================================================================================
        //ユーザーデータの取得結果
        //=================================================================================

        //ユーザー(プレイヤー)データの取得に成功
        private static void OnSuccessGettingPlayerData(GetUserDataResult result)
        {
            Debug.Log($"ユーザー(プレイヤー)データの取得に成功しました");
            Debug.Log($"ステータスデータ セット");
            DataManager.Instance.PlayerDataManager.PlayerCustomId = result.Data["1_PlayerId"].Value;
            DataManager.Instance.PlayerDataManager.PlayerName = result.Data["1_UserName"].Value;
            DataManager.Instance.PlayerDataManager.Rank = int.Parse(result.Data["1_Rank"].Value);
            DataManager.Instance.PlayerDataManager.Exp = int.Parse(result.Data["1_Exp"].Value);
            
            // ドレスアップ取得
            Debug.Log($"ドレスデータ セット");
            DataManager.Instance.PlayerDataManager.HairId = int.Parse(result.Data["2_HairId"].Value);
            DataManager.Instance.PlayerDataManager.DressId = int.Parse(result.Data["2_DressId"].Value);
            DataManager.Instance.PlayerDataManager.AccId = int.Parse(result.Data["2_AccId"].Value);
            
            // アイテム
            Debug.Log($"アイテムデータ セット");
            
            //HairItems
            try{DataManager.Instance.PlayerDataManager.HairItems = StringToIntList(result.Data["3_HairItems"].Value);}
            catch(KeyNotFoundException e)
            {Debug.Log("所持しているHairItemがありません"); DataManager.Instance.PlayerDataManager.HairItems = new List<int>(); }
            
            //DressItems
            try{DataManager.Instance.PlayerDataManager.DressItems = StringToIntList(result.Data["3_DressItems"].Value);}
            catch(KeyNotFoundException e)
            {Debug.Log("所持しているDressItemがありません"); DataManager.Instance.PlayerDataManager.DressItems = new List<int>();}
            
            //AccItems
            try{DataManager.Instance.PlayerDataManager.AccItems = StringToIntList(result.Data["3_AccItems"].Value);}
            catch(KeyNotFoundException e)
            {Debug.Log("所持しているAccItemがありません"); DataManager.Instance.PlayerDataManager.AccItems = new List<int>();}
            
            //FurnitureItems
            try
            { DataManager.Instance.PlayerDataManager.FurnitureItems = StringToIntList(result.Data["3_FurnitureItems"].Value); }
            catch (KeyNotFoundException e)
            { Debug.Log("所持しているアイテムがありません。"); DataManager.Instance.PlayerDataManager.FurnitureItems = new List<int>(); }

            //ハコニワデータ
            Debug.Log($"ハコニワデータ セット");

            var xSize = int.Parse(result.Data["4_XSize"].Value);
            var ySize =int.Parse(result.Data["4_YSize"].Value);
            DataManager.Instance.PlayerDataManager.HakoniwaData = new HakoniwaData
            (
                result.Data["4_HakoniwaName"].Value.ToString(),
                int.Parse(result.Data["4_BackGroundData"].Value),
                int.Parse(result.Data["4_WallData"].Value),
                int.Parse(result.Data["4_FloorData"].Value),
                xSize,
                ySize,
                StringToTwoDimArray( result.Data["4_WallAccData"].Value,xSize,ySize),
                StringToTwoDimArray( result.Data["4_FurnitureData"].Value,xSize,ySize)
            );
 
            _getUserDataStatus = PlayerDataStatus.Success;
        }

       

        //=================================================================================
        //ユーザーデータの更新
        //=================================================================================

        /// <summary>
        /// プレイヤーデータの更新
        /// </summary>
        public static async UniTask<PlayerDataStatus> UpdateStatusData()
        {
            //更新するデータ
            var updateDataDict = new Dictionary<string, string>()
            {
                //プレイヤーデータ
                {"1_PlayerId", DataManager.Instance.PlayerDataManager.PlayerCustomId},
                {"1_UserName", DataManager.Instance.PlayerDataManager.PlayerName},
                {"1_Rank", DataManager.Instance.PlayerDataManager.Rank.ToString()},
                {"1_Exp", DataManager.Instance.PlayerDataManager.Exp.ToString()},
            };

            //削除するキー
            var removeKeyList = new List<string>()
            {
                "Key"
            };

            //UpdateUserDataRequestのインスタンスを生成
            var request = new UpdateUserDataRequest
            {
                Data = updateDataDict,
                KeysToRemove = removeKeyList,
                Permission = UserDataPermission.Private //アクセス許可設定
            };

            //ユーザー(プレイヤー)データの更新
            Debug.Log($"プレイヤーデータの更新開始");
            _updateUserDataStatus = PlayerDataStatus.Loading;
            PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerData, OnUpdateError);
            await UniTask.WaitUntil(() => _updateUserDataStatus != PlayerDataStatus.Loading);
            return _updateUserDataStatus;
        }
        
        /// <summary>
        /// ドレスアップデータの更新
        /// </summary>
        public static async UniTask<PlayerDataStatus> UpdateDressData()
        {
            //更新するデータ
            var updateDataDict = new Dictionary<string, string>()
            {
                //ドレスアップ
                {"2_HairId", DataManager.Instance.PlayerDataManager.HairId.ToString()},
                {"2_DressId", DataManager.Instance.PlayerDataManager.DressId.ToString()},
                {"2_AccId", DataManager.Instance.PlayerDataManager.AccId.ToString()},
            };

            //削除するキー
            var removeKeyList = new List<string>()
            {
                "Key"
            };

            //UpdateUserDataRequestのインスタンスを生成
            var request = new UpdateUserDataRequest
            {
                Data = updateDataDict,
                KeysToRemove = removeKeyList,
                Permission = UserDataPermission.Private //アクセス許可設定
            };

            //ユーザー(プレイヤー)データの更新
            Debug.Log($"ドレスデータの更新開始");
            _updateUserDataStatus = PlayerDataStatus.Loading;
            PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerData, OnUpdateError);
            await UniTask.WaitUntil(() => _updateUserDataStatus != PlayerDataStatus.Loading);
            return _updateUserDataStatus;
        }
        
        /// <summary>
        /// アイテムデータの更新
        /// </summary>
        public static async UniTask<PlayerDataStatus> UpdateItemData()
        {
            //更新するデータ
            var updateDataDict = new Dictionary<string, string>()
            {
                // アイテム
                {"3_HairItems", DataManager.Instance.PlayerDataManager.GetItemsData(PlayerDataManager.ItemType.Hair)},
                {"3_DressItems", DataManager.Instance.PlayerDataManager.GetItemsData(PlayerDataManager.ItemType.Dress)},
                {"3_AccItems", DataManager.Instance.PlayerDataManager.GetItemsData(PlayerDataManager.ItemType.Acc)},
                {"3_FurnitureItems", DataManager.Instance.PlayerDataManager.GetItemsData(PlayerDataManager.ItemType.Furniture)}
            };

            //削除するキー
            var removeKeyList = new List<string> {"Key"};

            //UpdateUserDataRequestのインスタンスを生成
            var request = new UpdateUserDataRequest
            {
                Data = updateDataDict,
                KeysToRemove = removeKeyList,
                Permission = UserDataPermission.Private //アクセス許可設定
            };

            //ユーザー(プレイヤー)データの更新
            Debug.Log($"アイテムデータの更新開始");
            _updateUserDataStatus = PlayerDataStatus.Loading;
            PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerData, OnUpdateError);
            await UniTask.WaitUntil(() => _updateUserDataStatus != PlayerDataStatus.Loading);
            return _updateUserDataStatus;
        }
        
         
        /// <summary>
        /// ハコニワデータの更新
        /// </summary>
        public static async UniTask<PlayerDataStatus> UpdateHakoniwaData()
        {
            var hakoniwaName = DataManager.Instance.PlayerDataManager.HakoniwaData.HakoniwaName;
            var backGroundData = DataManager.Instance.PlayerDataManager.HakoniwaData.BackGroundData;
            var wallData = DataManager.Instance.PlayerDataManager.HakoniwaData.WallData;
            var floorData = DataManager.Instance.PlayerDataManager.HakoniwaData.FloorData;
            var wallAccData = DataManager.Instance.PlayerDataManager.HakoniwaData.WallAccData;
            var furnitureData = DataManager.Instance.PlayerDataManager.HakoniwaData.FurnitureData;
            var xSize = DataManager.Instance.PlayerDataManager.HakoniwaData.XSize;
            var ySize = DataManager.Instance.PlayerDataManager.HakoniwaData.YSize;
            
            //更新するデータ
            var updateDataDict = new Dictionary<string, string>()
            {
                // ハコニワデータ
                {"4_HakoniwaName", hakoniwaName},
                {"4_BackGroundData", backGroundData.ToString()},
                {"4_WallData",wallData.ToString()},
                {"4_FloorData",floorData.ToString()},
                {"4_XSize",xSize.ToString()},
                {"4_YSize",ySize.ToString()},
                {"4_WallAccData",TwoDimArrayIntToString(wallAccData,xSize,ySize)},
                {"4_FurnitureData",TwoDimArrayIntToString(furnitureData,xSize,ySize)}
            };

            //削除するキー
            var removeKeyList = new List<string> {"Key"};

            //UpdateUserDataRequestのインスタンスを生成
            var request = new UpdateUserDataRequest
            {
                Data = updateDataDict,
                KeysToRemove = removeKeyList,
                Permission = UserDataPermission.Private //アクセス許可設定
            };

            //ユーザー(プレイヤー)データの更新
            Debug.Log($"ハコニワデータの更新開始");
            _updateUserDataStatus = PlayerDataStatus.Loading;
            PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerData, OnUpdateError);
            await UniTask.WaitUntil(() => _updateUserDataStatus != PlayerDataStatus.Loading);
            return _updateUserDataStatus;
        }
        
        
        //ユーザー(プレイヤー)データの更新に成功
        private static void OnSuccessUpdatingPlayerData(UpdateUserDataResult result)
        {
            Debug.Log($"更新に成功しました");

            //result.ToJsonでjsonで形式で結果を確認可能(result.Dataはない)
            Debug.Log($"{result.ToJson()}");
            _updateUserDataStatus = PlayerDataStatus.Success;
        }
        

        //=================================================================================
        //プレイヤーインベントリの更新
        //=================================================================================

        /// <summary>
        /// インベントリの情報を取得
        /// </summary>
        public static async UniTask<PlayerDataStatus> GetUserInventory(){
            //GetUserInventoryRequestのインスタンスを生成
            var userInventoryRequest = new GetUserInventoryRequest();

            _getUserInventoryStatus = PlayerDataStatus.Loading;
            //インベントリの情報の取得
            Debug.Log($"インベントリの情報の取得開始");
            PlayFabClientAPI.GetUserInventory(userInventoryRequest, OnUserInventorySuccess, OnLoadingError);

            await UniTask.WaitUntil(() => _getUserInventoryStatus != PlayerDataStatus.Loading);
            return _getUserInventoryStatus;
        }
        
        //インベントリの情報の取得に成功
        private static void OnUserInventorySuccess(GetUserInventoryResult result){
            //result.Inventoryがインベントリの情報
            Debug.Log($"インベントリの情報の取得に成功");
    
            //所持している仮想通貨の情報をログで表示
            foreach(var virtualCurrency in result.VirtualCurrency)
            {
                //コイン
                if (virtualCurrency.Key == "CN") DataManager.Instance.PlayerDataManager.Coin = virtualCurrency.Value;
                Debug.Log($"仮想通貨 {virtualCurrency.Key} : {virtualCurrency.Value}");
            }

            _getUserInventoryStatus = PlayerDataStatus.Success;
        }
        

        //インベントリデータの更新
        public static async UniTask<PlayerDataStatus> UpdateUserInventory()
        {
            //更新するデータ
            var updateDataDict = new Dictionary<string, string>()
            {
                {"1_UserName", DataManager.Instance.PlayerDataManager.PlayerName},
                {"1_Rank", DataManager.Instance.PlayerDataManager.Rank.ToString()},
                {"1_Exp", DataManager.Instance.PlayerDataManager.Exp.ToString()},
            };

            //削除するキー
            var removeKeyList = new List<string>
            {
                "Key"
            };

            //UpdateUserDataRequestのインスタンスを生成
            var request = new UpdateUserDataRequest
            {
                Data = updateDataDict,
                KeysToRemove = removeKeyList,
                Permission = UserDataPermission.Private //アクセス許可設定
            };

            //ユーザー(プレイヤー)データの更新
            Debug.Log($"プレイヤー(ユーザー)データの更新開始");
            _updateUserDataStatus = PlayerDataStatus.Loading;
            PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerData, OnUpdateError);
            await UniTask.WaitUntil(() => _updateUserDataStatus != PlayerDataStatus.Loading);
            return _updateUserDataStatus;
        }
        
        /// <summary>
        /// コインを追加
        /// </summary>
        public static async UniTask<PlayerDataStatus> ChangeCoin(int addCoin)
        {
            _changeCoinStatus = PlayerDataStatus.Loading;

            //AddUserVirtualCurrencyRequestのインスタンスを生成
            var addUserVirtualCurrencyRequest = new AddUserVirtualCurrencyRequest {
                Amount          = addCoin,   //追加する金額
                VirtualCurrency = "CN", //仮想通貨のコード
            };
    
            //仮想通貨の追加
            Debug.Log($"仮想通貨の追加開始");
            PlayFabClientAPI.AddUserVirtualCurrency(addUserVirtualCurrencyRequest, OnChangeCoinSuccess, OnUpdateError);

            await UniTask.WaitUntil(() => _changeCoinStatus != PlayerDataStatus.Loading);

            return _changeCoinStatus;

        }

        /// <summary>
        /// コインを消費
        /// </summary>
        public static async UniTask<PlayerDataStatus> ChangeCoinMinus(int subCoin)
        {
            _changeCoinStatus = PlayerDataStatus.Loading;
            
            //subtractUserVirtualCurrencyRequestのインスタンスを生成
            var subtractUserVirtualCurrencyRequest = new SubtractUserVirtualCurrencyRequest()
            {
                Amount = subCoin, //消費する金額
                VirtualCurrency = "CN" //仮想通貨のコード
            };
            
            //仮想通貨の消費
            Debug.Log($"仮想通貨の消費開始");
            PlayFabClientAPI.SubtractUserVirtualCurrency(subtractUserVirtualCurrencyRequest, OnChangeCoinSuccess, OnUpdateError);

            await UniTask.WaitUntil((() => _changeCoinStatus != PlayerDataStatus.Loading));

            return _changeCoinStatus;
        }
        
        private static void OnChangeCoinSuccess(ModifyUserVirtualCurrencyResult result)
        {
            if (result.VirtualCurrency == "CN")
            {
                DataManager.Instance.PlayerDataManager.Coin = result.Balance;
            }
            _changeCoinStatus = PlayerDataStatus.Success;
        }
        
        
        //ユーザー(プレイヤー)データの取得に失敗
        private static void OnLoadingError(PlayFabError error)
        {
            PopupManager.ShowGoToTitleError("データの取得に失敗しました");
            Debug.LogWarning($"ユーザー(プレイヤー)データの取得に失敗しました : {error.GenerateErrorReport()}");
            _getUserDataStatus = PlayerDataStatus.Error;
            _getUserInventoryStatus = PlayerDataStatus.Error;
        }
        
        //ユーザー(プレイヤー)データの取得に失敗
        private static void OnUpdateError(PlayFabError error)
        {
            PopupManager.ShowGoToTitleError("データの読み込みに失敗しました");
            Debug.LogWarning($"ユーザー(プレイヤー)データの更新に失敗しました : {error.GenerateErrorReport()}");
            _changeCoinStatus = PlayerDataStatus.Error;
            _updateUserDataStatus = PlayerDataStatus.Error;
        }
        
        public static List<int> StringToIntList(string intString)
        {
            var items = new List<int>();
            var parseData = "";
            if (!string.IsNullOrEmpty(intString))
            {
                Debug.Log(intString);
                for (var i = 0; i < intString.Length; i++)
                {
                    var tmpStr = intString[i].ToString();
                    if (tmpStr == ",")
                    {
                        items.Add(int.Parse(parseData));
                        parseData = "";
                        continue;
                    }

                    parseData += intString[i];
                }
            }
            return items;
        }

        public static string TwoDimArrayIntToString(int[,] twoDimArray, int xSize,int ySize)
        {
            var data ="";
            for (var x = 0; x < xSize; x++)
            {
                for (var y = 0; y < ySize; y++)
                {
                    data += twoDimArray[x,y];
                    data += ",";
                }
                data += "|";
            }

            return data;
        }
        
        public static int[,] StringToTwoDimArray(string twoDimData, int xSize, int ySize)
        {
            var twoDimArray  = new int[xSize,ySize];
            var x = 0;
            var y = 0;
            var tmpData = "";
            foreach (var charData in twoDimData)
            {
                switch (charData.ToString())
                {
                    case "|":
                        y = 0;
                        x++;
                        continue;
                    case ",":
                        twoDimArray[x, y] += int.Parse(tmpData);
                        tmpData = "";
                        y++;
                        continue;
                    default:
                        tmpData += charData.ToString();
                        break;
                }
            }

            return twoDimArray;
        }
    }
}

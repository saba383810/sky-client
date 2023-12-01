using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Hakoniwa;
using PlayerData;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Playfab
{
    public static class PlayFabLogin
    {   
        /// -----------------------------------
        /// メールログイン
        /// -----------------------------------
        private const string TitleId = "A4BEE";

        public static void EmailSignUp(string email ,string pass,string userName)
        {
            var registerData = new RegisterPlayFabUserRequest()
            {
                TitleId = TitleId,
                Email = email,
                Password = pass,
                Username = userName
            };
        
            PlayFabClientAPI.RegisterPlayFabUser(registerData, result => 
            {
                Debug.Log("アカウントを作成しました");
            }, error => Debug.Log(error.GenerateErrorReport()));
        }

        public static void EmailLogin(string email, string pass)
        {
            var loginData = new LoginWithEmailAddressRequest()
            {
                TitleId = TitleId,
                Email = email,
                Password = pass,
            };
            PlayFabClientAPI.LoginWithEmailAddress(loginData, EmailLoginSuccess, error => Debug.LogError("ログイン失敗"));
        }

        public static void EmailLoginSuccess(LoginResult result)
        {
            Debug.Log($"emailログイン成功I id:{result.PlayFabId}");
        }


        /// -------------------------------------------------
        /// CustomIdログイン
        /// -------------------------------------------------
        
        //アカウントを作成するかどうか
        private static bool _shouldCreateAccount;
      
        //ログイン時に使うID
        private static string _customID;
        private static bool _isLoginDone = false;
        private static LoginStatusType _loginStatusType;
        public enum LoginStatusType
        {
           Success,
           Rename,
           Error,
        }
        
        public static async UniTask<LoginStatusType> CustomIdLogin()
        {
            _isLoginDone = false;
            _customID = LoadCustomID();
            var request = new LoginWithCustomIDRequest { CustomId = _customID,  CreateAccount = _shouldCreateAccount};
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
            
            //ログイン処理が終了するまでまつ
            await UniTask.WaitUntil(()=>_isLoginDone);
            return _loginStatusType;
        }

        private static void CustomIdLoginNext()
        {
            _customID = LoadCustomID();
            var request = new LoginWithCustomIDRequest { CustomId = _customID,  CreateAccount = _shouldCreateAccount};
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }
    
        //ログイン成功
        private static async void OnLoginSuccess(LoginResult result)
        {
            //アカウントを作成しようとしたのに、IDが既に使われていて、出来なかった場合
            if (_shouldCreateAccount && !result.NewlyCreated)
            {
                Debug.LogWarning($"CustomId : {_customID} は既に使われています。");
                CustomIdLoginNext();//ログインしなおし
                return;
            }
        
            //アカウント作成時にIDを保存
            if (result.NewlyCreated)
            {
                SaveCustomID();
                var status = await InitPlayerData();
                if (status == PlayFabPlayerData.PlayerDataStatus.Error)
                {
                    PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, null);
                    OnLoginFailure(new PlayFabError());
                    return;
                }

            }
            Debug.Log($"PlayFabのログインに成功\nPlayFabId : {result.PlayFabId}, CustomId : {_customID}\nアカウントを作成したか : {result.NewlyCreated}");
        
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerData-PlayerName")))
            {
                //playerNameを作成するpopUpを表示
                Debug.Log("名前を決めてください。");
                _loginStatusType = LoginStatusType.Rename;
                _isLoginDone = true;
            }
            else
            {
                //通常のログイン処理
                _loginStatusType = LoginStatusType.Success;
                _isLoginDone = true;
            }
        }

        //ログイン失敗
        private static void OnLoginFailure(PlayFabError error)
        {
            Debug.LogError($"PlayFabのログインに失敗\n{error.GenerateErrorReport()}");
            _loginStatusType = LoginStatusType.Error;
            _isLoginDone = true;
        }
      
        //=================================================================================
        //カスタムIDの取得
        //=================================================================================

        //IDを保存する時のKEY
        private static readonly string CUSTOM_ID_SAVE_KEY = "PlayerData-PlayerCustomId";
      
        //IDを取得
        private static string LoadCustomID() {
            //IDを取得
            var id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

            //保存されていなければ新規生成
            _shouldCreateAccount = string.IsNullOrEmpty(id);
            return _shouldCreateAccount ? GenerateCustomID() : id;
        }

        //IDの保存
        private static void SaveCustomID() 
        {
            PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, _customID);
        }
      
        //=================================================================================
        //カスタムIDの生成
        //=================================================================================
     
        //IDに使用する文字
        private const string IDCharacters = "0123456789abcdefghijklmnopqrstuvwxyz";

        //IDを生成する
        private static string GenerateCustomID()
        {
            const int idLength = 32; //IDの長さ
            var stringBuilder = new StringBuilder(idLength);
            var random = new System.Random();

            //ランダムにIDを生成
            for (var i = 0; i < idLength; i++)
            {
                stringBuilder.Append(IDCharacters[random.Next(IDCharacters.Length)]);
            }

            return stringBuilder.ToString();
        }

        public static bool isRegisterDone = false;
      
        /// <summary>
        /// ユーザ名を登録する
        /// </summary>
        public async static UniTask<LoginStatusType> RegisterUserName(string playerName)
        {
            isRegisterDone = false;
            // 文字の長さ
            if (playerName.Length < 3)
            { 
                Debug.Log("2文字以下は入力できません");
                return LoginStatusType.Error;
            }
            if (playerName.Length > 10)
            {
                Debug.Log("10文字以上は入力できません");
                return LoginStatusType.Error;
            }
        
            //使用できない文字
            if ((from c in playerName let unicodeCategory = char.GetUnicodeCategory(c) where char.IsSurrogate(c) || 
                    unicodeCategory == UnicodeCategory.NonSpacingMark || 
                    unicodeCategory == UnicodeCategory.SpacingCombiningMark || 
                    unicodeCategory == UnicodeCategory.EnclosingMark select c).Any())
            {
                Debug.Log("使用できない文字が含まれています");
                return LoginStatusType.Error;
            }

            //ユーザ名を指定して、UpdateUserTitleDisplayNameRequestのインスタンスを生成
            var request = new UpdateUserTitleDisplayNameRequest{
                DisplayName = playerName
            };

            //ユーザ名の登録
            Debug.Log($"ユーザ名の登録開始");
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnRegisterUserNameSuccess, OnRegisterUserNameFailure);

            await UniTask.WaitUntil(() => isRegisterDone);
            return _loginStatusType;
        }
  
        //ユーザ名の更新成功
        private static void OnRegisterUserNameSuccess(UpdateUserTitleDisplayNameResult result)
        {
            //result.DisplayNameに更新した後のユーザ名が入ってる
            Debug.Log($"ユーザ名の更新が成功しました : {result.DisplayName}");
            DataManager.Instance.PlayerDataManager.PlayerName = result.DisplayName;

            PlayerPrefs.SetString("PlayerData-PlayerName",result.DisplayName);
            DataManager.Instance.PlayerDataManager.PlayerName = result.DisplayName;
            //TODO loginの終了を通知
            _loginStatusType = LoginStatusType.Success;
            isRegisterDone = true;
        }

        //ユーザ名の更新失敗
        private static void OnRegisterUserNameFailure(PlayFabError error)
        {
            Debug.LogError($"ユーザ名の登録に失敗しました\n{error.GenerateErrorReport()}");
            // TODO error popup
            _loginStatusType = LoginStatusType.Error;
            isRegisterDone = true;
        }
    
        //-------------------------------------
        //データの初期化
        //-------------------------------------

        public static async UniTask<PlayFabPlayerData.PlayerDataStatus> InitPlayerData()
        { 
            //PlayerData関連の初期化
            DataManager.Instance.PlayerDataManager.PlayerCustomId =  PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY, _customID);
            DataManager.Instance.PlayerDataManager.PlayerName = null;
            DataManager.Instance.PlayerDataManager.Rank = 1;
            DataManager.Instance.PlayerDataManager.Exp = 0;
            DataManager.Instance.PlayerDataManager.Coin = 0;
            
            //Dress関連の初期化
            DataManager.Instance.PlayerDataManager.HairId = 57;
            DataManager.Instance.PlayerDataManager.DressId = 88;
            DataManager.Instance.PlayerDataManager.AccId = 46;
            
            //Dress関連の初期化
            DataManager.Instance.PlayerDataManager.HairItems = new List<int>{57};
            DataManager.Instance.PlayerDataManager.DressItems = new List<int>{88};
            DataManager.Instance.PlayerDataManager.AccItems = new List<int>{46};
            DataManager.Instance.PlayerDataManager.FurnitureItems = new List<int>();
            
            DataManager.Instance.PlayerDataManager.HakoniwaData = new HakoniwaData(
                "Milia(みりあ)のハコニワ",
                0, 1,
                26, 5, 5,
                new [,]
                {
                    {0,0,0,0,0},
                    {0,0,0,0,0},
                    {0,0,0,0,0},
                    {0,0,0,0,0},
                    {0,0,0,0,0}
                },
                new [,]
                {
                    {31001,0,31003,0,0},
                    {0,0,31002,0,0},
                    {0,0,0,0,0},
                    {14001,14001,12108,31001,0},
                    {14001,14001,12108,0,31001}
                }
            );

            
            var status =  await PlayFabPlayerData.UpdateAllPlayerData();
            return status;
        }
    }
}

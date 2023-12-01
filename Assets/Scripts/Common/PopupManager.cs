
using System.Collections.Generic;
using System.Linq;
using Common.PopupWindow;
using UnityEngine;

namespace Common
{
    public class PopupManager : MonoBehaviour
    {
        /// <summary>
        /// PopupNameに対応したPopupを表示
        /// </summary>
        /// <param name="popupName"></param>
        public static Popup ShowPopup(PopupName popupName)
        {
            var popupParent = GameObject.Find("PopupUI").transform;
            var popupPrefab = (GameObject)Resources.Load (GetPopupPrefabPath(popupName));
            var obj = Instantiate(popupPrefab, popupParent);
            var popup = obj.GetComponent<Popup>();
            popup.Setup();
            popup.Show();
            return popup;
        }

        public static void ShowErrorPopup(string errorText)
        {
            var popupParent = GameObject.Find("PopupUI").transform;
            var popupPrefab = (GameObject)Resources.Load (GetPopupPrefabPath(PopupName.Error));
            var obj = Instantiate(popupPrefab, popupParent);
            var popup = obj.GetComponent<ErrorPopup>();
            popup.Setup(errorText);
            popup.Show();
        }

        public static void ShowGoToTitleError(string error)
        {
            var popupParent = GameObject.Find("PopupUI").transform;
            var popupPrefab = (GameObject)Resources.Load (GetPopupPrefabPath(PopupName.ErrorGoToTitle));
            var obj = Instantiate(popupPrefab, popupParent);
            var popup = obj.GetComponent<GoToTitleErrorPopup>();
            popup.Setup(error);
            popup.Show();
        }

        /// <summary>
        /// パスを取得する関数
        /// </summary>
        private static string GetPopupPrefabPath(PopupName popupName)
        {
            var popupDict = PopupPathDictionary;
            var item = popupDict.FirstOrDefault(item => item.Key == popupName).Value;
            Debug.Assert(item != null , "[Error]存在しないpopup名が入力されました。\nPopupPathDictionaryにPathが登録されているか確認してください。");
            return item;
        }
        public enum PopupName
        {
            Setting,
            LoginCheck,
            Login,
            RegisterPlayerName,
            DataLoading,
            Mission,
            Friend,
            Error,
            ErrorGoToTitle,
            LoadingWindow
        }

        private static readonly Dictionary<PopupName, string> PopupPathDictionary = new ()
        {
            {PopupName.Setting, "Prefabs/Popup/SettingPopup"},
            {PopupName.LoginCheck,"Prefabs/Popup/LoginCheckPopup" },
            {PopupName.Login,"Prefabs/Popup/LoginPopup" },
            {PopupName.RegisterPlayerName,"Prefabs/Popup/RegisterPlayerNamePopup" },
            {PopupName.DataLoading,"Prefabs/Popup/DataLoadingPopup" },
            {PopupName.Mission,"Prefabs/Popup/MissionPopup" },
            {PopupName.Friend,"Prefabs/Popup/FriendPopup" },
            {PopupName.Error,"Prefabs/Popup/ErrorPopup" },
            {PopupName.ErrorGoToTitle,"Prefabs/Popup/GoToTitleErrorPopup" },
            {PopupName.LoadingWindow,"Prefabs/Popup/Loading" },
        };
        
    }
}

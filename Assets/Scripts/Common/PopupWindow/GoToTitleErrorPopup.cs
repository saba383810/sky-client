using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Common.PopupWindow
{
    public class GoToTitleErrorPopup : Popup
    {
        [SerializeField] private TMP_Text errorText = default;
        [SerializeField] private Button goToTitleButton = default; 
        public void Setup(string error)
        {
            base.Setup();
            errorText.text = error+"\nタイトル画面からやりなおしてください";
            goToTitleButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Title");
            });
        }
    }
}

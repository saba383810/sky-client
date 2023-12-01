using Common;
using OutGame.Character;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.WindowController
{
    public class HomeWindow : MonoBehaviour
    {
        [SerializeField] private Button settingButton = default;
        [SerializeField] private Button missionButton = default;
        [SerializeField] private Button friendButton = default;
        [SerializeField] private DressUpImage chara;
        private void Start()
        {
            chara?.Setup();
            Bind();
        }

        private void Bind()
        {
            settingButton.onClick.AddListener(() =>
            {
               PopupManager.ShowPopup(PopupManager.PopupName.Setting);
            });
            missionButton.onClick.AddListener(() =>
            {
                PopupManager.ShowPopup(PopupManager.PopupName.Mission);
            });
            friendButton.onClick.AddListener(() =>
            {
                PopupManager.ShowPopup(PopupManager.PopupName.Friend);
            });
            
        }
    }
}

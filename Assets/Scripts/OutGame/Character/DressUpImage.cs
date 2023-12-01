using PlayerData;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.Character
{
    public class DressUpImage : MonoBehaviour
    {
        [SerializeField] private Image hairImage = default;
        [SerializeField] private Image hairBackImage = default;
        [SerializeField] private Image dressImage = default;
        [SerializeField] private Image accImage = default;

        private const string DressUpPath = "Sprites/DressUp/";
        // Start is called before the first frame update
        public void Setup()
        {
            HairChange(DataManager.Instance.PlayerDataManager.HairId);
            DressChange(DataManager.Instance.PlayerDataManager.DressId);
            AccChange(DataManager.Instance.PlayerDataManager.AccId);
        }
    
        /// <summary>
        /// 髪型の変更を行う
        /// </summary>
        /// <param name="hairId"></param>
        public void HairChange(int hairId)
        {
            hairImage.sprite = Resources.Load<Sprite>($"{DressUpPath}Hair/{hairId}");
            hairBackImage.sprite = Resources.Load<Sprite>($"{DressUpPath}Hair_Back/{hairId}");
            DataManager.Instance.PlayerDataManager.HairId = hairId;
        }

        /// <summary>
        /// ドレスの変更を行う
        /// </summary>
        /// <param name="dressId"></param>
        public void DressChange(int dressId)
        {
            dressImage.sprite = Resources.Load<Sprite>($"{DressUpPath}Dress/{dressId}");
            DataManager.Instance.PlayerDataManager.DressId = dressId;
        }
        /// <summary>
        /// アクセサリーの変更を行う
        /// </summary>
        /// <param name="accId"></param>
        public void AccChange(int accId)
        {
            accImage.sprite = Resources.Load<Sprite>($"{DressUpPath}Acc/{accId}");
            DataManager.Instance.PlayerDataManager.AccId = accId;
        }
    }
}

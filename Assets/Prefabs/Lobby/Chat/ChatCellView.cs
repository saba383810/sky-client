using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class ChatCellView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text messageText;

        public void Setup(string playerName ,string message)
        {
            if (playerName == DataManager.Instance.PlayerDataManager.PlayerName)
            {
                nameText.color = Color.green;
                nameText.text = playerName+"(Mine)";
            }
            else
            {
                nameText.color = Color.white;
                nameText.text = playerName;
            }
          
            messageText.text = message;
            // Layoutを更新します
            var layoutGroup = messageText.transform.parent.GetComponent<LayoutGroup>();
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
            layoutGroup = layoutGroup.transform.parent.GetComponent<LayoutGroup>();
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
            gameObject.SetActive(true);
        }
    }
}

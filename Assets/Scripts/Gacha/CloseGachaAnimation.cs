using UnityEngine;
using UnityEngine.UI;

namespace Gacha
{
    public class CloseGachaAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject gachaAnimationObj;
        [SerializeField] private Button animationDeleteButton;
        private void Start()
        {
            animationDeleteButton.onClick.AddListener((() => Destroy(gachaAnimationObj)));
        }
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopWindowButtonController : MonoBehaviour
    {
        [SerializeField] private GameObject popupShopLeft, popupShopCenter, popupShopRight;
        [SerializeField] private Button leftPopupButton, centerPopupButton, rightPopupButton;
        private void Awake()
        {
            leftPopupButton.onClick.AddListener(() =>
            {
                popupShopLeft.SetActive(true);
                popupShopCenter.SetActive(false);
                popupShopRight.SetActive(false);
            });
            
            centerPopupButton.onClick.AddListener(() =>
            {
                popupShopLeft.SetActive(false);
                popupShopCenter.SetActive(true);
                popupShopRight.SetActive(false);
            });
            
            rightPopupButton.onClick.AddListener(() =>
            {
                popupShopLeft.SetActive(false);
                popupShopCenter.SetActive(false);
                popupShopRight.SetActive(true);
            });
        }
    }
}

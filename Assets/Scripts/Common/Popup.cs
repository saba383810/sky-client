using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
   public class Popup : MonoBehaviour
   {
      [SerializeField,Header("popup Window")] private Transform window;
      [SerializeField,Header("bgPanel")] private CanvasGroup bg;
      [SerializeField,Header("Exit Button")] private Button[] closeButtons;
      private float _animSpeed = 0.2f;

      public virtual void Setup()
      {
         // buttonイベントの解除
         foreach (var button in closeButtons) button.onClick.RemoveAllListeners();
        
         // buttonイベントの登録
         foreach (var button in closeButtons) button.onClick.AddListener(Hide);
      }
      public void Show()
      {
         Debug.Log($"{gameObject.name} Show");
         DOTween.Sequence()
            .OnStart(() =>
            {
               window.localScale = Vector3.zero;
               bg.alpha = 0;
               window.gameObject.SetActive(true);
               bg.gameObject.SetActive(true);
               gameObject.SetActive(true);
            })
            .Append(window.DOScale(Vector3.one, _animSpeed))
            .Join(bg.DOFade(1, _animSpeed));
      }

      public void Hide()
      {
         var myObj = gameObject;
         Debug.Log($"{myObj.name} Hide");
         DOTween.Sequence()
            .Append(window.DOScale(Vector3.zero, _animSpeed))
            .Join(bg.DOFade(0, _animSpeed))
            .OnComplete(() =>
            {
               window.gameObject.SetActive(false);
               bg.gameObject.SetActive(false);
               gameObject.SetActive(false);
               Destroy(myObj);
            });
      }
   }
}

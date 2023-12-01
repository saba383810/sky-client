using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OutGame.Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInformationView : MonoBehaviour
{
    [SerializeField] private DressUpImage charaIcon = default;
    [SerializeField] private TMP_Text userNameText = default;
    [SerializeField] private TMP_Text rankText = default;
    [SerializeField] private TMP_Text expText = default;
    [SerializeField] private Slider expSlider = default;
    private const float AnimSpeed = 0.3f;
    private const int ExpMaxValue = 120;
    
    public void Setup()
    {
        gameObject.SetActive(false);
        charaIcon.Setup();
        userNameText.text = DataManager.Instance.PlayerDataManager.PlayerName;
        rankText.text = DataManager.Instance.PlayerDataManager.Rank.ToString();
        
        //TODO MaxValueを現在のランクから取得
        expSlider.maxValue = ExpMaxValue;
        expSlider.value = DataManager.Instance.PlayerDataManager.Exp;
        expText.text = $"{DataManager.Instance.PlayerDataManager.Exp}/{ExpMaxValue}";
        Show();
    }

    public async void Show()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.4f));
        var userInfoPosition = transform.localPosition;
        var canvasGroup = transform.GetComponent<CanvasGroup>();
        await DOTween.Sequence()
            .OnStart(() =>
            {
                canvasGroup.alpha = 0;
                transform.localPosition += new Vector3(-300, 0, 0);
                gameObject.SetActive(true);
            })
            .AppendInterval(0.2f)
            .Append(transform.DOLocalMoveX(userInfoPosition.x, AnimSpeed))
            .Join(canvasGroup.DOFade(1,AnimSpeed))
            .OnComplete(() =>
            {
                transform.localPosition = userInfoPosition;
            });
    }
   
}

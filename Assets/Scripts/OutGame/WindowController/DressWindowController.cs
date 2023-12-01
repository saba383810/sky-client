using System;
using System.Collections.Generic;
using Common;
using Common.Loading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OutGame.Character;
using PlayerData;
using Playfab;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.WindowController
{
    public class DressWindowController : MonoBehaviour
    {
        [SerializeField] private Button hairEditButton = default;
        [SerializeField] private Button dressEditButton = default;
        [SerializeField] private Button accEditButton = default;
        [SerializeField] private Transform dressUpButtonGroupTransform = default;
        private Button[] _dressUpButtonGroup;
        [SerializeField] private GameObject textBox            = default;
        [SerializeField] private Transform charaTransform      = default;
        [SerializeField] private Transform dressUpWindowParent = default;
        [SerializeField] private TMP_Text dressUpTitleText = default;
        [SerializeField] private GameObject dressUpButtonItem  = default;
        [SerializeField] private Transform dressUpButtonParent = default;
        [SerializeField] private DressUpImage characterDressUp = default;
        [SerializeField] private DressUpImage dressUpImage = default;
        private List<GameObject> _dressUpWindowButtons;
    
        private const float AnimSpeed = 0.3f;
    
    
    
        private void Start()
        {
            BGMManager.Instance.Play(BGMPath.DRESS);
            _dressUpWindowButtons = new List<GameObject>();
            _dressUpButtonGroup = new[] {hairEditButton, dressEditButton, accEditButton};
            WindowControllerAnimation.SlideInAnimation(dressUpButtonGroupTransform);
            hairEditButton.onClick.AddListener(()=>EditButtonClicked(EditType.Hair));
            dressEditButton.onClick.AddListener(()=>EditButtonClicked(EditType.Dress));
            accEditButton.onClick.AddListener(()=>EditButtonClicked(EditType.Acc));
            dressUpImage.Setup();
        }
        /// <summary>
        /// 編集のタイプ
        /// </summary>
        public enum  EditType
        {
            Hair,
            Dress,
            Acc
        }

        /// <summary>
        /// ドレスButtonを押したときの処理
        /// </summary>
        /// <param name="editType">ボタンの種類を格納</param>
        private void EditButtonClicked(EditType editType)
        {
            DisableButtonGroup(_dressUpButtonGroup);
        
            var dressUpWindowBgButton = dressUpWindowParent.GetChild(0).GetComponent<Button>();
            var dressUpWindow = dressUpWindowParent.GetChild(1);
            var tmpDressUpWindowLocalPos = dressUpWindow.localPosition;
            var tmpButtonGroupLocalPos = dressUpButtonGroupTransform.localPosition;
            dressUpWindowBgButton.enabled = false;
            GenerateDressUpWindow(editType);

            if (editType == EditType.Hair) dressUpTitleText.text = "ヘアスタイル";
            else if (editType == EditType.Dress) dressUpTitleText.text = "ドレス";
            else if (editType == EditType.Acc) dressUpTitleText.text = "アクセサリ";
        
            //SlideInAnimation
            DOTween.Sequence()
                .OnStart(() =>
                {
                    dressUpWindow.localPosition += new Vector3(2000, 0, 0);
                    dressUpWindowParent.gameObject.SetActive(true);
                    textBox.SetActive(false);
                })
                .Append(dressUpButtonGroupTransform.DOLocalMove(tmpButtonGroupLocalPos + new Vector3(2000, 0, 0), AnimSpeed))
                .Join(dressUpWindow.DOLocalMove(tmpDressUpWindowLocalPos, AnimSpeed))
                .Join(charaTransform.DOLocalMoveX(-600,AnimSpeed))
                .OnComplete(() =>
                {
                    dressUpButtonGroupTransform.gameObject.SetActive(false);
                    dressUpWindowBgButton.enabled = true;
                });
        
            // 前のScreenに戻るアニメーション
            dressUpWindowBgButton.onClick.RemoveAllListeners();
            dressUpWindowBgButton.onClick.AddListener(()=>CloseEditMode(dressUpWindow,dressUpWindowBgButton,tmpButtonGroupLocalPos,tmpDressUpWindowLocalPos));
        }
        private const string DressUpPath = "Sprites/DressUp/";

        /// <summary>
        /// ドレスアップ変更ウィンドウを生成する
        /// </summary>
        public void GenerateDressUpWindow(EditType editType)
        {
            var items = editType switch
            {
                EditType.Hair => DataManager.Instance.PlayerDataManager.HairItems,
                EditType.Dress => DataManager.Instance.PlayerDataManager.DressItems,
                EditType.Acc => DataManager.Instance.PlayerDataManager.AccItems,
                _ => throw new ArgumentOutOfRangeException(nameof(editType), editType, null)
            };

            foreach (var itemNum in items)
            {
                var instanceObj = Instantiate(dressUpButtonItem, dressUpButtonParent);
                if (editType == EditType.Hair)
                {
                    instanceObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"{DressUpPath}Hair/{itemNum}");
                    instanceObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"{DressUpPath}Hair/{itemNum}");
                    instanceObj.transform.GetChild(0).localPosition = new Vector3(0, -100, 0);
                    instanceObj.transform.GetComponent<Button>().onClick.AddListener(() => characterDressUp.HairChange(itemNum));
                }
                else if (editType == EditType.Dress)
                {
                    instanceObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"{DressUpPath}Dress/{itemNum}");
                    instanceObj.transform.GetChild(0).localPosition = new Vector3(0, 90, 0);
                    instanceObj.transform.GetComponent<Button>().onClick.AddListener(() => characterDressUp.DressChange(itemNum));
                }
                else
                {
                    instanceObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"{DressUpPath}Acc/{itemNum}");
                    instanceObj.transform.GetChild(0).localPosition = new Vector3(0, -100, 0);
                    instanceObj.transform.GetComponent<Button>().onClick.AddListener(() => characterDressUp.AccChange(itemNum));
                }
                instanceObj.SetActive(true);
                _dressUpWindowButtons.Add(instanceObj);
            }
        }

        /// <summary>
        /// 配列に入っているボタンのクリックをすべて有効にする
        /// </summary>
        /// <param name="buttons"></param>
        private void EnableButtonGroup(IEnumerable<Button> buttons)
        {
            foreach (var btn in buttons)
            {
                btn.enabled = true;
            }
        }
    
        /// <summary>
        /// 配列に入っているボタンのクリックをすべて無効にする
        /// </summary>
        /// <param name="buttons"></param>
        private void DisableButtonGroup(IEnumerable<Button> buttons)
        {
            foreach (var btn in buttons)
            {
                btn.enabled = false;
            }
        }

        private async void CloseEditMode(Transform dressUpWindow , Button dressUpWindowBgButton,Vector3 tmpButtonGroupLocalPos,Vector3 tmpDressUpWindowLocalPos)
        {
            var loading = PopupManager.ShowPopup(PopupManager.PopupName.DataLoading);
            //ステータスUIのiconを変更
            GameObject.Find("Character_UI_Status_Icon").GetComponent<DressUpImage>()?.Setup();
            var status = await PlayFabPlayerData.UpdateDressData();

            if (status == PlayFabPlayerData.PlayerDataStatus.Error)
            {
               return;
            }
            
            loading.Hide();
            
            DOTween.Sequence()
                .OnStart(() =>
                {
                    dressUpButtonGroupTransform.gameObject.SetActive(true);
                    dressUpWindowBgButton.enabled =false;
                    textBox.SetActive(false);
                })
                .Append(dressUpButtonGroupTransform.DOLocalMove(tmpButtonGroupLocalPos, AnimSpeed))
                .Join(dressUpWindow.DOLocalMove(tmpDressUpWindowLocalPos + new Vector3(2000, 0, 0), AnimSpeed))
                .Join(charaTransform.DOLocalMoveX(0,AnimSpeed))
                .OnComplete(() =>
                {
                    dressUpWindow.localPosition = tmpDressUpWindowLocalPos;
                    dressUpWindowParent.gameObject.SetActive(false);
                    textBox.SetActive(true);
                    EnableButtonGroup(_dressUpButtonGroup);
                    foreach (var btn in _dressUpWindowButtons)
                    {
                        Destroy(btn);
                    }

                    _dressUpWindowButtons = new List<GameObject>();
                });

        }
    }
}

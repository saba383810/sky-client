using System;
using System.Threading;
using Common;
using Common.Loading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayerData;
using Playfab;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Hakoniwa
{
    public class HakoniwaController : MonoBehaviour
    {
        // Scripts
        [SerializeField] private HakoniwaGenerator _hakoniwaGenerator = default;
        
        // defaultButtonGroup
        [SerializeField] private Button exitButton = default;
        [SerializeField] private Button editButton = default;

        [SerializeField] private Button editModeExitButton = default;
        
        //editToolButtonGroup
        [SerializeField] private Transform arrowButtonsTransform = default;
        
        //object移動矢印ボタン
        [SerializeField] private Button xPlusMoveArrowButton = default;
        [SerializeField] private Button yPlusMoveArrowButton = default;
        [SerializeField] private Button xMinMoveArrowButton  = default;
        [SerializeField] private Button yMinMoveArrowButton  = default;
        [SerializeField] private Button moveSaveButton       = default;
        [SerializeField] private Button moveResetButton      = default;
        
        //editModeかどうか
        private bool _editMode = false;
        private HakoniwaObjData _currentObjData;
        private bool _isAnimation = false;
        private int _deltaMoveX  = 0;
        private int _deltaMoveY = 0;


        private CancellationTokenSource cts; 
        // Start is called before the first frame update
        private void Start()
        {
            BGMManager.Instance.Play(BGMPath.HAKONIWA);
            cts = new CancellationTokenSource();
            
            // playerDataManagerから箱庭データを取得
            var hakoniwaData = DataManager.Instance.PlayerDataManager.HakoniwaData;

            //箱庭を生成
            _hakoniwaGenerator.GenerateHakoniwa(hakoniwaData,cts.Token).Forget();
            
            //戻るボタン
            exitButton.onClick.AddListener(() => LoadingManager.ChangeScene("OutGame"));
            
            //編集もdモード開始
            editButton.onClick.AddListener(() =>
            {
                if (_editMode || _isAnimation) return;
                EditMode(cts.Token).Forget();
                DOTween.Sequence()
                    .OnStart(() =>
                    {
                        _isAnimation = true;
                        editButton.transform.localRotation = Quaternion.Euler(0,0,0);
                        editModeExitButton.transform.localRotation = Quaternion.Euler(0,90,0);
                        editButton.gameObject.SetActive(true);
                        editModeExitButton.gameObject.SetActive(true);
                    })
                    .Append(editButton.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.2f))
                    .Append(editModeExitButton.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f))
                    .OnComplete(() => _isAnimation = false);
            });
            
            // 編集モードを終了
            editModeExitButton.onClick.AddListener(EditModeExit);
            
            xPlusMoveArrowButton.onClick.AddListener(() =>
            {
                if (!CheckMoveOK(MoveDirection.Xplus,hakoniwaData)) return;
                _currentObjData.PosX += 1;
                _deltaMoveX += 1;
                _hakoniwaGenerator.SetFurniturePos(_currentObjData.FurnitureID,_currentObjData.PosX,_currentObjData.PosY,hakoniwaData.XSize,_currentObjData.gameObject,true);
            });
            
            xMinMoveArrowButton.onClick.AddListener(() =>
            {
                if (!CheckMoveOK(MoveDirection.Xmin,hakoniwaData)) return;
                _currentObjData.PosX -= 1;
                _deltaMoveX -= 1;
                _hakoniwaGenerator.SetFurniturePos(_currentObjData.FurnitureID,_currentObjData.PosX,_currentObjData.PosY,hakoniwaData.XSize,_currentObjData.gameObject,true);
            });
            
            yPlusMoveArrowButton.onClick.AddListener(() =>
            {
                if (!CheckMoveOK(MoveDirection.Yplus,hakoniwaData)) return;
                _currentObjData.PosY += 1;
                _deltaMoveY += 1;
                _hakoniwaGenerator.SetFurniturePos(_currentObjData.FurnitureID,_currentObjData.PosX,_currentObjData.PosY,hakoniwaData.XSize,_currentObjData.gameObject,true);
            });
            yMinMoveArrowButton.onClick.AddListener(() =>
            {
                if (!CheckMoveOK(MoveDirection.Ymin,hakoniwaData)) return;
                _currentObjData.PosY -= 1;
                _deltaMoveY -= 1;
                _hakoniwaGenerator.SetFurniturePos(_currentObjData.FurnitureID,_currentObjData.PosX,_currentObjData.PosY,hakoniwaData.XSize,_currentObjData.gameObject,true);
            });
            
            moveSaveButton.onClick.AddListener(() =>
            {
                // HakoniwaDataを変更する処理
                var previousPos = new[] {_currentObjData.PosX-_deltaMoveX, _currentObjData.PosY-_deltaMoveY};
                var editedPos = new[] {_currentObjData.PosX, _currentObjData.PosY};
                var isEditDone = _hakoniwaGenerator.MoveHakoniwaObjects(previousPos,editedPos);
                //設置が完了したら
                if (isEditDone)
                {
                    _deltaMoveX = 0;
                    _deltaMoveY = 0;
                    Debug.Log("家具の移動が完了しました");
                    // arrowButtonの非表示アニメーション
                    DOTween.Sequence()
                        .OnStart(() => _isAnimation = true)
                        .Append(arrowButtonsTransform.DOScale(Vector3.zero, 0.2f))
                        .OnComplete(() =>
                        {
                            arrowButtonsTransform.gameObject.SetActive(false);
                            _isAnimation = false;
                        });
                }
                else
                {
                    Debug.Log("家具を移動できませんでした");
                    //TODO 設置できませんのpopUp
                }

            });
            moveResetButton.onClick.AddListener(() =>
            {
                _hakoniwaGenerator.GenerateHakoniwa(DataManager.Instance.PlayerDataManager.HakoniwaData,cts.Token).Forget();
                EditModeExitAnimation();
                
            });
            
        }

        private async UniTaskVoid EditMode(CancellationToken token)
        {
            _editMode = true;
            //編集モード
            while (_editMode)
            {
                await UniTask.WaitUntil(()=>Input.GetMouseButtonDown(0)||!_editMode, cancellationToken: token);
                if (Camera.main == null||!_editMode) continue;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                var hit2d = Physics2D.GetRayIntersection(ray);

                if (hit2d) 
                {
                    if(!hit2d.transform.CompareTag("Furniture")) continue;
                    _currentObjData = hit2d.transform.gameObject.GetComponent<HakoniwaObjData>();
                    // 選択中のbuttonを表示
                    DOTween.Sequence()
                        .OnStart(() =>
                        {
                            arrowButtonsTransform.localScale = Vector3.zero;
                            arrowButtonsTransform.gameObject.SetActive(true);
                        })
                        .Append(arrowButtonsTransform.DOScale(Vector3.one, 0.2f));
                    
                    Debug.Log($" 取得状況 furnitureID : {_currentObjData.FurnitureID} 配置配列 [{_currentObjData.PosY},{_currentObjData.PosX}]");
                }

                if (hit2d.collider == null)
                {
                    //TODO 移動されていて配置できるならDataを変更
                    continue;
                }

                // objectを取得できた場合
                if (_currentObjData.gameObject == hit2d.transform.gameObject)
                {
                    _currentObjData.ClickedAnimation();
                    //TODO editToolButtonを表示
                    
                }
            }
            //TODO 編集モードの終了 編集ツールの非表示
        }

        private async void EditModeExit()
        {
            if (!_editMode || _isAnimation) return;

            var loading = PopupManager.ShowPopup(PopupManager.PopupName.DataLoading);
            var status = await PlayFabPlayerData.UpdateHakoniwaData();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            loading.Hide();
            if (status == PlayFabPlayerData.PlayerDataStatus.Error)
            {
                PopupManager.ShowGoToTitleError("箱庭の編集データが保存できませんでした");
                return;
            }
            _editMode = false;
            EditModeExitAnimation();
        }


        private void EditModeExitAnimation()
        {
            DOTween.Sequence()
                .OnStart(() =>
                {
                    _isAnimation = true;
                    editButton.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    editModeExitButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    editButton.gameObject.SetActive(true);
                    editModeExitButton.gameObject.SetActive(true);
                })
                .Append(editModeExitButton.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.2f))
                .Join(arrowButtonsTransform.transform.DOScale(Vector3.zero, 0.2f))
                .Append(editButton.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f))
                .OnComplete(() =>
                {
                    _isAnimation = false;
                    _editMode = false;
                    arrowButtonsTransform.gameObject.SetActive(false);
                });
        }

        private enum MoveDirection
        {
            Xplus,
            Yplus,
            Xmin,
            Ymin
        }
        
        private bool CheckMoveOK(MoveDirection moveDirection,HakoniwaData hakoniwaData)
        {
            var furnitureSize = _hakoniwaGenerator.GetFurnitureSize(_currentObjData.FurnitureID);
            var furnitureDirection =  _hakoniwaGenerator.GetFurnitureDirection(_currentObjData.FurnitureID);
            if (furnitureSize == 1)
            {
                switch (moveDirection)
                {
                    case MoveDirection.Xplus:
                        if (_currentObjData.PosX >= hakoniwaData.XSize - 1) return false;
                        break;
                    case MoveDirection.Xmin:
                        if (_currentObjData.PosX <= 0) return false;
                        break;
                    case MoveDirection.Yplus:
                        if (_currentObjData.PosY >= hakoniwaData.YSize - 1) return false;
                        break;
                    case MoveDirection.Ymin:
                        if (_currentObjData.PosY <= 0) return false;
                        break;
                }
            }
            else if (furnitureSize == 2)
            {
                switch (moveDirection)
                {
                    case MoveDirection.Xplus:
                            break;
                    case MoveDirection.Xmin:
                        break;
                    case MoveDirection.Yplus:
                        break;
                    case MoveDirection.Ymin:
                        break;
                }
            }
            else if (furnitureSize == 4)
            {
                switch (moveDirection)
                {
                    case MoveDirection.Xplus:
                            break;
                    case MoveDirection.Xmin:
                        break;
                    case MoveDirection.Yplus:
                        break;
                    case MoveDirection.Ymin:
                        break;
                }
            }

            return true;
        }

        private void OnDestroy()
        {
            cts.Cancel();
        }
    }
}

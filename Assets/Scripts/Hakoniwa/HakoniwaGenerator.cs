using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PlayerData;
using Unity.VisualScripting;
using UnityEngine;

namespace Hakoniwa
{
    public class HakoniwaGenerator : MonoBehaviour
    {
        [SerializeField] private Transform wallParent;
        [SerializeField] private Transform floorParent;
        [SerializeField] private Transform furnitureParent;
        private GameObject[] _hakoniwaObjects = new GameObject[1];
        
        private int hakoniwaObjCnt;
        private HakoniwaData _currentHakoniwaData;
        private CancellationTokenSource cts;
        
        /// <summary>
        /// 家具のズレを保存する構造体
        /// </summary>
        private struct FurniturePivot
        {
            public float X { get; }
            public float Y { get; }
            public FurniturePivot(float x,float y)
            {
                X = x;
                Y = y;
            }
        }
        
        private void Start()
        {
            cts = new CancellationTokenSource();
        }

        /// <summary>
        /// ハコニワを生成する
        /// </summary>
        /// <param name="hakoniwaData"></param>
        public async UniTaskVoid GenerateHakoniwa(HakoniwaData hakoniwaData,CancellationToken token)
        {
            // 箱庭のObjectの初期化
            foreach (var obj in _hakoniwaObjects)
            {
                if (obj == null) continue;
                Destroy(obj.gameObject);
            }
            _hakoniwaObjects = new GameObject[300];
            hakoniwaObjCnt = 0;
            
            //箱庭の生成
            await GenerateFloor(hakoniwaData.FloorData, hakoniwaData.XSize, hakoniwaData.YSize);
            await GenerateWall(hakoniwaData.WallData, hakoniwaData.XSize, hakoniwaData.YSize);
            await GenerateFurniture(hakoniwaData.FurnitureData, hakoniwaData.XSize, hakoniwaData.YSize);
            
            // 箱庭の生成が終了したら現在のHakoniwaDataとして保存する
            _currentHakoniwaData = hakoniwaData;
        }
        
        /// <summary>
        /// 地面を生成する
        /// </summary>
        /// <param name="floorNum"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        private async UniTask<bool> GenerateFloor(int floorNum,int xSize, int ySize)
        {
            Debug.Log("床を生成");
            var floorSprite = Resources.Load<Sprite>($"Sprites/Furniture/Floor/{floorNum}");
            for (var y = 0; y < ySize; y++)
            {
                for (var x = 0; x < xSize; x++)
                {
                    var floorObj = (GameObject)Resources.Load("Prefabs/Hakoniwa/floorObj");
                    var instanceObj = Instantiate(floorObj,floorParent);
                    var spriteRender = instanceObj.GetComponent<SpriteRenderer>();
                    //家具のレイヤーを設定
                    spriteRender.sortingOrder = (y * xSize) + x;
                    //画像を設定
                    spriteRender.sprite = floorSprite;
                    //座標を設定
                    var xPos = (HakoniwaData.XPivot * x) + (-HakoniwaData.XPivot * y);
                    var yPos = (HakoniwaData.YPivot * x) + (HakoniwaData.YPivot * y);
                    instanceObj.transform.localPosition = new Vector2(xPos,yPos);
                    
                    _hakoniwaObjects[hakoniwaObjCnt] = instanceObj;
                    hakoniwaObjCnt++;
                }
            }
            return true;
        }
        /// <summary>
        /// 壁を生成する
        /// </summary>
        /// <param name="wallNum"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        private async UniTask<bool> GenerateWall(int wallNum,int xSize, int ySize)
        {
            Debug.Log("壁を生成");
            var wallRightSprite = Resources.Load<Sprite>($"Sprites/Furniture/Wall/Brick/{wallNum}_Right");
            var wallLeftSprite = Resources.Load<Sprite>($"Sprites/Furniture/Wall/Brick/{wallNum}_Left");
            var wallCenterSprite = Resources.Load<Sprite>($"Sprites/Furniture/Wall/Brick/{wallNum}_Center");
        
            // 真ん中の生成
            var wallCenterObj = (GameObject)Resources.Load("Prefabs/Hakoniwa/wallObj");
            var instanceCenterObj = Instantiate(wallCenterObj,wallParent);
            var centerSpriteRender = instanceCenterObj.GetComponent<SpriteRenderer>();
            centerSpriteRender.sortingOrder = 0;
            centerSpriteRender.sprite = wallCenterSprite;
            instanceCenterObj.transform.localPosition = new Vector2(0,0);
            _hakoniwaObjects[hakoniwaObjCnt] = instanceCenterObj;
            hakoniwaObjCnt++;
        
            //右側の生成
            for (var i = 0; i < xSize-1; i++)
            {
                var wallObj = (GameObject)Resources.Load("Prefabs/Hakoniwa/wallObj");
                var instanceObj = Instantiate(wallObj,wallParent);
                var spriteRender = instanceObj.GetComponent<SpriteRenderer>();
                spriteRender.sortingOrder = i+1;
                spriteRender.sprite = wallRightSprite;
                instanceObj.transform.localPosition = new Vector2(HakoniwaData.XPivot * (i + 1), HakoniwaData.YPivot * (i + 1));
                _hakoniwaObjects[hakoniwaObjCnt] = instanceObj;
                hakoniwaObjCnt++;
            }
        
            //左側の生成
            for (var i = 0; i < ySize-1; i++)
            {
                var wallObj = (GameObject)Resources.Load("Prefabs/Hakoniwa/wallObj");
                var instanceObj = Instantiate(wallObj,wallParent);
                var spriteRender = instanceObj.GetComponent<SpriteRenderer>();
                spriteRender.sortingOrder = i+1;
                spriteRender.sprite = wallLeftSprite;
                instanceObj.transform.localPosition = new Vector2(HakoniwaData.XPivot * -(i + 1), HakoniwaData.YPivot * (i + 1));
                _hakoniwaObjects[hakoniwaObjCnt] = instanceObj;
                hakoniwaObjCnt++;
            }
            return true;
        }

        /// <summary>
        /// 家具を生成する
        /// </summary>
        /// <param name="furnitureData"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        private async UniTask<bool> GenerateFurniture(int[,] furnitureData,int xSize ,int ySize)
        {
            Debug.Log("家具を生成");
            
            
            var furniturePlacementDone = new bool[furnitureData.GetLength(0),furnitureData.GetLength(1)];
            for (var y = 0; y < ySize; y++)
            {
                for (var x = 0; x < xSize; x++)
                {
                    var furnitureID =  furnitureData[y, x];
                    //家具の番号が0だったら何もしない
                    if (furnitureID == 0 || furniturePlacementDone[y,x]) continue;
                    
                    //家具を取得
                    var furnitureSprite = GetFurnitureSprite(furnitureID);
                    //家具の向きを取得
                    var furnitureDirection = GetFurnitureDirection(furnitureID);

                    //　家具を生成
                    var furnitureObj = (GameObject)Resources.Load("Prefabs/Hakoniwa/furnitureObj");
                    var instanceObj = Instantiate(furnitureObj,furnitureParent);
                    var spriteRender = instanceObj.GetComponent<SpriteRenderer>();

                    //家具のspriteを設定
                    spriteRender.sprite = furnitureSprite;

                    //家具のコライダーの設定
                    instanceObj.transform.AddComponent<PolygonCollider2D>();
                    
                    //家具の配列情報の設定
                    var hakoniwaObjData = instanceObj.transform.AddComponent<HakoniwaObjData>();
                    hakoniwaObjData.PosX = x;
                    hakoniwaObjData.PosY = y;
                    hakoniwaObjData.FurnitureID = furnitureID;
                    
                    //家具の向きを設定
                    var tmpLocalScale = instanceObj.transform.localScale;
                    tmpLocalScale.x *= furnitureDirection;
                    
                    //もし家具がベットなら反転
                    if (GetFurnitureTypeNum(furnitureID) == 1)tmpLocalScale.x *= -1;
                    instanceObj.transform.localScale = tmpLocalScale;
                    
                    // 家具の位置をセット
                    SetFurniturePos(furnitureID,x,y,xSize,instanceObj,false);
                    
                    //インスタンス化したobjectを保持
                    _hakoniwaObjects[hakoniwaObjCnt] = instanceObj;
                    hakoniwaObjCnt++;
                    //配置が終了した座標を保持
                    furniturePlacementDone = SetFurniturePlacementDone(furniturePlacementDone,furnitureID,x,y);
                }
            }
            return true;
        }

        /// <summary>
        /// ID,座標,家具を送ることで正しい位置に配置する処理
        /// </summary>
        /// <param name="furnitureID">家具のID</param>
        /// <param name="x">配置したい配列のx座標</param>
        /// <param name="y">配置したい配列のy座標</param>
        /// <param name="maxX">配列のx座標の最大値</param>
        /// <param name="instanceObj">座標を設定したobj</param>
        public void SetFurniturePos(int furnitureID,int x,int y,int maxX,GameObject instanceObj,bool isEditingObj)
        {
            // 家具の向きを取得
            var furnitureDirection = GetFurnitureDirection(furnitureID);
            // 家具のズレを取得
            var furniturePivots = GetFurniturePivot(furnitureID);
            // spriteRenderを取得
            var spriteRender = instanceObj.GetComponent<SpriteRenderer>();
            // 家具の座標を計算
            var posX = (HakoniwaData.XPivot * x) + (-HakoniwaData.XPivot * y) + furniturePivots.X;
            var posY = (HakoniwaData.YPivot * x) + (HakoniwaData.YPivot * y) + furniturePivots.Y;
            
            //家具のレイヤーを設定
            if (isEditingObj)
            {
                //編集中のobjは常に前に出るように設置Objectより+1の値を入れる
                spriteRender.sortingOrder = (y * maxX*2) + (x*2+1);
            }
            else
            {
                //編集中objより下に行くようにセット
                spriteRender.sortingOrder = (y * maxX*2) + (x*2);
            }

            //家具を反転していて2マス家具ならpositionをずらす
            if (furnitureDirection == -1 && GetFurnitureSize(furnitureID) == 2) { posX -= 0.5f; posY += 0.02f; }
                    
            //座標を代入
            instanceObj.transform.localPosition = new Vector2(posX,posY);
        }
        
        /// <summary>
        /// 家具IDを送ることで、向きを取得する
        /// </summary>
        /// <param name="furnitureID"></param>
        /// <returns></returns>
        public int GetFurnitureDirection(int furnitureID)
        {
            return ((furnitureID % 10000) % 1000) / 100 == 0 ? 1 : -1;
        }
        /// <summary>
        /// 家具のサイズを返す
        /// </summary>
        /// <param name="furnitureID"></param>
        /// <returns></returns>
        public int GetFurnitureSize(int furnitureID)
        {
            return (furnitureID % 10000) / 1000;
        }
        /// <summary>
        /// 家具の種類の番号を返す
        /// </summary>
        /// <param name="furnitureID"></param>
        /// <returns></returns>
        private int GetFurnitureTypeNum(int furnitureID)
        {
            return furnitureID / 10000;
        }

        /// <summary>
        /// 家具のIDを送ることで家具のSpriteを返す
        /// </summary>
        /// <param name="furnitureID"></param>
        /// <returns></returns>
        private Sprite GetFurnitureSprite(int furnitureID)
        {
            //10000の位を確認し家具の種類を取得する
            var furnitureTypeName = (furnitureID / 10000) switch
            {
                1 => "Beds",
                2 => "Device",
                3 => "Seats",
                4 => "Stairs",
                5 => "Tables",
                6 => "Wardrobes",
                _ => ""
            };

            //100以下の値を確認し家具の番号を取得
            var furnitureNum = (furnitureID % 10000) % 1000 % 100;

            return Resources.Load<Sprite>($"Sprites/Furniture/{furnitureTypeName}/{furnitureNum}");
        }
        /// <summary>
        /// 家具のズレを取得
        /// </summary>
        /// <param name="furnitureID"></param>
        /// <returns></returns>
        private FurniturePivot GetFurniturePivot(int furnitureID)
        {
            //10000の位を確認し家具の種類を取得する
            var furnitureTypeName = (furnitureID / 10000) switch
            {
                1 => "Beds",
                2 => "Devices",
                3 => "Seats",
                4 => "Stairs",
                5 => "Tables",
                6 => "Wardrobes",
                _ => ""
            };

            //100以下の値を確認し家具の番号を取得
            var furnitureNum = (furnitureID % 10000) % 1000 % 100;
            
            if (furnitureTypeName == "Beds")
            {
                switch (furnitureNum)
                {
                    case >= 1 and  <= 6 :
                        return new FurniturePivot(0f,-0.15f);
                    case >= 7 and <= 12 :
                        return new FurniturePivot(0.25f,-0.02f);
                }
            }
            else if (furnitureTypeName == "Device")
            {
                
            }
            else if (furnitureTypeName == "Seats")
            {
                switch (furnitureNum)
                {
                    case >= 1 and  <= 6 :
                        return new FurniturePivot(0f,0.15f);
                }
            }
            else if (furnitureTypeName == "Stairs")
            {
                
            }
            else if (furnitureTypeName == "Tables")
            {
                switch (furnitureNum)
                {
                    case >= 1 and  <= 10 :
                        return new FurniturePivot(0.25f,-0.04f);
                }
            }
            else if (furnitureTypeName == "Wardrobes")
            {
                
            }
            
            //当てはまらないobjだったら
            Debug.LogWarning($"pivotが登録されていない家具です。\nID:{furnitureID}");
            return new FurniturePivot(1.0f,1.0f);
        }

        /// <summary>
        /// 家具の配置が終了した配列にtureを入れて返す
        /// </summary>
        /// <param name="furniturePlacementDone">家具の配置終了状況を保存する配列</param>
        /// <param name="furnitureID">家具のID</param>
        /// <param name="x">操作中のx座標</param>
        /// <param name="y">操作中のy座標</param>
        /// <returns></returns>
        private bool[,] SetFurniturePlacementDone(bool[,] furniturePlacementDone,int furnitureID ,int x,int y)
        {
            //初期化
            var furnitureSize = GetFurnitureSize(furnitureID);
            var furnitureDirection = GetFurnitureDirection(furnitureID);

            furniturePlacementDone[y,x] = true;
            switch (furnitureSize)
            {
                // 1マス家具ならreturn
                case 1:
                    return furniturePlacementDone;
                //2マス家具なら残り1マスを埋める
                case 2:
                {
                    if (furnitureDirection == -1) 
                        furniturePlacementDone[y+1,x] = true;
                    else
                        furniturePlacementDone[y,x+1] = true;
                    
                    return furniturePlacementDone;
                }
                //4マス家具だったら残り3マスを埋める
                case 4:
                    furniturePlacementDone[y + 1, x + 1] = true;
                    furniturePlacementDone[y+1,x] = true;
                    furniturePlacementDone[y,x+1] = true;
                    return furniturePlacementDone;
                default:
                    Debug.LogError("予期せぬIDです");
                    throw new Exception();
            }
        }
        /// <summary>
        /// 前の位置と移動後の位置を送ることで配置データを変更(正しく設置できたならtrueを返す)
        /// </summary>
        /// <param name="previousPos">前の配置のデータ [0] ,[1] = x , y</param>
        /// <param name="movePos">移動後の配置のデータ [0] ,[1] = x , y</param>
        public bool MoveHakoniwaObjects(IReadOnlyList<int> previousPos , IReadOnlyList<int> movePos)
        {
            var previousPosX = previousPos[0];
            var previousPosY = previousPos[1];
            var movePosX = movePos[0];
            var movePosY = movePos[1];

            var tmpCurrentHakoniwaData = _currentHakoniwaData;
            var moveFurnitureID = tmpCurrentHakoniwaData.FurnitureData[previousPosY, previousPosX];
            var moveFurnitureSize = GetFurnitureSize(moveFurnitureID);
            var moveFurnitureDirection = GetFurnitureDirection(moveFurnitureID);
            
            Debug.Log($"家具ID[{moveFurnitureID}]を座標[{previousPosY},{previousPosX}]から座標[{movePosY},{movePosX}]に移動します");
            
            //家具のサイズが1だったら
            if (moveFurnitureSize == 1)
            {
                //配列のデータを削除
                tmpCurrentHakoniwaData.FurnitureData[previousPosY, previousPosX] = 0;
                
                //設置できるかチェック
                if (tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX] != 0) return false;
                
                //配置
                tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX] = moveFurnitureID;

            }
            //家具のサイズが2だったら
            else if(moveFurnitureSize == 2)
            {
                //削除
                tmpCurrentHakoniwaData.FurnitureData[previousPosY, previousPosX] = 0;
                tmpCurrentHakoniwaData.FurnitureData[previousPosY+1, previousPosX] = 0;
                
                //家具の向きによって配置位置を変更
                if (moveFurnitureDirection == -1)
                {
                    //削除
                    tmpCurrentHakoniwaData.FurnitureData[previousPosY, previousPosX] = 0;
                    tmpCurrentHakoniwaData.FurnitureData[previousPosY+1, previousPosX] = 0;
                    
                    //設置できるかチェック
                    if (tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX] != 0) return false;
                    if (tmpCurrentHakoniwaData.FurnitureData[movePosY + 1, movePosX] != 0) return false;
                    
                    //配置
                    tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX] = moveFurnitureID;
                    tmpCurrentHakoniwaData.FurnitureData[movePosY + 1, movePosX] = moveFurnitureID;
                }
                else
                {
                    //削除
                    tmpCurrentHakoniwaData.FurnitureData[previousPosY, previousPosX] = 0;
                    tmpCurrentHakoniwaData.FurnitureData[previousPosY, previousPosX+1] = 0;
                    
                    // 設置できるかチェック
                    if (tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX] != 0) return false;
                    if (tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX+1] != 0) return false;
                    
                    //配置
                    tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX] = moveFurnitureID; 
                    tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX+1] = moveFurnitureID;
                }
            }
            //家具のサイズが４だったら
            else
            {
                //削除
                tmpCurrentHakoniwaData.FurnitureData[previousPosY, previousPosX] = 0;
                tmpCurrentHakoniwaData.FurnitureData[previousPosY+1, previousPosX] = 0;
                tmpCurrentHakoniwaData.FurnitureData[previousPosY, previousPosX+1] = 0;
                tmpCurrentHakoniwaData.FurnitureData[previousPosY+1, previousPosX+1] = 0;
                
                //設置できるかをチェック
                if (tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX] != 0) return false;
                if (tmpCurrentHakoniwaData.FurnitureData[movePosY+1, movePosX] != 0) return false;
                if (tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX+1] != 0) return false;
                if (tmpCurrentHakoniwaData.FurnitureData[movePosY+1, movePosX+1] != 0) return false;
                
                //配置
                tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX] = moveFurnitureID;
                tmpCurrentHakoniwaData.FurnitureData[movePosY+1, movePosX] = moveFurnitureID;
                tmpCurrentHakoniwaData.FurnitureData[movePosY, movePosX+1] = moveFurnitureID;
                tmpCurrentHakoniwaData.FurnitureData[movePosY+1, movePosX+1] = moveFurnitureID;
            }
            
            //正常終了したらデータを反映
            _currentHakoniwaData = tmpCurrentHakoniwaData;
            //箱庭を再生成
            GenerateHakoniwa(_currentHakoniwaData,cts.Token).Forget();
            //playerDataに登録
            DataManager.Instance.PlayerDataManager.HakoniwaData = _currentHakoniwaData;
            return true;
        }

        private void OnDestroy()
        {
            cts.Cancel();
        }
    }
}

namespace Hakoniwa
{
    public class HakoniwaData
    {
        public string HakoniwaName { get; set; }
        public int BackGroundData { get; set; }
        public int WallData { get; set; }
        public int FloorData { get; set; }
        public int[,] WallAccData { get; set; }
        public int[,] FurnitureData { get; set; }
        public int XSize { get; set; }
        public int YSize { get; set; }
        public const float XPivot = 0.5f;
        public const float YPivot = -0.25f;


        /// <summary>
        /// 箱庭データの初期化
        /// </summary>
        /// <param name="hakoniwaName">箱庭の名前</param>
        /// <param name="backGroundData">背景画像の番号</param>
        /// <param name="wallData">壁の画像番号</param>
        /// <param name="wallAccData">壁の装飾品の配置情報</param>
        /// <param name="floorData">地面の画像番後</param>
        /// <param name="furnitureData">家具の配置情報</param>
        /// <param name="xSize">横幅</param>
        /// <param name="ySize">縦幅</param>
        /// 
        public HakoniwaData(string hakoniwaName,int backGroundData,int wallData, int floorData ,int xSize, int ySize, int[,] wallAccData , int[,] furnitureData)
        {
            HakoniwaName = hakoniwaName;
            BackGroundData = backGroundData;
            WallData = wallData;
            FloorData = floorData;
            FurnitureData = furnitureData;
            XSize = xSize;
            YSize = ySize;
            WallAccData = wallAccData;
        }

        public int GetFurnitureData(int xPos,int yPos)
        {
            return FurnitureData[xPos, yPos];
        }
    }
}
    

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
namespace MyFarm.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        [Header("种植瓦片信息切换")] 
        public RuleTile digTile;
        public RuleTile waterTile;
        private Tilemap digTilemap;
        private Tilemap waterTilemap;
        [Header("地图信息")] public List<MapData_SO> mapDataList;
        //场景名字+坐标和对应瓦片名字信息
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();
        private Grid currentGrid;
        private Season currentSeason;
        
        private void OnEnable()
        {
            EventHandler.ExcuteActionAfterAnimation += OnExcuteActionAfterAnimation;
            EventHandler.AfterSceneEvent += OnAfterSceneEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMap += RefreshMap;
        }
        private void OnDisable()
        {
            EventHandler.ExcuteActionAfterAnimation -= OnExcuteActionAfterAnimation;
            EventHandler.AfterSceneEvent -= OnAfterSceneEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= RefreshMap;
        }
        private void Start()
        {
            foreach (var mapData in mapDataList)
            {
                InitTileDetailsDict(mapData);
            }
        }
        /// <summary>
        /// 根据地图信息初始化字典
        /// </summary>
        /// <param name="mapData"></param>
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    gridX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y,
                };
                //字典的key
                string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.sceneName;
                if (GetTileDetails(key)!=null)//已经有这个瓦片了 先获取
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                    case GridType.FishArea:
                        tileDetails.canFish = tileProperty.boolTypeValue;
                        break;
                }
                tileDetails.filedType=tileProperty.filedType;

                if (GetTileDetails(key)!=null)//已经有这个瓦片了 更新
                {
                    tileDetailsDict[key]=tileDetails;
                }
                else//没有瓦片 添加
                {
                    tileDetailsDict.Add(key, tileDetails);
                }
            }
        }
        /// <summary>
        /// 根据key返回瓦片信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key];
            }
            return null;
        }
        /// <summary>
        /// 根据鼠标网格坐标返回瓦片信息
        /// </summary>
        /// <param name="mouseGridPos"></param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y+"y"+SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }
        /// <summary>
        /// 根据共有对象池里的对象返回其对应瓦片信息
        /// </summary>
        /// <returns></returns>
        public TileDetails GetTileDetailsByItemPosition(Transform itemPos)
        {
            Vector3Int Pos = currentGrid.WorldToCell(itemPos.position);
            string key = Pos.x + "x" + Pos.y+"y"+SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }
        /// <summary>
        /// 每天执行一次
        /// </summary>
        /// <param name="day"></param>
        /// <param name="season"></param>
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daysSinceWatered>-1)
                {
                    tile.Value.daysSinceWatered=-1;
                }

                if (tile.Value.daysSinceDug>-1)
                {
                    tile.Value.daysSinceDug++;
                }
                //超期且没种销毁耕地
                if (tile.Value.daysSinceDug>3&&tile.Value.seedItemID==-1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                    //QuestManager.Instance.UpdateQuestProgress("锄地个数",-1);
                }

                if (tile.Value.seedItemID>-1)
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();
        }
        private void OnAfterSceneEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            digTilemap=GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTilemap=GameObject.FindWithTag("Water").GetComponent<Tilemap>();
            RefreshMap();
        }
        private void OnExcuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);
            if (currentTile != null)
            {
                //WORKFLOW:实际的使用流程 不用考虑是否有在cursor里已经判断了
                switch (itemDetails)
                {
                    case Consumable {consumableType: Consumable.ConsumableType.Seed}:
                        EventHandler.CallPlantSeedEvent(itemDetails.ItemID,currentTile);
                        break;
                    case Consumable {CanDropped: true}:
                        EventHandler.CallDropItemEvent(itemDetails.ItemID,mouseWorldPos);
                        break;
                    case Equipment {equipmentType: Equipment.EquipmentType.HoeTool}:
                        SetDigTile(currentTile);
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        currentTile.daysSinceDug = 0;
                        //任务：锄地调用检测
                        QuestManager.Instance.UpdateQuestProgress("锄地个数",1);
                        //音效
                        break;
                    case Equipment {equipmentType: Equipment.EquipmentType.WaterTool}:
                        SetWaterTile(currentTile);
                        currentTile.daysSinceWatered = 0;
                        //音效
                        break;
                    case Equipment {equipmentType: Equipment.EquipmentType.CollectTool}:
                        Crop currentCrop = GetCropObj(mouseWorldPos);
                        //执行收割 方法
                        currentCrop.ProcessToolAction(itemDetails,currentTile);
                        break;
                    case Equipment {equipmentType: Equipment.EquipmentType.FishingTool}:
                        if (FishManager.Instance.fishGameState == FishState.Empty)//蓄力
                        {
                            EventHandler.CallFishRodAccumulation();
                        }
                        else if (FishManager.Instance.fishGameState == FishState.Charge)//甩钩
                        {
                            Debug.Log(mouseWorldPos);
                            EventHandler.CallThrowFishRod(mouseWorldPos);
                        }
                        break;
                       
                }
                UpdateTileDetails(currentTile);
            }
        }
        /// <summary>
        /// 通过物理方法判断鼠标点击位置的农作物
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <returns></returns>
        private Crop GetCropObj(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders=Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop=null;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                {
                    currentCrop=colliders[i].GetComponent<Crop>();
                }
            }
            return currentCrop;
        }
        /// <summary>
        /// 修改挖坑瓦片
        /// </summary>
        /// <param name="tileDetails"></param>
        private void SetDigTile(TileDetails tileDetails)
        {
            var pos=new Vector3Int(tileDetails.gridX,tileDetails.gridY,0);
            if (digTile!=null)
            {
                digTilemap.SetTile(pos,digTile);
            }
        }
        /// <summary>
        /// 获取挖坑的瓦片的个数
        /// </summary>
        /// <returns></returns>
        public int GetDigTileCount()
        {
            int count = 0;
        
            // 遍历Tilemap中的每个Cell位置
            foreach (var pos in digTilemap.cellBounds.allPositionsWithin)
            {
                // 获取当前格子的Tile
                TileBase tile = digTilemap.GetTile(pos);
                // 判断是否是目标Tile
                if (tile == digTile)
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 修改浇水瓦片
        /// </summary>
        /// <param name="tileDetails"></param>
        private void SetWaterTile(TileDetails tileDetails)
        {
            var pos=new Vector3Int(tileDetails.gridX,tileDetails.gridY,0);
            if (waterTile!=null)
            {
                waterTilemap.SetTile(pos,waterTile);
            }
        }
        /// <summary>
        /// 获取浇水的的瓦片的个数
        /// </summary>
        /// <returns></returns>
        public int GetWaterTileCount()
        {
            int count = 0;
        
            // 遍历Tilemap中的每个Cell位置
            foreach (var pos in waterTilemap.cellBounds.allPositionsWithin)
            {
                // 获取当前格子的Tile
                TileBase tile = waterTilemap.GetTile(pos);
            
                // 判断是否是目标Tile
                if (tile == waterTile)
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 更新瓦片信息
        /// </summary>
        /// <param name="tileDetails"></param>
        private void UpdateTileDetails(TileDetails tileDetails)
        {
            string key=tileDetails.gridX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key]=tileDetails;
            }
        }
        /// <summary>
        /// 刷新地图信息
        /// </summary>
        private void RefreshMap()
        {
            if (digTilemap!=null)
                digTilemap.ClearAllTiles();
            if (waterTilemap!=null)
                waterTilemap.ClearAllTiles();
            foreach (var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }
            DisplayMap(SceneManager.GetActiveScene().name);
        }
        /// <summary>
        /// 显示地图瓦片
        /// </summary>
        /// <param name="sceneName"></param>
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;
                if (key.Contains(sceneName))//找到当前场景 更新状态
                {
                    if (tileDetails.daysSinceDug>-1)
                    {
                        SetDigTile(tileDetails);
                    }
                    if (tileDetails.daysSinceWatered>-1)
                    {
                        SetWaterTile(tileDetails);
                    }

                    if (tileDetails.seedItemID>-1)
                    {
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemID,tileDetails);    
                    }
                    
                }
            }
            
        }
        /// <summary>
        /// 根据鼠标世界坐标获取瓦片信息
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <returns></returns>
        public TileDetails GetTileDetailsFromWorldPosition(Vector3 mouseWorldPos)
        {
            // 将世界坐标转换为网格坐标
            Vector3Int mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

            // 使用网格坐标查找相应的瓦片信息
            return GetTileDetailsOnMousePosition(mouseGridPos);
        }

        public int GetDistanceToShore(TileDetails currentTile)
        {
            // 定义四个方向：右、左、上、下
            Vector3Int[] directions = {
                new Vector3Int(1, 0, 0),  // 右
                new Vector3Int(-1, 0, 0), // 左
                new Vector3Int(0, 1, 0),  // 上
                new Vector3Int(0, -1, 0)  // 下
            };
            Vector3Int cellPosition=new Vector3Int(currentTile.gridX,currentTile.gridY,0);
            // 使用队列进行 BFS
            Queue<(Vector3Int position, int distance)> queue = new Queue<(Vector3Int position, int distance)>();
            queue.Enqueue((cellPosition, 0)); // 初始位置，距离为 0
            
            // 记录已访问的网格
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
            visited.Add(cellPosition);
            // BFS 主循环
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                Vector3Int pos = current.position;
                int distance = current.distance;
                // 检查四个方向
                foreach (var dir in directions)
                {
                    Vector3Int neighborPos = pos + dir;
                    // 获取邻居网格的 TileDetails
                    TileDetails neighborTile = GetTileDetailsOnMousePosition(neighborPos);
                  
                    if (neighborTile!=null)
                    {
                        // 如果邻居网格是岸边，返回距离 + 1
                        if (!neighborTile.canFish)
                            return distance + 1;

                        // 如果未访问过，加入队列
                        if (!visited.Contains(neighborPos))
                        {
                            visited.Add(neighborPos);
                            queue.Enqueue((neighborPos, distance + 1));
                        }
                    }
                }
            }

            // 如果没有找到岸边，返回 -1
            return -1;
        }
    }
}
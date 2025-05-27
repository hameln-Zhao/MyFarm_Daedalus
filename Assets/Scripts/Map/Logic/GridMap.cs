using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyFarm.Map
{
    //挂载在每一个瓦片地图上
    [ExecuteInEditMode] //在编辑的模式下运行
    public class GridMap : MonoBehaviour
    {
        public MapData_SO MapData;
        public GridType GridType;
        public int FiledType;
        private Tilemap currentTilemap;

        //地图被关闭的时候读所有的数据并存储
        private void OnEnable()
        {
            if (!Application.IsPlaying(this))
            {
                currentTilemap = GetComponent<Tilemap>();
                if (MapData != null)
                {
                    MapData.tileProperties.Clear();
                }
            }
        }

        private void OnDisable()
        {
            if (!Application.IsPlaying(this))
            {
                currentTilemap = GetComponent<Tilemap>();
                UpdateTileProperties();
#if UNITY_EDITOR
                if (MapData != null)
                    EditorUtility.SetDirty(MapData); //标记为脏方便实时修改
#endif
            }
        }

        private void UpdateTileProperties()
        {
            currentTilemap.CompressBounds(); //规定瓦片地图实际绘制的范围(10x10里有2x2有瓦片那就是2x2)
            if (!Application.IsPlaying(this))
            {
                if (MapData != null)
                {
                    //已绘制范围的左下角坐标
                    Vector3Int startPos = currentTilemap.cellBounds.min;
                    //已绘制范围的右上角坐标
                    Vector3Int endPos = currentTilemap.cellBounds.max;
                    for (int x = startPos.x; x < endPos.x; x++)
                    {
                        for (int y = startPos.y; y < endPos.y; y++)
                        {
                            // TileBase代表单个瓦片
                            TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));
                            if (tile != null)
                            {
                                TileProperty newTile = new TileProperty
                                {
                                    tileCoordinate = new Vector2Int(x, y),
                                    gridType = this.GridType,
                                    boolTypeValue = true,
                                    filedType = this.FiledType
                                };
                                MapData.tileProperties.Add(newTile);
                            }
                        }
                    }
                }
            }

        }
    }
}


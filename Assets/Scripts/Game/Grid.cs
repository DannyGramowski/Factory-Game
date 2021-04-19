using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public int width;
    public int height;
    public static Grid Instance { get; private set; }

    [SerializeField] Transform floor;
    [SerializeField] Transform gridParentPrefab;
    [SerializeField] Cell cellPrefab;
    
    private float cellWidth;
    private float cellHeight;
    private Cell[,] grid;

    private void Awake() {
        Utils.CheckSingletonValid<Grid>(this);
        Instance = this;
        cellWidth = (floor.localScale.x / width);
        cellHeight = (floor.localScale.z / height);

        grid = new Cell[width, height];
        for(int x  = 0; x < grid.GetLength(0); x++) {
            Transform parent = Instantiate(gridParentPrefab, Vector3.zero, Quaternion.identity, transform);
            parent.name = "grid parent " + x;
            for(int y = 0; y < grid.GetLength(1); y++) {
               Cell temp = Instantiate(cellPrefab, WorldPos(x, y), Quaternion.identity, parent);
                grid[x, y] = temp;
                temp.Startup(new Vector2Int(x,y), cellWidth, cellHeight);
            }
        }
    }

    private Vector3 WorldPos(int x, int y) {
        return new Vector3(x * cellWidth + cellWidth/2, floor.localScale.y / 2, y * cellHeight + cellHeight / 2);
    }
    public Vector2Int CellNum(Vector3 worldPos) {
        return new Vector2Int((int) (worldPos.x / cellWidth), (int) (worldPos.z / cellHeight));
    }

    public Cell GetCell(int x, int y) {
        return grid[x,y];
    }

    public Cell GetCell(Vector2Int nums) {
        return GetCell(nums.x, nums.y);
    }

    public bool Placable(Vector2Int pos, Building building) {
        Vector2Int dimensions = building.dimensions;
        if (pos.x + dimensions.x >= width || pos.y + dimensions.y >= height) return false;
        for(int x = pos.x; x < pos.x + dimensions.x; x++) {
            for (int y = pos.y; y < pos.y + dimensions.y; y++) {
                if (GetCell(x, y).building && GetCell(x,y).building != building) return false;
            }
        }
        return true;
    }

    public bool Placable(Cell cell, Building building) {
        return Placable(cell.pos, building);
    }

    public Vector2 GetCellScale() {
        return new Vector2(cellWidth, cellHeight);
    }

}
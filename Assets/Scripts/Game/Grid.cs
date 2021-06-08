using UnityEngine;
using Factory.Buildings;

namespace Factory.Core {

    public class Grid : Singleton<Grid> {
        public int width;
        public int height;

        [SerializeField] Transform floor;
        [SerializeField] Transform gridParentPrefab;
        [SerializeField] Cell cellPrefab;
        [SerializeField] float cellSize;

        private Cell[,] grid;

        private void Awake() {
            width = (int)(floor.localScale.x / cellSize);
            height = (int)(floor.localScale.z / cellSize);

            grid = new Cell[width, height];
            for (int x = 0; x < grid.GetLength(0); x++) {
                Transform parent = Instantiate(gridParentPrefab, Vector3.zero, Quaternion.identity, transform);
                parent.name = "grid parent " + x;
                for (int y = 0; y < grid.GetLength(1); y++) {
                    Cell temp = Instantiate(cellPrefab, WorldPos(x, y), Quaternion.identity, parent);
                    grid[x, y] = temp;
                    temp.Startup(new Vector2Int(x, y), cellSize);
                    temp.ShowDebug(GlobalPointers.showDebug);
                }
            }
        }

        private Vector3 WorldPos(int x, int y) {
            return new Vector3(x * cellSize + cellSize / 2, floor.localScale.y / 2, y * cellSize + cellSize / 2);
        }
        public Vector2Int CellNum(Vector3 worldPos) {
            return new Vector2Int((int)(worldPos.x / cellSize), (int)(worldPos.z / cellSize));
        }

        public Cell GetCell(int x, int y) {
            return grid[x, y];
        }

        public Cell GetCell(Vector2Int nums) {
            return GetCell(nums.x, nums.y);
        }

        public bool Placable(Vector2Int pos, Building building) {
            Vector2Int dimensions = building.dimensions;
            if (pos.x + dimensions.x >= width || pos.y + dimensions.y >= height) return false;
            for (int x = pos.x; x < pos.x + dimensions.x; x++) {
                for (int y = pos.y; y < pos.y + dimensions.y; y++) {
                    if (GetCell(x, y).building && GetCell(x, y).building != building) return false;
                }
            }
            return true;
        }

        public bool Placable(Cell cell, Building building) {
            return Placable(cell.pos, building);
        }

        public Vector2 GetCellScale() {
            return new Vector2(cellSize, cellSize);
        }

    }
}
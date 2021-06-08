﻿using System.Collections.Generic;
using UnityEngine;
using Factory.Core;
using Grid = Factory.Core.Grid;

namespace Factory.Buildings {
    [SelectionBase]
    public abstract class Building : MonoBehaviour {
        public Direction direction;
        public Cell baseCell;
        public Vector2Int dimensions;
        public int buildingType; //0-belt, 1-miner, 2-grabber, 3 storage

        public bool selected = false;

        [SerializeField] new string name;
        [SerializeField] List<Cell> placedCells;

        static int[] namingNums;

        protected virtual void Awake() {
            if (namingNums == null) {
                namingNums = new int[GlobalPointers.buildingPrefabs.Length];
            } 
             namingNums[buildingType]++;
            
            transform.name = name + namingNums[buildingType];
        }

        public Vector3 Rotate(Vector3 buildingRot) {
            dimensions = Utils.SwapVector2(dimensions);
            SetLocation();
            buildingRot += Utils.rotate90Y;
            transform.eulerAngles = buildingRot;
            return buildingRot;
        }

        public void SetHoverPosition(Cell cell) {
            this.baseCell = cell;
            SetLocation();
        }

        public void SetPostion() {
            SetLocation();
            SetPlacedCells();
        }

        public void ReduceNamingNum() {
            namingNums[buildingType]--;
        }

        public virtual void Deconstruct() {
            Destroy(gameObject);
        }

        public virtual void SetShowDebug(bool showDebug) { }

        private void SetLocation() {
            Vector3 addedPos = new Vector3((Grid.Instance.GetCellScale().x * (dimensions.x - 1)) / 2, 0, (Grid.Instance.GetCellScale().y * (dimensions.y - 1)) / 2);
            transform.position = baseCell.transform.position + addedPos;
        }

        private void SetPlacedCells() {
            print("set cells for " + name);
            for (int x = baseCell.pos.x; x < baseCell.pos.x + dimensions.x; x++) {
                for (int y = baseCell.pos.y; y < baseCell.pos.y + dimensions.y; y++) {
                    placedCells.Add(Grid.Instance.GetCell(x, y));
                }
            }

            foreach (Cell c in placedCells) {
                c.building = this;
            }
        }

        public virtual void OnHover(Cell hoverCell) {
                SetHoverPosition(hoverCell);
            
        }

        public virtual void Place() {
            direction = Utils.AngleToDirection(InputManager.Instance.buildingRot.y);
            SetPostion();
        }

        public virtual bool BuildingPlaced() { return true; }
    }
}
﻿using Factory.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Factory.Buildings {
    public class Belt : Building {
        public BeltSystem beltSystem;
        //public Grabber grabber;
        public Vector3 itemPos;
        public float speed;
        public int beltNum => beltSystem.BeltNum(this);
        Core.Grid grid;

        protected override void Awake() {
            grid = Core.Grid.Instance;
            base.Awake();
        }

        public override void Place(Direction placeDirection) {
            base.Place(placeDirection);
            itemPos = Utils.Vector3SetY(transform.position, GlobalPointers.itemHeight);
            CheckBeltSystem();
        }

         bool CheckBeltSystem() {
             bool addedBelt = false;
            //print("check belt system " + baseCell + " direction " + direction + ".");
                 Vector2Int forwardCell = Utils.Vector2FromDirection(direction) + baseCell.pos;
                 if (validCell(forwardCell)) {
                     Cell cell = grid.GetCell(forwardCell);
                     Belt belt = cell.building as Belt;
                     if (CheckBeltConnected(baseCell, cell) && belt.beltSystem.belts[0].Equals(belt)) { //to only add a belt infront if it is adding to the first belt
                         belt.beltSystem.AddBelt(this, true);
                         addedBelt = true;
                     }
                 }

                 addedBelt |= AddToBeltSystem(90);
                 addedBelt |= AddToBeltSystem(180);
                 addedBelt |= AddToBeltSystem(270);

                 if (!addedBelt) {
                     BeltSystem temp = Instantiate(GlobalPointers.beltSystemPrefab, GlobalPointers.buildingParent);
                     temp.AddBelt(this, false);
                     temp.SetShowDebug(GlobalPointers.showDebug);
                 }
             return true;
         }


        bool AddToBeltSystem(float angle) {
            Direction direc = Utils.AngleToDirection(transform.eulerAngles.y + angle);
            Vector2Int addedPos = Utils.Vector2FromDirection(direc);
            Vector2Int testCell = addedPos + baseCell.pos;

            if (validCell(testCell)) {
//                print("angle " + angle);
                if (CheckBeltConnected(grid.GetCell(testCell), baseCell)) {
                    BeltSystem otherBeltSystem = (grid.GetCell(testCell).building as Belt).beltSystem;
                    if (beltSystem && !(otherBeltSystem.Equals(beltSystem))) {
                        otherBeltSystem.CombineBeltSystems(beltSystem);
                    } else if (!(otherBeltSystem.Equals(beltSystem))) {
                    //    print("added to belt system");
                        otherBeltSystem.AddBelt(this, false);
                    }
                  //  print("did nothing");
                    return true;
                }

            }
            return false;
        }

        bool CheckBeltConnected(Cell to, Cell from) {
            Belt toBelt = to?.building as Belt;
            Belt fromBelt = from?.building as Belt;
          //  print($"to belt {toBelt}, from belt {fromBelt} for {name}");
            if (toBelt && fromBelt) {
                Vector2Int checkPos = (Utils.Vector2FromDirection(toBelt.direction) + to.pos);
                if (checkPos.Equals(from.pos)) {
            //        print("belt connected is true");
                    return true;
                }
            }
            return false;
        }

        /* protected override void OverrideLoad(object state) {
             //Start();
            // CheckBeltSystem();   
         }*/

        bool validCell(Vector2Int pos) {
          //  print("pos " + pos);
            //print("grid " + grid);
            return !(pos.x > grid.width - 1 || pos.x < 0 || pos.y > grid.height - 1 || pos.y < 0);
        }
    }
}
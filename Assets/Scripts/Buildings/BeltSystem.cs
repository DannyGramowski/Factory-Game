using System.Collections.Generic;
using UnityEngine;
using Factory.Core;
using System.Linq;

namespace Factory.Buildings {
    public class BeltSystem : MonoBehaviour, IOrderable {
        public List<Belt> belts = new List<Belt>();
        [SerializeField] int savePriority;//allows belts to be ordered
        bool showDebug = false;
        private static int namingNum = 1;

        private void Start() {
            transform.name = "Belt System " + namingNum;
            namingNum++;
            savePriority = GlobalPointers.buildingPrefabs.Where(s => s is Belt).First().GetSavePriority();
        }

        public void AddBelt(Belt b, bool front) {
            b.beltSystem = this;
            b.transform.parent = transform;
            if (front) {
                belts.Insert(0, b);
            } else {
                belts.Add(b);
            }
        }

        public void CombineBeltSystems(BeltSystem addedBelts) {
            foreach (Belt b in addedBelts.belts) {
                AddBelt(b, false);
            }
            addedBelts.belts.Clear();
            Destroy(addedBelts.gameObject);
        }

        public Belt NextBelt(Belt b) {
            if (b.beltNum == belts.Count - 1) {
                return GetBeltForward();
            }
            return belts[b.beltNum + 1];
        }

        //returns the belt infront of the last belt in the system
        public Belt GetBeltForward() {
            Belt b = belts[belts.Count - 1];
            return Core.Grid.Instance.GetCell(b.GetBaseCell().pos + Utils.Vector2FromDirection(b.direction))?.building as Belt;
        }

        public void SetShowDebug(bool visible) {
            showDebug = visible;
        }

        public int BeltNum(Belt b) {
            return belts.IndexOf(b);
        }

        private void OnDrawGizmos() {
            if (showDebug && belts.Count > 0) {
                for (int i = 0; i < belts.Count - 1; i++) {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(belts[i].transform.position, belts[i + 1].transform.position);
                }

                Gizmos.color = Color.green;
                Gizmos.DrawCube(belts[0].transform.position + new Vector3(0, 0.25f, 0), Vector3.one * 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawCube(belts[belts.Count - 1].transform.position + new Vector3(0, 0.25f, 0), Vector3.one * 0.5f);
            }
        }

        public int GetSavePriority() {
            return savePriority;
        }
    }
}
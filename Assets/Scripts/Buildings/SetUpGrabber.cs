using Factory.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Factory.Buildings {
    [ExecuteInEditMode]
    public class SetUpGrabber : MonoBehaviour {
        [SerializeField] ProductionBuilding setGrabbers;
        [SerializeField] ProducableBuildings producableBuildings;
        [SerializeField] IOType ioType;

        private enum GenerationType {
            border
        }
        [SerializeField] GenerationType generationType;

        public void GenerateGrabbers() {
            switch (generationType) {
                case GenerationType.border:
                    GenerateBorders();
                    break;
                default:
                    Debug.LogWarning($"{generationType} is not a valid generation");
                    break;
            }
        }

        public void GenerateBorders() {
            List<GrabberSpot> temp = new List<GrabberSpot>();

            for (int x = 0; x < setGrabbers.dimensions.x; x++) {
                for (int y = 0; y < setGrabbers.dimensions.y; y++) {
                    List<Direction> newDirecs = new List<Direction>();
                    if (x == 0) {
                        newDirecs.Add(Direction.left);
                    } else if (x == setGrabbers.dimensions.x - 1) {
                        newDirecs.Add(Direction.right);
                    }

                    if (y == 0) {
                        newDirecs.Add(Direction.down);
                    } else if (y == setGrabbers.dimensions.y - 1) {
                        newDirecs.Add(Direction.up);
                    }

                    if (newDirecs.Count == 0) {
                        continue;
                    }

                    temp.Add(new GrabberSpot(new Vector2Int(x, y), newDirecs, ioType));
                }
            };
            setGrabbers.SetGrabberSpots(temp);
            print($"generated {temp.Count} grabber spots");
        }
    }
}

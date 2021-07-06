using Factory.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Factory.Buildings {
    [ExecuteInEditMode]
    public class SetUpGrabber : MonoBehaviour {
        public ProductionBuilding setGrabbersInput;
        [SerializeField] ProducableBuildings producableBuildings;
        [SerializeField] IOType ioType;

        private enum GenerationType {
            border
        }
        [SerializeField] GenerationType generationType;

        public void GenerateGrabbers() {
            if (setGrabbersInput == null) {
                Debug.LogWarning("there is no building to set the grabbers of");
                return; 
            }

            ProductionBuilding setGrabbers = PrefabUtility.InstantiatePrefab(setGrabbersInput) as ProductionBuilding;
            switch (generationType) {
                case GenerationType.border:
                    GenerateBorders(setGrabbers);
                    break;
                default:
                    Debug.LogWarning($"{generationType} is not a valid generation");
                    break;
            }
            PrefabUtility.ApplyPrefabInstance(setGrabbers.gameObject, InteractionMode.AutomatedAction);
            DestroyImmediate(setGrabbers.gameObject);
        }

        public void GenerateBorders(ProductionBuilding setGrabbers) {
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

using System.Collections;
using UnityEngine;

namespace Factory.Saving {
    [System.Serializable]
    public class SVector3 {
        float x, y, z;

        public SVector3(Vector3 vector) {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector() {
            return new Vector3(x, y, z);
        }
    }
}
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

        public SVector3(float x1, float y1, float z1) {
            x = x1;
            y = y1;
            z = z1;
        }

        public Vector3 ToVector() {
            return new Vector3(x, y, z);
        }
    }
}
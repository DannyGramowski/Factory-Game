using System.Collections;
using UnityEngine;

namespace Factory.Saving {
    [System.Serializable]
    public class SVector2 {
        float x, y;

        public SVector2(Vector2 vector) {
            x = vector.x;
            y = vector.y;
        }

        public SVector2(Vector2Int vector) {
            x = vector.x;
            y = vector.y;
        }

        public SVector2(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public Vector2 ToVector() {
            return new Vector2(x, y);
        }

        public Vector2Int ToVectorInt() {
            return new Vector2Int((int)x, (int)y);
        }

    }
}
using Factory.Saving;
using UnityEngine;
namespace Factory.Core {

    public class Timer : MonoBehaviour {
        private float currTime;
        private float maxTime;
        private bool looping;

        private void Update() {
            if (!TimerDone()) {
                currTime -= Time.deltaTime;
            } else if (looping) {
                StartTimer();
            }
        }

        public void StartUp(float maxTime, bool looping) {
            this.maxTime = maxTime;
            this.looping = looping;
        }

        public void StartUp(float maxTime) {
            StartUp(maxTime, false);
        }

        public void StartTimer() {
            currTime = maxTime;
        }

        public void StartTimer(float newTime) {
            maxTime = newTime;
            StartTimer();
        }

        public float GetTime() {
            return currTime;
        }

        public bool TimerDone() {
            return currTime <= 0;
        }

        public object Save() {
            return new SVector3(currTime, maxTime, looping ? 1 : 0);
        }

        public void Load(object obj) {
            Vector3 vector = ((SVector3)obj).ToVector();
            currTime = vector.x;
            maxTime = vector.y;
            looping = vector.z == 1;
        }
    }
}
using UnityEngine;

namespace Scenes {
    public sealed class TimerController : MonoBehaviour {
        public float ElapsedTime;
        public bool IsRunning;

        void Update() {
            if (!IsRunning) return;
            ElapsedTime += Time.deltaTime;
        }

        public void ResetTimer() {
            ElapsedTime = 0;
        }

        public void StartTimer() {
            IsRunning = true;
        }

        public void StopTimer() {
            IsRunning = false;
        }
    }
}

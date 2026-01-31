using UnityEngine;

namespace UI {
    public sealed class CursorController : MonoBehaviour {
        void Awake() {
            LockCursor();
        }

        public void LockCursor() {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void UnlockCursor() {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

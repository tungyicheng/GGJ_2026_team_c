using UnityEngine;

namespace UI {
    public sealed class CursorController : MonoBehaviour {
        void Awake() {
            LockCursor();
        }

        void OnDisable() {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void LockCursor() {
            if (!isActiveAndEnabled) return;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void UnlockCursor() {
            if (!isActiveAndEnabled) return;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

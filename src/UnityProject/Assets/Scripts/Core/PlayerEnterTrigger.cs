using System;
using UnityEngine;

namespace Scenes {
    public sealed class PlayerEnterTrigger : MonoBehaviour {
        public event Action OnPlayerEnter;

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                OnPlayerEnter?.Invoke();
            }
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes {
    public sealed class Bootstrapper : MonoBehaviour {
        public int BuildSceneIndex;

        void Start() {
            SceneManager.LoadScene(BuildSceneIndex);
        }
    }
}

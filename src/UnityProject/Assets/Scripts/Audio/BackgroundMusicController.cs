using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio {
    public sealed class BackgroundMusicController : MonoBehaviour {
        public static BackgroundMusicController Instance;
        public AudioSource BassLayerMusic;
        public AudioSource DrumsLayerMusic;
        public AudioSource GuitarLayerMusic;
        public float fadeTime = 1;
        public int TitleSceneBuildIndex;
        public int GameSceneBuildIndex;

        void Awake() {
            if (Instance && Instance != this) {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        void OnEnable() {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.buildIndex == TitleSceneBuildIndex) ApplyTitleSceneMusicLayers();
            if (activeScene.buildIndex == GameSceneBuildIndex) ApplyGameSceneMusicLayers();
            PlayMusic();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable() {
            StopMusic();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.buildIndex == GameSceneBuildIndex)
                ApplyGameSceneMusicLayers();
        }

        void PlayMusic() {
            BassLayerMusic.PlayScheduled(AudioSettings.dspTime + 0.2f);
            DrumsLayerMusic.PlayScheduled(AudioSettings.dspTime + 0.2f);
            GuitarLayerMusic.PlayScheduled(AudioSettings.dspTime + 0.2f);
        }

        void StopMusic() {
            BassLayerMusic.Stop();
            DrumsLayerMusic.Stop();
            GuitarLayerMusic.Stop();
        }

        public void ApplyTitleSceneMusicLayers() {
            StopAllCoroutines();
            StartCoroutine(FadeChannel(BassLayerMusic, 1f));
            StartCoroutine(FadeChannel(DrumsLayerMusic, 0f));
            StartCoroutine(FadeChannel(GuitarLayerMusic, 0f));
        }

        public void ApplyGameSceneMusicLayers() {
            StopAllCoroutines();
            StartCoroutine(FadeChannel(BassLayerMusic, 1f));
            StartCoroutine(FadeChannel(DrumsLayerMusic, 1f));
            StartCoroutine(FadeChannel(GuitarLayerMusic, 1f));
        }

        IEnumerator FadeChannel(AudioSource channel, float targetVolume) {
            float time = 0;
            float start = channel.volume;

            while (time < fadeTime) {
                time += Time.deltaTime;
                channel.volume = Mathf.Lerp(start, targetVolume, time / fadeTime);
                yield return null;
            }

            channel.volume = targetVolume;
            yield break;
        }
    }
}

using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI {
    [RequireComponent(typeof(UIDocument))]
    [DisallowMultipleComponent]
    public sealed class TitleScreen : MonoBehaviour {
        public int StartLoadBuildSceneIndex;
        public int FadeDuration = 100;
        public AudioController AudioController;
        public AudioSource ClickSound;

        VisualElement rootVisualElement;
        Button startButton;
        Button optionsButton;
        Button creditsButton;
        VisualElement quitButtonContainer;
        Button quitButton;
        VisualElement optionsPanel;
        SliderInt soundEffectsVolumeSlider;
        SliderInt musicVolumeSlider;
        Button optionsPanelBackButton;
        VisualElement creditsPanel;
        Button creditsPanelBackButton;

        void OnEnable() {
            if (!TryGetComponent(out UIDocument document)) return;
            rootVisualElement = document.rootVisualElement;
            startButton = rootVisualElement.Q<Button>("start-button");
            optionsButton = rootVisualElement.Q<Button>("options-button");
            creditsButton = rootVisualElement.Q<Button>("credits-button");
            quitButtonContainer = rootVisualElement.Q<VisualElement>("quit-button-container");
            quitButton = rootVisualElement.Q<Button>("quit-button");
            optionsPanel = rootVisualElement.Q<VisualElement>("options-panel");
            optionsPanelBackButton = optionsPanel.Q<Button>("options-panel-back-button");
            soundEffectsVolumeSlider = optionsPanel.Q<SliderInt>("sound-effects-volume-slider");
            musicVolumeSlider = optionsPanel.Q<SliderInt>("music-volume-slider");
            creditsPanel = rootVisualElement.Q<VisualElement>("credits-panel");
            creditsPanelBackButton = creditsPanel.Q<Button>("credits-panel-back-button");
            startButton.clicked += OnStartButtonClicked;
            optionsButton.clicked += OnOptionsButtonClicked;
            creditsButton.clicked += OnCreditsButtonClicked;
#if UNITY_WEBGL
            quitButtonContainer.style.display = DisplayStyle.None;
#endif
            quitButton.clicked += OnQuitButtonClicked;
            soundEffectsVolumeSlider.value = AudioController.SoundEffectsVolume;
            soundEffectsVolumeSlider.RegisterValueChangedCallback(OnSoundEffectVolumeChanged);
            musicVolumeSlider.value = AudioController.MusicVolume;
            musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
            optionsPanelBackButton.clicked += OnOptionsPanelBackButtonClicked;
            creditsPanelBackButton.clicked += OnCreditsPanelBackButtonClicked;
        }

        void OnDisable() {
            startButton.clicked -= OnStartButtonClicked;
            optionsButton.clicked -= OnOptionsButtonClicked;
            creditsButton.clicked -= OnCreditsButtonClicked;
            quitButton.clicked -= OnQuitButtonClicked;
            soundEffectsVolumeSlider.UnregisterValueChangedCallback(OnSoundEffectVolumeChanged);
            musicVolumeSlider.UnregisterValueChangedCallback(OnMusicVolumeChanged);
            optionsPanelBackButton.clicked -= OnOptionsPanelBackButtonClicked;
            creditsPanelBackButton.clicked -= OnCreditsPanelBackButtonClicked;
        }

        void OnStartButtonClicked() {
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            rootVisualElement.pickingMode = PickingMode.Ignore;
            rootVisualElement.experimental.animation.Start(1f, 0f, FadeDuration,
                    static (element, value) => element.style.opacity = value)
                .OnCompleted(() => SceneManager.LoadSceneAsync(StartLoadBuildSceneIndex));
        }

        void OnOptionsButtonClicked() {
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            optionsPanel.style.display = DisplayStyle.Flex;
            optionsPanel.experimental.animation.Start(0f, 1f, FadeDuration,
                static (element, value) => element.style.opacity = value);
        }

        void OnCreditsButtonClicked() {
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            BackgroundMusicController.Instance.ApplyGameSceneMusicLayers();
            creditsPanel.style.display = DisplayStyle.Flex;
            creditsPanel.experimental.animation.Start(0f, 1f, FadeDuration,
                static (element, value) => element.style.opacity = value);
        }

        void OnSoundEffectVolumeChanged(ChangeEvent<int> evt) {
            if (evt.newValue == AudioController.SoundEffectsVolume) return;
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            AudioController.SoundEffectsVolume = evt.newValue;
        }

        void OnMusicVolumeChanged(ChangeEvent<int> evt) {
            if (evt.newValue == AudioController.MusicVolume) return;
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            AudioController.MusicVolume = evt.newValue;
        }

        void OnOptionsPanelBackButtonClicked() {
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            optionsPanel.experimental.animation.Start(1f, 0f, FadeDuration,
                    static (element, value) => element.style.opacity = value)
                .OnCompleted(() => optionsPanel.style.display = DisplayStyle.None);
        }

        void OnCreditsPanelBackButtonClicked() {
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            BackgroundMusicController.Instance.ApplyTitleSceneMusicLayers();
            creditsPanel.experimental.animation.Start(1f, 0f, FadeDuration,
                    static (element, value) => element.style.opacity = value)
                .OnCompleted(() => creditsPanel.style.display = DisplayStyle.None);
        }

        void OnQuitButtonClicked() {
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            rootVisualElement.pickingMode = PickingMode.Ignore;
            rootVisualElement.experimental.animation.Start(1f, 0f, FadeDuration,
                    static (element, value) => element.style.opacity = value)
                .OnCompleted(() => {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                });
        }
    }
}

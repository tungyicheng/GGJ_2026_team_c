using System;
using Scenes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI {
    [RequireComponent(typeof(UIDocument))]
    [DisallowMultipleComponent]
    public sealed class GameScreen : MonoBehaviour {
        public int MainMenuLoadBuildSceneIndex;
        public int FadeDuration = 100;
        public InputActionReference PauseActionReference;
        public AudioSource ClickSound;
        public CursorController CursorController;
        public PlayerInput PlayerInput;
        public TimerController timerController;
        public PlayerEnterTrigger gameClearTrigger;

        VisualElement rootVisualElement;
        VisualElement pausePanel;
        Label pauseTime;
        Button resumeButton;
        Button mainMenuButton;
        VisualElement mainMenuPanel;
        VisualElement gameOverPanel;
        Label clearTime;
        Button clearTimeMainMenuButton;

        void OnEnable() {
            PauseActionReference.action.Enable();
            PauseActionReference.action.performed += OnPauseActionPerformed;
            gameClearTrigger.OnPlayerEnter += OnPlayerEnterGameClearTrigger;
            if (!TryGetComponent(out UIDocument document)) return;
            rootVisualElement = document.rootVisualElement;
            pausePanel = rootVisualElement.Q<VisualElement>("pause-panel");
            pauseTime = rootVisualElement.Q<Label>("pause-time");
            resumeButton = pausePanel.Q<Button>("resume-button");
            mainMenuButton = pausePanel.Q<Button>("main-menu-button");
            mainMenuPanel = rootVisualElement.Q<VisualElement>("main-menu-panel");
            gameOverPanel = rootVisualElement.Q<VisualElement>("game-over-panel");
            clearTime = rootVisualElement.Q<Label>("clear-time");
            clearTimeMainMenuButton = rootVisualElement.Q<Button>("clear-time-main-menu-button");
            resumeButton.clicked += OnResumeButtonClicked;
            mainMenuButton.clicked += OnMainMenuButtonClicked;
            clearTimeMainMenuButton.clicked += OnClearTimeMainMenuButtonClicked;
        }

        void OnDisable() {
            PauseActionReference.action.Disable();
            PauseActionReference.action.performed -= OnPauseActionPerformed;
            gameClearTrigger.OnPlayerEnter -= OnPlayerEnterGameClearTrigger;
            resumeButton.clicked -= OnResumeButtonClicked;
            mainMenuButton.clicked -= OnMainMenuButtonClicked;
            clearTimeMainMenuButton.clicked -= OnClearTimeMainMenuButtonClicked;
        }

        void Update() {
            var time = TimeSpan.FromSeconds(timerController.ElapsedTime);
            var timeText = $@"{time:mm\:ss\:ff}";
            pauseTime.text = timeText;
            clearTime.text = timeText;
        }

        void Pause() {
            Time.timeScale = 0f;
            CursorController.UnlockCursor();
            PlayerInput.enabled = false;
            pausePanel.style.display = DisplayStyle.Flex;
            pausePanel.experimental.animation.Start(0f, 1f, FadeDuration,
                static (element, value) => element.style.opacity = value);
        }

        void Resume() {
            Time.timeScale = 1f;
            CursorController.LockCursor();
            PlayerInput.enabled = true;
            pausePanel.experimental.animation.Start(1f, 0f, FadeDuration,
                    static (element, value) => element.style.opacity = value)
                .OnCompleted(() => pausePanel.style.display = DisplayStyle.None);
        }

        public void GameOver() {
            Time.timeScale = 0f;
            CursorController.UnlockCursor();
            pausePanel.style.display = DisplayStyle.None;
            gameOverPanel.style.display = DisplayStyle.Flex;
            gameOverPanel.experimental.animation.Start(0f, 1f, FadeDuration,
                static (element, value) => element.style.opacity = value);
        }

        void OnPauseActionPerformed(InputAction.CallbackContext context) {
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            if (Time.timeScale > 0f) Pause();
            else Resume();
        }

        void OnResumeButtonClicked() {
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            Resume();
        }

        void OnMainMenuButtonClicked() {
            if (!ClickSound.isPlaying)
                ClickSound.Play();
            mainMenuPanel.style.display = DisplayStyle.Flex;
            mainMenuPanel.experimental.animation.Start(0f, 1f, FadeDuration,
                    static (element, value) => element.style.opacity = value)
                .OnCompleted(() => {
                    SceneManager.LoadSceneAsync(MainMenuLoadBuildSceneIndex);
                    Time.timeScale = 1f;
                });
        }

        void OnClearTimeMainMenuButtonClicked() {
            OnMainMenuButtonClicked();
        }

        void OnPlayerEnterGameClearTrigger() {
            GameOver();
        }
    }
}

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

        VisualElement rootVisualElement;
        VisualElement pausePanel;
        Button resumeButton;
        Button mainMenuButton;
        VisualElement mainMenuPanel;

        void OnEnable() {
            PauseActionReference.action.Enable();
            PauseActionReference.action.performed += OnPauseActionPerformed;
            if (!TryGetComponent(out UIDocument document)) return;
            rootVisualElement = document.rootVisualElement;
            pausePanel = rootVisualElement.Q<VisualElement>("pause-panel");
            resumeButton = pausePanel.Q<Button>("resume-button");
            mainMenuButton = pausePanel.Q<Button>("main-menu-button");
            mainMenuPanel = rootVisualElement.Q<VisualElement>("main-menu-panel");
            resumeButton.clicked += OnResumeButtonClicked;
            mainMenuButton.clicked += OnMainMenuButtonClicked;
        }

        void OnDisable() {
            PauseActionReference.action.Disable();
            PauseActionReference.action.performed -= OnPauseActionPerformed;
            resumeButton.clicked -= OnResumeButtonClicked;
            mainMenuButton.clicked -= OnMainMenuButtonClicked;
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
            Time.timeScale = 1f;
            mainMenuPanel.style.display = DisplayStyle.Flex;
            mainMenuPanel.experimental.animation.Start(0f, 1f, FadeDuration,
                    static (element, value) => element.style.opacity = value)
                .OnCompleted(() => SceneManager.LoadSceneAsync(MainMenuLoadBuildSceneIndex));
        }
    }
}

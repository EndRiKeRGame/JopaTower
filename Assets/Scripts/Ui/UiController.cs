using System;
using Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class UiController : MonoBehaviour
    {
        public event Action OnStartButtonPressed;
        public event Action OnRestartButtonPressed;
        public event Action OnPauseButtonPressed;
        public event Action OnContinueButtonPressed;
        public event Action OnMainMenuButtonPressed;
        
        // main view
        [SerializeField] private SpaceView _spaceView;
        
        // other views
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private DeathScreenView _deathScreenView;
        [SerializeField] private FinaleScreenView _finaleScreenView;
        [SerializeField] private PauseMenuView _pauseMenuView;
        [SerializeField] private TitlesScreenView _titlesScreenView;
        
        // special views
        [SerializeField] private DialogueView _dialogueView;
        
        // buttons
        [SerializeField] private Button _soundButton;
        [SerializeField] private Button _pauseButton;
        
        // sound
        [SerializeField] private Image _soundImg;
        [SerializeField] private SoundSpritesConfig _soundSpritesConfig;
        [SerializeField] private AudioSource _audioSource;
        
        private bool _isSoundMuted = false;

        private void Awake()
        {
            _mainMenuView.Init(OnStartButtonClicked, OnExitButtonClicked);
            _deathScreenView.Init(OnRestartButtonClicked, OnExitButtonClicked);
            _finaleScreenView.Init(OnFinaleViewButtonClicked);
            _pauseMenuView.Init(OnContinueButtonClicked, OnToMainMenuButtonClicked);
            _titlesScreenView.Init(OnToMainMenuButtonClicked);
            
            _pauseButton.onClick.AddListener(OnPauseButtonClicked);
            _soundButton.onClick.AddListener(OnSoundButtonClicked);

            HideAllInstantly();
            ShowMainMenuScreen();
        }

        public void ShowDialogue(FullDialogueAsset asset, Action onComplete = null)
        {
            _dialogueView.ShowDialogue(asset, onComplete);
        }

        public void ShowDeathScreen()
        {
            if (!_spaceView.IsShown)
                _spaceView.Show();

            _deathScreenView.Show();
        }
        
        public void ShowFinaleScreen()
        {
            if (!_spaceView.IsShown)
                _spaceView.ShowInstantly();

            _finaleScreenView.Show();
        }
        
        public void ShowMainMenuScreen()
        {
            if (!_spaceView.IsShown)
                _spaceView.ShowInstantly();
            
            Debug.Log("MainMenuScreen");
            _mainMenuView.Show();
        }

        public void ShowPauseMenuScreen()
        {
            if (!_spaceView.IsShown)
                _spaceView.Show();

            _pauseMenuView.Show();
        }

        private void OnStartButtonClicked()
        {
            HideAll();
            OnStartButtonPressed?.Invoke();
        }

        private void OnExitButtonClicked()
        {
            Application.Quit();
        }

        private void OnRestartButtonClicked()
        {
            HideAll();
            OnRestartButtonPressed?.Invoke();
        }

        private void OnFinaleViewButtonClicked()
        {
            _finaleScreenView.HideInstantly();
            _titlesScreenView.Show();
        }

        private void OnToMainMenuButtonClicked()
        {
            HideAllExceptSpace();
            ShowMainMenuScreen();
            OnMainMenuButtonPressed?.Invoke();
        }

        private void OnPauseButtonClicked()
        {
            HideAll();
            ShowPauseMenuScreen();
            OnPauseButtonPressed?.Invoke();
        }

        private void OnContinueButtonClicked()
        {
            HideAll();
            OnContinueButtonPressed?.Invoke();
        }

        private void OnSoundButtonClicked()
        {
            _isSoundMuted = !_isSoundMuted;
            _soundImg.sprite = _isSoundMuted ? _soundSpritesConfig.SoundOff : _soundSpritesConfig.SoundOn;
            _audioSource.mute = _isSoundMuted;
        }

        private void HideAll()
        {
            _spaceView.Hide();
            
            _mainMenuView.Hide();
            _deathScreenView.Hide();
            _finaleScreenView.Hide();
            _pauseMenuView.Hide();
            _titlesScreenView.Hide();
        }
        
        private void HideAllInstantly()
        {
            _spaceView.HideInstantly();
            
            _mainMenuView.HideInstantly();
            _deathScreenView.HideInstantly();
            _finaleScreenView.HideInstantly();
            _pauseMenuView.HideInstantly();
            _titlesScreenView.HideInstantly();
        }
        
        private void HideAllExceptSpace()
        {
            _mainMenuView.Hide();
            _deathScreenView.Hide();
            _finaleScreenView.Hide();
            _pauseMenuView.Hide();
            _titlesScreenView.Hide();
        }
        
        private void OnDestroy()
        {
            _pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        }
    }
}
using System;
using Player;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Ui
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;
        
        [SerializeField]
        private Button _restartButton;
        
        [SerializeField]
        private CanvasGroup _canvasGroupMainMenu;
        
        [SerializeField]
        private CanvasGroup _canvasGroupDeathScreen;
        
        [SerializeField]
        private CanvasGroup _canvasGroupSpace;
        
        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private Image _image;
        
        [SerializeField]
        private CanvasGroup _finaleCG;
        
        [SerializeField]
        private CanvasGroup _finaleScene1CG;
        
        [SerializeField]
        private CanvasGroup _finaleScene2CG;
        
        [SerializeField]
        private Button _toTitleScreen;
        
        [SerializeField]
        private Button _toMainMenu;
        
        [SerializeField]
        private Button _toExit;
        
        [Header("Animation values")]
        [SerializeField]
        private float _animationDuration = 0.2f;
        
        [SerializeField]
        private float _colorChangeAnimationDuration = 10f;
        
        public event Action OnStartButtonPressed;
        public event Action<bool> OnRestartButtonPressed;

        private Scrollbar _scrollBar;

        public void Init(HealthSystem healthSystem)
        {
            ShowSpace();
            ShowMainMenu();
            
            _scrollBar = _scrollRect.horizontalScrollbar;
            Tween.Custom(0f, 1f, new TweenSettings(600f, Ease.Default, cycleMode: CycleMode.Rewind, cycles: -1), f => _scrollBar.value = f);
            
            Sequence seq = Sequence.Create();
            
            var firstState = Tween.Color(_image, new TweenSettings<Color>(new Color(1f, 1f, 0f, 0.8f), _colorChangeAnimationDuration));
            var secondState = Tween.Color(_image, new TweenSettings<Color>(new Color(1f, 0f, 1f, 0.8f), _colorChangeAnimationDuration));
            var thirdState = Tween.Color(_image, new TweenSettings<Color>(new Color(0f, 1f, 1f, 0.8f), _colorChangeAnimationDuration));
            
            seq.Chain(firstState);
            seq.Chain(secondState);
            seq.Chain(thirdState);
            seq.SetRemainingCycles(-1);

            _startButton.onClick.AddListener(StartGame);
            _restartButton.onClick.AddListener(RestartGame);
            _toExit.onClick.AddListener(() => Application.Quit());
            
            healthSystem.OnDeath += ShowDeathScreen;
            
            _toTitleScreen.onClick.AddListener(ShowTitles);
            _toMainMenu.onClick.AddListener(HideFinale);
            
            _toTitleScreen.interactable = false;
            _toMainMenu.interactable = false;
        }

        private void StartGame()
        {
            HideSpace();
            OnStartButtonPressed?.Invoke();
        }
        
        private void RestartGame()
        {
            HideSpace();
            OnRestartButtonPressed?.Invoke(true);
        }
        
        private void StopGame()
        {
            ShowMainMenu();
            OnRestartButtonPressed?.Invoke(false);
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveAllListeners();
        }
        
        public void ShowMainMenu()
        {
            ShowSpace();
            HideDeathScreen();
            
            _canvasGroupMainMenu.blocksRaycasts = true;
            _canvasGroupMainMenu.alpha = 1f;
        }
        
        public void HideMainMenu()
        {
            _canvasGroupMainMenu.blocksRaycasts = false;
            _canvasGroupMainMenu.alpha = 0f;
        }
        
        public void ShowDeathScreen()
        {
            ShowSpace();
            HideMainMenu();
            
            _canvasGroupDeathScreen.blocksRaycasts = true;
            _canvasGroupDeathScreen.alpha = 1f;
        }
        
        public void HideDeathScreen()
        {
            _canvasGroupDeathScreen.blocksRaycasts = false;
            _canvasGroupDeathScreen.alpha = 0f;
        }
        
        public void ShowSpace()
        {
            _canvasGroupSpace.blocksRaycasts = true;
            Tween.Alpha(_canvasGroupSpace, 1f, 0.2f);
        }
        
        public void ShowSpaceInsta()
        {
            _canvasGroupSpace.blocksRaycasts = true;
            _canvasGroupSpace.alpha = 1f;
        }
        
        public void HideSpace()
        {
            _canvasGroupSpace.blocksRaycasts = false;
            Tween.Alpha(_canvasGroupSpace, 0f, 0.2f);
        }

        public void ShowFinale()
        {
            ShowSpaceInsta();
            HideMainMenu();
            HideDeathScreen();
            _finaleCG.alpha = 1f;
            _finaleCG.blocksRaycasts = true;
            _finaleScene1CG.blocksRaycasts = true;
            
            var seq = Sequence.Create();
            seq.Insert(0f, Tween.Alpha(_finaleScene1CG, 1f, 0.4f));
            _toTitleScreen.interactable = true;
        }
        
        private void ShowTitles()
        {
            _finaleScene1CG.blocksRaycasts = false;
            _finaleScene2CG.blocksRaycasts = true;
            var seq = Sequence.Create();
            seq.Insert(0f, Tween.Alpha(_finaleScene1CG, 0f, 0.4f));
            seq.Insert(0.4f, Tween.Alpha(_finaleScene2CG, 1f, 0.4f));
            seq.ChainCallback(() => _toMainMenu.interactable = true);
        }

        private void HideFinale()
        {
            _finaleCG.blocksRaycasts = false;
            _finaleScene2CG.blocksRaycasts = false;
            HideMainMenu();
            
            var seq = Sequence.Create();
            seq.Insert(0f, Tween.Alpha(_finaleCG, 0f, 0.4f));
            seq.Insert(0.4f, Tween.Alpha(_canvasGroupMainMenu, 1f, 0.4f));
            
            _canvasGroupMainMenu.blocksRaycasts = true;
            OnRestartButtonPressed?.Invoke(false);
            
            _toTitleScreen.interactable = false;
            _toMainMenu.interactable = false;
        }
    }
}
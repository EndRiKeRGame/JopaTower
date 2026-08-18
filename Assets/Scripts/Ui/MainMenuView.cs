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
        
        [Header("Animation values")]
        [SerializeField]
        private float _animationDuration = 0.2f;
        
        [SerializeField]
        private float _colorChangeAnimationDuration = 10f;
        
        public event Action OnStartButtonPressed;
        public event Action OnRestartButtonPressed;

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
            
            healthSystem.OnDeath += ShowDeathScreen;
        }

        private void StartGame()
        {
            HideSpace();
            OnStartButtonPressed?.Invoke();
        }
        
        private void RestartGame()
        {
            HideSpace();
            OnRestartButtonPressed?.Invoke();
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveAllListeners();
        }
        
        public void ShowMainMenu()
        {
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
        
        public void HideSpace()
        {
            _canvasGroupSpace.blocksRaycasts = false;
            Tween.Alpha(_canvasGroupSpace, 0f, 0.2f);
        }
    }
}
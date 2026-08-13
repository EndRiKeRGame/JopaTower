using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Ui
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField]
        private Boot _boot;
        
        [SerializeField]
        private Button _startButton;
        
        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private Image _image;
        
        [Header("Animation values")]
        [SerializeField]
        private float _animationDuration = 0.2f;
        
        [SerializeField]
        private float _colorChangeAnimationDuration = 10f;
        
        [SerializeField]
        private AnimationCurve _animCurve;

        private Scrollbar _scrollBar;

        private void Awake()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1;
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
        }

        private void StartGame()
        {
            _canvasGroup.blocksRaycasts = false;
            _boot.StartGame();
            Tween.Alpha(_canvasGroup, 0, 0.2f);
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveAllListeners();
        }
    }
}
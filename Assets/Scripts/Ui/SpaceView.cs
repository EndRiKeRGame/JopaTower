using Interfaces;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class SpaceView : MonoBehaviour, IShowable
    {
        public bool IsShown => _spaceCG.blocksRaycasts;
        
        [SerializeField]
        private CanvasGroup _spaceCG;
        
        [SerializeField]
        private Image _spaceImage;
        
        [SerializeField]
        private ScrollRect _scrollRect;
        
        [SerializeField]
        private float _colorChangeAnimationDuration = 10f;
        
        private Scrollbar _scrollBar;

        private void Awake()
        {
            _scrollBar = _scrollRect.horizontalScrollbar;
            Tween.Custom(0f, 1f, new TweenSettings(600f, Ease.Default, cycleMode: CycleMode.Rewind, cycles: -1), f => _scrollBar.value = f);
            
            Sequence seq = Sequence.Create();
            
            var firstState = Tween.Color(_spaceImage, new TweenSettings<Color>(new Color(1f, 1f, 0f, 0.8f), _colorChangeAnimationDuration));
            var secondState = Tween.Color(_spaceImage, new TweenSettings<Color>(new Color(1f, 0f, 1f, 0.8f), _colorChangeAnimationDuration));
            var thirdState = Tween.Color(_spaceImage, new TweenSettings<Color>(new Color(0f, 1f, 1f, 0.8f), _colorChangeAnimationDuration));
            
            seq.Chain(firstState);
            seq.Chain(secondState);
            seq.Chain(thirdState);
            seq.SetRemainingCycles(-1);
        }

        public void Show()
        {
            Tween.Alpha(_spaceCG, 1f, 0.2f);
            _spaceCG.interactable = true;
            _spaceCG.blocksRaycasts = true;
        }
        
        public void ShowInstantly()
        {
            _spaceCG.alpha = 1;
            _spaceCG.interactable = true;
            _spaceCG.blocksRaycasts = true;
        }

        public void Hide()
        {
            Tween.Alpha(_spaceCG, 0f, 0.2f);
            _spaceCG.interactable = false;
            _spaceCG.blocksRaycasts = false;
        }

        public void HideInstantly()
        {
            _spaceCG.alpha = 0;
            _spaceCG.interactable = false;
            _spaceCG.blocksRaycasts = false;
        }
    }
}
using Interfaces;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class MainMenuView : MonoBehaviour, IShowable
    {
        [SerializeField]
        private CanvasGroup _mainCG;
        
        [SerializeField]
        private Button _startButton;
        
        [SerializeField]
        private Button _exitButton;

        private UnityAction _onStartButtonClicked;
        private UnityAction _onExitButtonClicked;

        public void Init(UnityAction onStartClicked, UnityAction onExitClicked)
        {
            _onStartButtonClicked = onStartClicked;
            _onExitButtonClicked = onExitClicked;
            
            _startButton.onClick.AddListener(_onStartButtonClicked);
            _exitButton.onClick.AddListener(_onExitButtonClicked);
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveListener(_onStartButtonClicked);
            _exitButton.onClick.RemoveListener(_onExitButtonClicked);
        }

        public void Show()
        {
            Tween.Alpha(_mainCG, 1f, 0.2f);
            _mainCG.interactable = true;
            _mainCG.blocksRaycasts = true;
        }

        public void Hide()
        {
            Tween.Alpha(_mainCG, 0f, 0.2f);
            _mainCG.interactable = false;
            _mainCG.blocksRaycasts = false;
        }

        public void HideInstantly()
        {
            _mainCG.alpha = 0;
            _mainCG.interactable = false;
            _mainCG.blocksRaycasts = false;
        }
    }
}
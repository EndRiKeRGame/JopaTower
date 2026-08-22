using Interfaces;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class PauseMenuView : MonoBehaviour, IShowable
    {
        [SerializeField]
        private CanvasGroup _pauseCG;
        
        [SerializeField]
        private Button _continueButton;
        
        [SerializeField]
        private Button _mainMenuButton;

        private UnityAction _onContinueButtonClicked;
        private UnityAction _onToMainMenuButtonClicked;

        public void Init(UnityAction continueBtn, UnityAction toMainMenu)
        {
            _onContinueButtonClicked = continueBtn;
            _onToMainMenuButtonClicked = toMainMenu;
            
            _continueButton.onClick.AddListener(_onContinueButtonClicked);
            _mainMenuButton.onClick.AddListener(_onToMainMenuButtonClicked);
        }

        public void Show()
        {
            Tween.Alpha(_pauseCG, 1f, 0.2f);
            _pauseCG.interactable = true;
            _pauseCG.blocksRaycasts = true;
        }

        public void Hide()
        {
            Tween.Alpha(_pauseCG, 0f, 0.2f);
            _pauseCG.interactable = false;
            _pauseCG.blocksRaycasts = false;
        }

        public void HideInstantly()
        {
            _pauseCG.alpha = 0;
            _pauseCG.interactable = false;
            _pauseCG.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            _continueButton.onClick.RemoveListener(_onContinueButtonClicked);
            _mainMenuButton.onClick.RemoveListener(_onToMainMenuButtonClicked);
        }
    }
}
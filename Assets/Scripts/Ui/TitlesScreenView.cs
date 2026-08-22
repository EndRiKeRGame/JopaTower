using Interfaces;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class TitlesScreenView : MonoBehaviour, IShowable
    {
        [SerializeField]
        private CanvasGroup _titlesCG;
        
        [SerializeField]
        private Button _continueButton;

        private UnityAction _onContinueButtonClicked;

        public void Init(UnityAction onExitClicked)
        {
            _onContinueButtonClicked = onExitClicked;
            
            _continueButton.onClick.AddListener(_onContinueButtonClicked);
        }

        private void OnDestroy()
        {
            _continueButton.onClick.RemoveListener(_onContinueButtonClicked);
        }

        public void Show()
        {
            Tween.Alpha(_titlesCG, 1f, 0.2f);
            _titlesCG.interactable = true;
            _titlesCG.blocksRaycasts = true;
        }

        public void Hide()
        {
            Tween.Alpha(_titlesCG, 0f, 0.2f);
            _titlesCG.interactable = false;
            _titlesCG.blocksRaycasts = false;
        }

        public void HideInstantly()
        {
            _titlesCG.alpha = 0;
            _titlesCG.interactable = false;
            _titlesCG.blocksRaycasts = false;
        }
    }
}
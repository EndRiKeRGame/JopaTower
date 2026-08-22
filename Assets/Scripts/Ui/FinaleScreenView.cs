using Interfaces;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class FinaleScreenView : MonoBehaviour, IShowable
    {
        [SerializeField]
        private CanvasGroup _finaleCG;
        
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
            Tween.Alpha(_finaleCG, 1f, 0.2f);
            _finaleCG.interactable = true;
            _finaleCG.blocksRaycasts = true;
        }

        public void Hide()
        {
            Tween.Alpha(_finaleCG, 0f, 0.2f);
            _finaleCG.interactable = false;
            _finaleCG.blocksRaycasts = false;
        }

        public void HideInstantly()
        {
            _finaleCG.alpha = 0;
            _finaleCG.interactable = false;
            _finaleCG.blocksRaycasts = false;
        }
    }
}
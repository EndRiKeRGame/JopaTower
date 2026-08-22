using Interfaces;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class DeathScreenView : MonoBehaviour, IShowable
    {
        [SerializeField]
        private CanvasGroup _deathCG;
        
        [SerializeField]
        private Button _restartButton;
        
        [SerializeField]
        private Button _exitButton;

        private UnityAction _onRestartButtonClicked;
        private UnityAction _onExitButtonClicked;

        public void Init(UnityAction onRestartClicked, UnityAction onExitClicked)
        {
            _onRestartButtonClicked = onRestartClicked;
            _onExitButtonClicked = onExitClicked;
            
            _restartButton.onClick.AddListener(_onRestartButtonClicked);
            _exitButton.onClick.AddListener(_onExitButtonClicked);
        }

        private void OnDestroy()
        {
            _restartButton.onClick.RemoveListener(_onRestartButtonClicked);
            _exitButton.onClick.RemoveListener(_onExitButtonClicked);
        }

        public void Show()
        {
            Tween.Alpha(_deathCG, 1f, 0.2f);
            _deathCG.interactable = true;
            _deathCG.blocksRaycasts = true;
        }

        public void Hide()
        {
            Tween.Alpha(_deathCG, 0f, 0.2f);
            _deathCG.interactable = false;
            _deathCG.blocksRaycasts = false;
        }

        public void HideInstantly()
        {
            _deathCG.alpha = 0;
            _deathCG.interactable = false;
            _deathCG.blocksRaycasts = false;
        }
    }
}
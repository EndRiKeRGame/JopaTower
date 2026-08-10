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
        
        [Header("Animation values")]
        [SerializeField]
        private float _animationDuration = 0.2f;

        private void Awake()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1;
            
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
using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace.Ui
{
    public class HoverButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Tween.Alpha(_canvasGroup, 1, 0.2f);
            Tween.Scale(_canvasGroup.transform, 1.1f, 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Tween.Alpha(_canvasGroup, 0, 0.2f);
            Tween.Scale(_canvasGroup.transform, 1f, 0.2f);
        }
    }
}
using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class ComicsView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _comicsCG;
        [SerializeField] private Button _skipButton;
        [SerializeField] private CanvasGroup _firstPageCG;
        [SerializeField] private CanvasGroup _secondPageCG;
        [SerializeField] private CanvasGroup _thirdPageCG;
        [SerializeField] private CanvasGroup _fourthPageCG;
        [SerializeField] private CanvasGroup _fivethPageCG;

        [Header("Animation Settings")]
        [SerializeField] private float _fadeDuration = 0.4f; // Длительность затухания/появления

        private CanvasGroup[] _pages;
        private int _currentPageIndex;
        private Action _onComplete;
        private bool _isTransitioning; // Флаг для блокировки спама кнопкой

        private void Awake()
        {
            // Собираем все страницы в массив для удобства
            _pages = new[]
            {
                _firstPageCG,
                _secondPageCG,
                _thirdPageCG,
                _fourthPageCG,
                _fivethPageCG
            };

            // Подписываемся на кнопку
            _skipButton.onClick.AddListener(ShowNextPage);

            // Изначально скрываем весь комикс и все страницы
            HideComicsInstant();
            HideAllPagesInstant();
        }

        /// <summary>
        /// Запускает показ комикса с первой страницы.
        /// </summary>
        /// <param name="onComplete">Событие, которое вызовется после окончания комикса.</param>
        public void ShowComics(Action onComplete = null)
        {
            _onComplete = onComplete;
            _currentPageIndex = 0;
            _isTransitioning = false;

            // Показываем весь контейнер комикса
            Tween.Alpha(_comicsCG, 1f, _fadeDuration);
            _comicsCG.interactable = true;
            _comicsCG.blocksRaycasts = true;

            // Показываем первую страницу
            ShowPageInstant(_currentPageIndex);
        
            // Предзагружаем вторую страницу позади
            if (_currentPageIndex + 1 < _pages.Length)
            {
                ShowPageBehind(_currentPageIndex + 1);
            }
        }

        private void ShowNextPage()
        {
            if (_isTransitioning) return; // Блокируем нажатия во время анимации

            _currentPageIndex++;

            if (_currentPageIndex < _pages.Length)
            {
                // Плавно скрываем текущую страницу
                _isTransitioning = true;
            
                var currentPage = _pages[_currentPageIndex - 1];
            
                Tween.Alpha(currentPage, 0f, _fadeDuration)
                    .OnComplete(() =>
                    {
                        currentPage.interactable = false;
                        currentPage.blocksRaycasts = false;
                        _isTransitioning = false;
                    });

                // Предзагружаем следующую страницу позади, если она есть
                if (_currentPageIndex + 1 < _pages.Length)
                {
                    ShowPageBehind(_currentPageIndex + 1);
                }
            }
            else
            {
                // Все страницы показаны – завершаем комикс
                HideComics();
                _onComplete?.Invoke();
                _onComplete = null;
            }
        }

        /// <summary>
        /// Мгновенно показывает страницу (для первой страницы).
        /// </summary>
        private void ShowPageInstant(int index)
        {
            var page = _pages[index];
            page.alpha = 1f;
            page.interactable = true;
            page.blocksRaycasts = true;
        }

        /// <summary>
        /// Показывает страницу позади текущей (для предзагрузки).
        /// </summary>
        private void ShowPageBehind(int index)
        {
            var page = _pages[index];
            page.alpha = 1f; // Страница уже видна, но позади
            page.interactable = true; // Интерактивность включаем сразу
            page.blocksRaycasts = true; // Но она будет перекрыта текущей страницей
        }

        private void HideAllPagesInstant()
        {
            foreach (var page in _pages)
            {
                page.alpha = 0f;
                page.interactable = false;
                page.blocksRaycasts = false;
            }
        }

        private void HideComics()
        {
            _isTransitioning = true;
        
            Tween.Alpha(_comicsCG, 0f, _fadeDuration)
                .OnComplete(() =>
                {
                    _comicsCG.interactable = false;
                    _comicsCG.blocksRaycasts = false;
                    _isTransitioning = false;
                });
        
            HideAllPagesInstant();
        }

        private void HideComicsInstant()
        {
            _comicsCG.alpha = 0f;
            _comicsCG.interactable = false;
            _comicsCG.blocksRaycasts = false;
        }
    }
}
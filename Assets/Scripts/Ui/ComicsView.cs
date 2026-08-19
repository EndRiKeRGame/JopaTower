using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class ComicsView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _comicsCG;
    [SerializeField] private Button _skipButton;
    [SerializeField] private CanvasGroup _firstPageCG;
    [SerializeField] private CanvasGroup _secondPageCG;
    [SerializeField] private CanvasGroup _thirdPageCG;
    [SerializeField] private CanvasGroup _fourthPageCG;
    [SerializeField] private CanvasGroup _fivethPageCG;

    private CanvasGroup[] _pages;
    private int _currentPageIndex;
    private Action _onComplete;

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
        HideAllPages();
    }

    /// <summary>
    /// Запускает показ комикса с первой страницы.
    /// </summary>
    /// <param name="onComplete">Событие, которое вызовется после окончания комикса.</param>
    public void ShowComics(Action onComplete = null)
    {
        _onComplete = onComplete;
        _currentPageIndex = 0;

        // Показываем весь контейнер комикса
        Tween.Alpha(_comicsCG, 1f, 0.2f);
        _comicsCG.interactable = true;
        _comicsCG.blocksRaycasts = true;

        // Показываем первую страницу
        ShowPage(_currentPageIndex);
    }

    private void ShowNextPage()
    {
        _currentPageIndex++;

        if (_currentPageIndex < _pages.Length)
        {
            // Показываем следующую страницу
            ShowPage(_currentPageIndex);
        }
        else
        {
            // Все страницы показаны – завершаем комикс
            HideComics();
            _onComplete?.Invoke();
            _onComplete = null;
        }
    }

    private void ShowPage(int index)
    {
        // Скрываем все страницы
        HideAllPages();

        // Показываем нужную страницу
        var page = _pages[index];
        page.alpha = 1f;
        page.interactable = true;
        page.blocksRaycasts = true;
    }

    private void HideAllPages()
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
        Tween.Alpha(_comicsCG, 0f, 0.2f);
        _comicsCG.interactable = false;
        _comicsCG.blocksRaycasts = false;
        HideAllPages();
    }

    private void HideComicsInstant()
    {
        _comicsCG.alpha = 0f;
        _comicsCG.interactable = false;
        _comicsCG.blocksRaycasts = false;
    }
}
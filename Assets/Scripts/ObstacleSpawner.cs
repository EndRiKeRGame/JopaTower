using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float _spawnInterval = 3f; // Интервал между циклами спавна
    [SerializeField] private int _crystalsPerSpawn = 1; // Сколько кристаллов создаётся за один цикл спавна
    [SerializeField] private float _minXSpawnPosition = -8f; // Минимальная позиция по X
    [SerializeField] private float _maxXSpawnPosition = 8f; // Максимальная позиция по X
    [SerializeField] private float _spawnHeightY = 10f; // Высота спавна кристалла
    [SerializeField] private float _fallDelay = 1f; // Задержка перед падением
    [SerializeField] private float _fallSpeed = 15f; // Скорость падения кристалла
    [SerializeField] private float _targetYPosition = -5f; // Конечная Y-позиция падения

    [Header("Lifetime After Landing")]
    [SerializeField] private float _lieDuration = 2f; // Сколько секунд кристалл лежит на месте после падения
    [SerializeField] private float _shrinkDuration = 0.3f; // За сколько секунд кристалл сжимается в 0 перед удалением

    [Header("Limits")]
    [SerializeField] private int _maxSimultaneousCrystals = 5; // Сколько кристаллов может одновременно существовать в сцене

    [Header("Warning Settings")]
    [SerializeField] private float _warningDuration = 1f; // Сколько секунд подсказка держится на экране — независимо от _fallDelay

    [Header("Camera Shake")]
    [SerializeField] private float _shakeStrength = 0.3f;
    [SerializeField] private float _shakeDuration = 0.15f;

    [Header("References")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameObject[] _crystalPrefab; // Префаб кристалла
    [SerializeField] private GameObject _warningPrefab; // Префаб предупреждения

    private Coroutine _spawnCoroutine;
    private bool _isSpawning;
    private readonly List<GameObject> _activeCrystals = new List<GameObject>(); // Список активных кристаллов (включая уже упавшие, но ещё не уничтоженные)
    private readonly List<GameObject> _activeWarnings = new List<GameObject>(); // Список активных подсказок
    
    public void Init(float startPos, float endPos)
    {
        _spawnHeightY = startPos;
        _targetYPosition = endPos;
    }
    
    /// <summary>
    /// Запускает спавн препятствий.
    /// </summary>
    public void StartSpawning()
    {
        if (_isSpawning) return;
            
        _isSpawning = true;
        _spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Останавливает спавн препятствий и удаляет все кристаллы и подсказки.
    /// </summary>
    public void StopSpawning()
    {
        if (!_isSpawning) return;
            
        _isSpawning = false;

        // Кристаллы теперь спавнятся параллельно (каждый — своя корутина),
        // поэтому глушим разом все корутины компонента, а не только SpawnLoop
        StopAllCoroutines();
        _spawnCoroutine = null;
            
        ClearAllSpawned();
    }

    private IEnumerator SpawnLoop()
    {
        while (_isSpawning)
        {
            // Ждём интервал между циклами спавна
            yield return new WaitForSeconds(_spawnInterval);

            for (int i = 0; i < _crystalsPerSpawn; i++)
            {
                // Если лимит уже выбран — в этом цикле больше не создаём
                if (_activeCrystals.Count >= _maxSimultaneousCrystals)
                    break;

                // Каждый кристалл живёт своей собственной корутиной,
                // поэтому несколько штук падают и лежат на сцене одновременно
                StartCoroutine(SpawnCrystalRoutine());
            }
        }
    }

    private IEnumerator SpawnCrystalRoutine()
    {
        // Генерируем случайную позицию по X
        float randomX = Random.Range(_minXSpawnPosition, _maxXSpawnPosition);
        Vector3 spawnPosition = new Vector3(randomX, _spawnHeightY, 0f);

        // Тряска камеры
        ShakeCamera();

        // Создаём кристалл наверху
        var prefab = _crystalPrefab[Random.Range(0, _crystalPrefab.Length)];
        GameObject crystal = Instantiate(prefab, spawnPosition, Quaternion.identity);
            
        // Добавляем кристалл в список активных
        _activeCrystals.Add(crystal);
            
        // Создаём предупреждение на уровне камеры
        Vector3 warningPosition = new Vector3(randomX, _mainCamera.transform.position.y, 0f);
        GameObject warning = Instantiate(_warningPrefab, warningPosition, Quaternion.identity);
        _activeWarnings.Add(warning);

        // Подсказка живёт свой собственный срок — независимо от _fallDelay
        StartCoroutine(DestroyWarningAfterDelay(warning, _warningDuration));

        // Ждём перед падением
        yield return new WaitForSeconds(_fallDelay);

        // Запускаем падение кристалла
        yield return MakeCrystalFall(crystal);

        // Кристалл долежал на месте, затем сжимается и уничтожается
        yield return LieThenShrinkAndDestroy(crystal);
    }

    private IEnumerator DestroyWarningAfterDelay(GameObject warning, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (warning != null)
        {
            _activeWarnings.Remove(warning);
            Destroy(warning);
        }
    }

    private IEnumerator MakeCrystalFall(GameObject crystal)
    {
        if (crystal == null) yield break;

        float fallDistance = _spawnHeightY - _targetYPosition;
        float fallTime = fallDistance / _fallSpeed;
        float elapsedTime = 0f;
            
        Vector3 startPosition = crystal.transform.position;
        Vector3 endPosition = new Vector3(startPosition.x, _targetYPosition, startPosition.z);

        while (elapsedTime < fallTime)
        {
            if (crystal == null) yield break;
                
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fallTime;
            crystal.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        // Кристалл достиг целевой позиции
        if (crystal != null)
        {
            crystal.transform.position = endPosition;
        }
    }

    /// <summary>
    /// Кристалл лежит на месте _lieDuration секунд, затем плавно сжимается
    /// в 0 за _shrinkDuration секунд и уничтожается.
    /// </summary>
    private IEnumerator LieThenShrinkAndDestroy(GameObject crystal)
    {
        if (crystal == null)
        {
            _activeCrystals.Remove(crystal);
            yield break;
        }

        yield return new WaitForSeconds(_lieDuration);

        if (crystal == null)
        {
            _activeCrystals.Remove(crystal);
            yield break;
        }

        // Плавное уменьшение через PrimeTween перед удалением со сцены
        Tween shrinkTween = Tween.Scale(crystal.transform, Vector3.zero, _shrinkDuration);
        yield return shrinkTween.ToYieldInstruction();

        _activeCrystals.Remove(crystal);

        if (crystal != null)
        {
            Destroy(crystal);
        }
    }

    /// <summary>
    /// Удаляет все активные кристаллы и все ещё не пропавшие подсказки.
    /// </summary>
    private void ClearAllSpawned()
    {
        foreach (var crystal in _activeCrystals)
        {
            if (crystal != null)
            {
                Destroy(crystal);
            }
        }
        _activeCrystals.Clear();

        foreach (var warning in _activeWarnings)
        {
            if (warning != null)
            {
                Destroy(warning);
            }
        }
        _activeWarnings.Clear();
    }

    private void ShakeCamera()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        if (_mainCamera != null)
        {
            // Используем PrimeTween для тряски
            Tween.ShakeLocalPosition(_mainCamera.transform, 
                new Vector3(_shakeStrength, _shakeStrength, 0f), 
                _shakeDuration);
        }
    }

    private void OnDestroy()
    {
        // Очищаем кристаллы и подсказки при уничтожении объекта
        ClearAllSpawned();
    }
}
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private float _spawnInterval = 3f; // Интервал между спавнами
        [SerializeField] private float _minXSpawnPosition = -8f; // Минимальная позиция по X
        [SerializeField] private float _maxXSpawnPosition = 8f; // Максимальная позиция по X
        [SerializeField] private float _spawnHeightY = 10f; // Высота спавна кристалла
        [SerializeField] private float _fallDelay = 1f; // Задержка перед падением
        [SerializeField] private float _fallSpeed = 15f; // Скорость падения кристалла
        [SerializeField] private float _targetYPosition = -5f; // Конечная Y-позиция падения

        [Header("Camera Shake")]
        [SerializeField] private float _shakeStrength = 0.3f;
        [SerializeField] private float _shakeDuration = 0.15f;

        [Header("References")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private GameObject[] _crystalPrefab; // Префаб кристалла
        [SerializeField] private GameObject _warningPrefab; // Префаб предупреждения

        private Coroutine _spawnCoroutine;
        private bool _isSpawning;
        private List<GameObject> _activeCrystals = new List<GameObject>(); // Список активных кристаллов

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
        /// Останавливает спавн препятствий и удаляет все кристаллы.
        /// </summary>
        public void StopSpawning()
        {
            if (!_isSpawning) return;
            
            _isSpawning = false;
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
            
            // Удаляем все активные кристаллы
            ClearAllCrystals();
        }

        private IEnumerator SpawnLoop()
        {
            while (_isSpawning)
            {
                // Ждём интервал между спавнами
                yield return new WaitForSeconds(_spawnInterval);
                
                // Спавним кристалл
                yield return SpawnCrystalRoutine();
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

            // Ждём перед падением
            yield return new WaitForSeconds(_fallDelay);

            // Убираем предупреждение
            if (warning != null)
            {
                Destroy(warning);
            }

            // Запускаем падение кристалла
            yield return MakeCrystalFall(crystal);
            
            // Удаляем кристалл из списка после завершения падения
            if (crystal != null)
            {
                _activeCrystals.Remove(crystal);
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
                // Здесь можно добавить дополнительную логику (например, эффект приземления)
            }
        }

        /// <summary>
        /// Удаляет все активные кристаллы.
        /// </summary>
        private void ClearAllCrystals()
        {
            foreach (var crystal in _activeCrystals)
            {
                if (crystal != null)
                {
                    Destroy(crystal);
                }
            }
            _activeCrystals.Clear();
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
            // Очищаем кристаллы при уничтожении объекта
            ClearAllCrystals();
        }
    }
}
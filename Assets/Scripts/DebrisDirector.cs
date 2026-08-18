using UnityEngine;

/// <summary>
/// Режиссёр опасностей Акта 2.
/// Раз в интервал спавнит случайный осколок из массива над игроком и трясёт камеру.
/// Интервал уменьшается к концу акта.
/// РЕАЛЬНАЯ высота секции и границы акта определяются по башне автоматически;
/// определение ретраится каждый кадр, пока башня не сгенерирована.
/// </summary>
public class DebrisDirector : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform _player;
    [Tooltip("Объект, в котором лежат сгенерированные секции (ActTowerGenerator). Можно не заполнять — найдём сами.")]
    [SerializeField] private Transform _towerRoot;
    [SerializeField] private CameraShake _shake;
    [SerializeField] private GameObject[] _debrisPrefabs; // 4 типа осколков

    [Header("Геометрия акта")]
    [Tooltip("Запасная высота, если не удастся снять её с башни.")]
    [SerializeField] private float _sectionHeight = 6f;
    [Tooltip("Подстрока, по которой узнаём секции акта 2 в именах (например, Act2).")]
    [SerializeField] private string _actMarker = "Act2";

    [Header("Ручные границы (если автоопределение не нашло маркер)")]
    [SerializeField] private int _manualStartRoom = -1;
    [SerializeField] private int _manualEndRoom = -1;

    [Header("Спавн")]
    [SerializeField] private float _spawnHalfWidth = 6f;    // разброс по X вокруг игрока
    [SerializeField] private float _spawnAbovePlayer = 12f; // высота над игроком

    [Header("Темп: от редкого к частому")]
    [SerializeField] private float _intervalAtActStart = 6f;
    [SerializeField] private float _intervalAtActEnd = 2f;

    [Header("Отладка")]
    [SerializeField] private bool _debugLog = true;
    [SerializeField] private float _silenceLogInterval = 2f; // как часто печатать "почему молчим"

    private int _startRoom;
    private int _endRoom;
    private bool _boundsDetected;
    private bool _heightDetected;
    private float _timer = 2f;
    private float _silenceLogTimer;
    private bool _warnedPlayer;
    private bool _warnedPrefabs;

    void Update()
    {
        // --- Проверки с одноразовыми предупреждениями ---
        if (_player == null)
        {
            if (!_warnedPlayer)
            {
                Debug.LogError("DebrisDirector: не назначен Player! Спавн невозможен.");
                _warnedPlayer = true;
            }
            return;
        }

        if (_debrisPrefabs == null || _debrisPrefabs.Length == 0)
        {
            if (!_warnedPrefabs)
            {
                Debug.LogError("DebrisDirector: массив debrisPrefabs пустой! Спавн невозможен.");
                _warnedPrefabs = true;
            }
            return;
        }

        // --- Высота и границы: ретрай каждый кадр, пока башня не сгенерирована ---
        if (!_boundsDetected)
        {
            TryDetectBounds();
            if (!_boundsDetected) return;
        }

        // --- В какой комнате игрок ---
        int room = Mathf.FloorToInt(_player.position.y / _sectionHeight);

        // Вне своего акта режиссёр молчит, но в дебаге объясняет почему
        if (room < _startRoom || room > _endRoom)
        {
            if (_debugLog)
            {
                _silenceLogTimer -= Time.deltaTime;
                if (_silenceLogTimer <= 0f)
                {
                    _silenceLogTimer = _silenceLogInterval;
                    Debug.Log($"DebrisDirector: тишина. playerY={_player.position.y:F1} room={room} bounds={_startRoom}..{_endRoom} height={_sectionHeight:F2}");
                }
            }
            return;
        }

        // Прогресс внутри акта 0..1 — чем дальше, тем чаще
        float progress = (room - _startRoom) / Mathf.Max(1, _endRoom - _startRoom);
        float interval = Mathf.Lerp(_intervalAtActStart, _intervalAtActEnd, progress);

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = interval;
            SpawnDebris(room);
            if (_shake != null) _shake.Shake(0.4f, 0.15f);
        }
    }

    /// <summary>
    /// Определяет реальную высоту секции и границы акта по башне.
    /// НЕ блокируется, пока в башне реально не появились секции:
    /// генерация происходит позже Start этого скрипта.
    /// </summary>
    bool TryDetectBounds()
    {
        if (_towerRoot == null)
        {
            ActTowerGenerator gen = FindObjectOfType<ActTowerGenerator>();
            if (gen != null) _towerRoot = gen.transform;
        }

        if (_towerRoot == null) return false;

        bool anySection = false;
        int min = int.MaxValue;
        int max = int.MinValue;

        foreach (Transform child in _towerRoot)
        {
            string n = child.name;
            if (!n.StartsWith("Section_")) continue; // стены и прочее пропускаем

            // Достаём номер: "Section_014_..." → "014" → 14
            string after = n.Substring("Section_".Length);
            int underscoreIndex = after.IndexOf('_');
            string numPart = underscoreIndex >= 0 ? after.Substring(0, underscoreIndex) : after;
            if (!int.TryParse(numPart, out int idx)) continue;

            anySection = true;

            // Реальная высота: секция с номером idx стоит на Y = idx * высота
            if (!_heightDetected && idx > 0)
            {
                _sectionHeight = child.position.y / idx;
                _heightDetected = true;
                if (_debugLog)
                    Debug.Log($"DebrisDirector: реальная высота секции = {_sectionHeight:F2}");
            }

            if (!n.Contains(_actMarker)) continue; // не наш акт

            if (idx < min) min = idx;
            if (idx > max) max = idx;
        }

        // Башня есть, но секций в ней ещё нет — генерация не прошла, ждём
        if (!anySection) return false;

        // Автоопределение по маркеру
        if (min <= max)
        {
            _startRoom = min;
            _endRoom = max;
            _boundsDetected = true;
            if (_debugLog)
                Debug.Log($"DebrisDirector: границы акта определены автоматически: комнаты {_startRoom}..{_endRoom}");
            return true;
        }

        // Маркер не найден — берём ручные числа
        if (_manualStartRoom >= 0 && _manualEndRoom >= _manualStartRoom)
        {
            _startRoom = _manualStartRoom;
            _endRoom = _manualEndRoom;
            _boundsDetected = true;
            if (_debugLog)
                Debug.Log($"DebrisDirector: использую ручные границы: комнаты {_startRoom}..{_endRoom}");
            return true;
        }

        return false;
    }

    void SpawnDebris(int room)
    {
        // Границы акта в мировых координатах
        float actTopY = (_endRoom + 1) * _sectionHeight;
        float actBottomY = _startRoom * _sectionHeight;

        // Над игроком, но не выше потолка акта — не лезем в следующий акт
        float y = Mathf.Min(_player.position.y + _spawnAbovePlayer, actTopY - 0.5f);
        float x = Random.Range(-_spawnHalfWidth, _spawnHalfWidth);

        // Случайный тип осколка — тот же стиль, что sectionPrefabs в TowerSectionGenerator
        GameObject prefab = _debrisPrefabs[Random.Range(0, _debrisPrefabs.Length)];

        // Родительствуем к режиссёру, чтобы в Hierarchy был порядок
        GameObject debris = Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity, transform);

        // Сообщаем осколку игрока и нижнюю границу акта
        FallingDebris fd = debris.GetComponent<FallingDebris>();
        if (fd != null) fd.Init(_player, actBottomY - 1f);

        if (_debugLog)
            Debug.Log($"DebrisDirector: заспавнен осколок в комнате {room} на ({x:F1}; {y:F1})");
    }

    /// <summary>
    /// Вызвать при полном рестарте башни (DestroyTower + GenerateFullTower),
    /// чтобы старые осколки не остались висеть от прошлой попытки.
    /// </summary>
    public void ClearAllDebris()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
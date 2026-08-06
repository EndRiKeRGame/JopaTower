using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Генератор башни, разбитой на акты.
///
/// Логика актов:
/// Element 0 в массиве acts = Act 1
/// Element 1 в массиве acts = Act 2
/// Element 2 в массиве acts = Act 3
///
/// Каждый акт состоит из 10 секций:
/// индекс 0    - переходная секция входа в акт, фиксированная
/// индексы 1-4 - случайные секции из пула A
/// индексы 5-8 - случайные секции из пула B
/// индекс 9    - переходная секция выхода из акта, фиксированная
///
/// Важно:
/// Папки Assets/Prefabs/Act1, Act2, Act3 используются только для порядка.
/// Код не ищет префабы по папкам автоматически.
/// Все префабы назначаются вручную в инспекторе.
/// </summary>
public class ActTowerGenerator : MonoBehaviour
{
    // ==========================================================
    // Описание одного акта
    // ==========================================================

    // [System.Serializable] позволяет Unity показывать этот класс в инспекторе.
    // Без этого атрибута поля ActConfig было бы неудобно или невозможно
    // нормально редактировать в Inspector.
    [System.Serializable]
    public class ActConfig
    {
        [Header("Секция 1 (переход в акт)")]
        public GameObject transitionStart;

        [Header("Секции 2-5 (пул A)")]
        public GameObject[] poolA;

        [Header("Секции 6-9 (пул B)")]
        public GameObject[] poolB;

        [Header("Секция 10 (переход из акта)")]
        public GameObject transitionEnd;
    }

    // ==========================================================
    // Настройки в инспекторе
    // ==========================================================

    [Header("Акты: Element 0 = Act1, Element 1 = Act2, Element 2 = Act3")]
    public ActConfig[] acts = new ActConfig[3];

    [Header("Игрок и параметры генерации")]
    public Transform player;

    [Tooltip("Высота одной секции. Все префабы секций должны быть такой же высоты.")]
    public float sectionHeight = 6f;

    [Tooltip("Сколько секций содержит один акт. Для текущей схемы это 10.")]
    public int sectionsPerAct = 10;

    [Tooltip("Сколько секций держать заранее выше игрока.")]
    public int sectionsAhead = 4;

    [Tooltip("Сколько секций оставлять позади игрока перед удалением.")]
    public int sectionsBehindToKeep = 2;

    [Header("Поведение после последнего акта")]
    [Tooltip("Если true, после Act3 генератор снова вернётся к Act1. Если false, генерация остановится.")]
    public bool loopAfterLastAct = true;

    [Header("Отладка")]
    [Tooltip("Если true, в консоли будут сообщения о спавне секций.")]
    public bool debugLog = true;

    // ==========================================================
    // Внутреннее состояние генератора
    // ==========================================================

    // [SerializeField] делает приватные поля видимыми в инспекторе.
    // Это удобно во время тестирования.
    [Header("Состояние генератора (удобно для теста)")]
    [SerializeField] private List<GameObject> activeSections = new List<GameObject>();
    [SerializeField] private int nextSpawnIndex;
    [SerializeField] private float nextSpawnY;
    [SerializeField] private int lastPlayerSectionIndex = -1;

    // ==========================================================
    // Start вызывается один раз при старте
    // ==========================================================
    void Start()
    {
        // Сбрасываем внутреннее состояние
        nextSpawnY = 0f;
        nextSpawnIndex = 0;
        lastPlayerSectionIndex = -1;

        // Проверка: назначен ли игрок
        if (player == null)
        {
            Debug.LogError("ActTowerGenerator: не назначен Player. Перетащи объект игрока в поле Player.");
            enabled = false;
            return;
        }

        // Проверка: есть ли акты
        if (acts == null || acts.Length == 0)
        {
            Debug.LogError("ActTowerGenerator: массив Acts пустой. Добавь хотя бы один акт в инспекторе.");
            enabled = false;
            return;
        }

        // Проверка: высота секции должна быть положительной
        if (sectionHeight <= 0f)
        {
            Debug.LogError("ActTowerGenerator: sectionHeight должна быть больше 0.");
            enabled = false;
            return;
        }

        // Проверка: количество секций в акте должно быть положительным
        if (sectionsPerAct <= 0)
        {
            Debug.LogError("ActTowerGenerator: sectionsPerAct должна быть больше 0.");
            enabled = false;
            return;
        }

        // Спавним стартовый запас секций, чтобы игрок не появился в пустоте
        for (int i = 0; i < sectionsAhead; i++)
        {
            if (!CanSpawnIndex(nextSpawnIndex))
            {
                break;
            }

            SpawnNextSection();
        }
    }

    // ==========================================================
    // Update вызывается каждый кадр
    // ==========================================================
    void Update()
    {
        if (player == null)
        {
            return;
        }

        // Определяем, в какой секции сейчас находится игрок.
        //
        // Пример:
        // player.position.y = 13.7
        // sectionHeight = 6
        // 13.7 / 6 = 2.28
        // Mathf.FloorToInt(2.28) = 2
        int playerSectionIndex = Mathf.FloorToInt(player.position.y / sectionHeight);

        // Если игрок всё ещё в той же секции, что и в прошлом кадре,
        // не выполняем лишнюю работу.
        if (playerSectionIndex == lastPlayerSectionIndex)
        {
            return;
        }

        lastPlayerSectionIndex = playerSectionIndex;

        // До какого глобального номера секции нужно достроить башню
        int targetIndex = playerSectionIndex + sectionsAhead;

        // Спавним секции, пока не достигнем нужного верха
        while (nextSpawnIndex <= targetIndex)
        {
            if (!CanSpawnIndex(nextSpawnIndex))
            {
                break;
            }

            SpawnNextSection();
        }

        // Удаляем секции, которые остались далеко внизу
        CleanupOldSections(playerSectionIndex);
    }

    // ==========================================================
    // Проверка, можно ли спавнить секцию с таким глобальным индексом
    // ==========================================================
    bool CanSpawnIndex(int globalIndex)
    {
        if (acts == null || acts.Length == 0)
        {
            return false;
        }

        // Если зацикливание включено, спавнить можно всегда
        if (loopAfterLastAct)
        {
            return true;
        }

        // Если зацикливание выключено, ограничиваем общее число секций
        int totalSections = acts.Length * sectionsPerAct;

        return globalIndex < totalSections;
    }

    // ==========================================================
    // Спавн одной секции
    // ==========================================================
    void SpawnNextSection()
    {
        // Выбираем префаб для текущего глобального номера секции
        GameObject prefab = ChoosePrefabForIndex(nextSpawnIndex);

        if (prefab != null)
        {
            // Создаём копию префаба на нужной высоте
            GameObject section = Instantiate(
                prefab,
                new Vector3(0f, nextSpawnY, 0f),
                Quaternion.identity,
                transform
            );

            // Делает имя объекта понятным в Hierarchy.
            // Например:
            // Section_000_Act1_TransitionStart
            section.name = $"Section_{nextSpawnIndex:000}_{prefab.name}";

            activeSections.Add(section);
        }
        else
        {
            Debug.LogWarning($"ActTowerGenerator: для секции {nextSpawnIndex} не назначен префаб. Пропускаем.");
        }

        // Переходим к следующей секции
        nextSpawnY += sectionHeight;
        nextSpawnIndex++;
    }

    // ==========================================================
    // Выбор префаба по глобальному номеру секции
    // ==========================================================
    GameObject ChoosePrefabForIndex(int globalIndex)
    {
        if (acts == null || acts.Length == 0)
        {
            return null;
        }

        // Определяем номер акта.
        //
        // Пример:
        // globalIndex = 27
        // sectionsPerAct = 10
        // 27 / 10 = 2
        // Значит это акт с индексом 2, то есть Act3, если считать по-человечески.
        int actIndex = globalIndex / sectionsPerAct;

        if (loopAfterLastAct)
        {
            // Если актов 3, индексы будут повторяться так:
            // 0, 1, 2, 0, 1, 2, 0, 1, 2...
            actIndex %= acts.Length;
        }
        else
        {
            // Если зацикливание выключено и акты закончились,
            // больше ничего не выбираем.
            if (actIndex >= acts.Length)
            {
                return null;
            }
        }

        // Определяем позицию секции внутри акта.
        //
        // Пример:
        // globalIndex = 27
        // sectionsPerAct = 10
        // 27 % 10 = 7
        // Значит внутри акта это секция с индексом 7.
        int indexInAct = globalIndex % sectionsPerAct;

        ActConfig act = acts[actIndex];

        if (act == null)
        {
            Debug.LogWarning($"ActTowerGenerator: акт с индексом {actIndex} пустой.");
            return null;
        }

        GameObject result;

        // Выбираем тип секции по её индексу внутри акта
        switch (indexInAct)
        {
            // Человеческая секция 1
            case 0:
                result = act.transitionStart;
                break;

            // Человеческие секции 2, 3, 4, 5
            case 1:
            case 2:
            case 3:
            case 4:
                result = GetRandomFromPool(act.poolA);
                break;

            // Человеческие секции 6, 7, 8, 9
            case 5:
            case 6:
            case 7:
            case 8:
                result = GetRandomFromPool(act.poolB);
                break;

            // Человеческая секция 10
            case 9:
                result = act.transitionEnd;
                break;

            default:
                Debug.LogWarning($"ActTowerGenerator: indexInAct = {indexInAct}, sectionsPerAct = {sectionsPerAct}. Проверь настройки.");
                result = GetRandomFromPool(act.poolA);
                break;
        }

        // Отладочный вывод в консоль.
        // actIndex + 1 используется только для удобства,
        // потому что человек привык считать акты с 1, а код с 0.
        if (debugLog)
        {
            string prefabName = result == null ? "NULL" : result.name;

            Debug.Log($"Spawn section {globalIndex}: Act {actIndex + 1}/{acts.Length}, indexInAct {indexInAct}, prefab {prefabName}");
        }

        return result;
    }

    // ==========================================================
    // Случайный выбор префаба из пула
    // ==========================================================
    GameObject GetRandomFromPool(GameObject[] pool)
    {
        if (pool == null || pool.Length == 0)
        {
            return null;
        }

        return pool[Random.Range(0, pool.Length)];
    }

    // ==========================================================
    // Удаление старых секций, которые остались далеко внизу
    // ==========================================================
    void CleanupOldSections(int playerSectionIndex)
    {
        // Идём с конца списка, чтобы безопасно удалять элементы
        for (int i = activeSections.Count - 1; i >= 0; i--)
        {
            GameObject section = activeSections[i];

            // Если объект уже был уничтожен, просто убираем его из списка
            if (section == null)
            {
                activeSections.RemoveAt(i);
                continue;
            }

            // Определяем индекс этой секции по её высоте
            int sectionIndex = Mathf.FloorToInt(section.transform.position.y / sectionHeight);

            // Если секция осталась далеко позади игрока, удаляем её
            if (sectionIndex < playerSectionIndex - sectionsBehindToKeep)
            {
                Destroy(section);
                activeSections.RemoveAt(i);
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Генерирует башню из префабов-отделений (каждое отделение — набор платформ с GrabPoint'ами)
/// по мере подъёма игрока, и удаляет старые отделения снизу, чтобы не копить объекты.
///
/// Требование: все префабы в sectionPrefabs должны иметь одинаковую высоту sectionHeight
/// и "нулевую" точку (pivot) внизу секции, чтобы они стыковались друг с другом без разрывов.
/// </summary>
public class TowerSectionGenerator : MonoBehaviour
{
    [Header("Заготовки отделений башни")]
    public GameObject[] sectionPrefabs;

    [Header("Игрок и параметры генерации")]
    public Transform player;
    public float sectionHeight = 6f;
    public int sectionsAhead = 4;         // сколько секций держим сгенерированными выше игрока
    public int sectionsBehindToKeep = 2;  // сколько секций оставляем позади игрока про запас

    private readonly List<GameObject> _activeSections = new List<GameObject>();
    private float _nextSpawnY;
    private int _lastPlayerSectionIndex = -1;

    void Start()
    {
        _nextSpawnY = 0f;
        for (int i = 0; i < sectionsAhead; i++)
        {
            SpawnNextSection();
        }
    }

    void Update()
    {
        int playerSectionIndex = Mathf.FloorToInt(player.position.y / sectionHeight);

        if (playerSectionIndex == _lastPlayerSectionIndex) return;
        _lastPlayerSectionIndex = playerSectionIndex;

        float targetTopY = (playerSectionIndex + sectionsAhead) * sectionHeight;
        while (_nextSpawnY < targetTopY + 0.01f)
        {
            SpawnNextSection();
        }

        CleanupOldSections(playerSectionIndex);
    }

    void SpawnNextSection()
    {
        GameObject prefab = sectionPrefabs[Random.Range(0, sectionPrefabs.Length)];
        GameObject section = Instantiate(prefab, new Vector3(0f, _nextSpawnY, 0f), Quaternion.identity, transform);
        _activeSections.Add(section);
        _nextSpawnY += sectionHeight;
    }

    void CleanupOldSections(int playerSectionIndex)
    {
        for (int i = _activeSections.Count - 1; i >= 0; i--)
        {
            GameObject section = _activeSections[i];
            if (section == null) { _activeSections.RemoveAt(i); continue; }

            int sectionIndex = Mathf.FloorToInt(section.transform.position.y / sectionHeight);
            if (sectionIndex < playerSectionIndex - sectionsBehindToKeep)
            {
                Destroy(section);
                _activeSections.RemoveAt(i);
            }
        }
    }
}

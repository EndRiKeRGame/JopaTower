using DefaultNamespace;
using UnityEngine;

public class ActTowerGenerator : MonoBehaviour
{
    [SerializeField]
    private TowerConfig _towerConfig;

    [Tooltip("Высота одной секции. Все префабы секций должны быть такой же высоты.")]
    [SerializeField]
    private float _sectionHeight = 6f;

    [Tooltip("Сколько секций содержит один акт. Для текущей схемы это 10.")]
    [SerializeField]
    private int _sectionsPerAct = 10;
    
    private int nextSpawnIndex;
    private float nextSpawnY;
    private int lastPlayerSectionIndex = -1;
    
    public void GenerateFullTower()
    {
        // Сбрасываем внутреннее состояние
        nextSpawnY = 0f;
        nextSpawnIndex = 0;
        lastPlayerSectionIndex = -1;

        var act1 = _towerConfig.Act1.GetSectionOrder(_sectionsPerAct);
        var act2 = _towerConfig.Act2.GetSectionOrder(_sectionsPerAct);
        var act3 = _towerConfig.Act3.GetSectionOrder(_sectionsPerAct);
        
        foreach (var section in act1)
            SpawnNextSection(section);
        
        foreach (var section in act2)
            SpawnNextSection(section);
        
        foreach (var section in act3)
            SpawnNextSection(section);
    }
    
    public void SpawnNextSection(GameObject spawnSection)
    {
        // Создаём копию префаба на нужной высоте
        GameObject section = Instantiate(
            spawnSection,
            new Vector3(0f, nextSpawnY, 0f),
            Quaternion.identity,
            transform
        );
        
        section.name = $"Section_{nextSpawnIndex:000}_{spawnSection.name}";

        // Переходим к следующей секции
        nextSpawnY += _sectionHeight;
        nextSpawnIndex++;
    }
}
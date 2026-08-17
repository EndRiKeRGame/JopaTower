using DefaultNamespace;
using UnityEngine;

public class ActTowerGenerator : MonoBehaviour
{
    private int _nextSpawnIndex;
    private float _nextSpawnY;
    private int _lastPlayerSectionIndex = -1;
    private float _sectionHeight;
    
    public void GenerateFullTower(TowerConfig config, float sectionHeight, int sectionsPerAct)
    {
        // Сбрасываем внутреннее состояние
        _nextSpawnY = 0f;
        _nextSpawnIndex = 0;
        _lastPlayerSectionIndex = -1;
        _sectionHeight = sectionHeight;

        var prologue = config.Prologue.GetSectionOrderForPrologue();
        var act1 = config.Act1.GetSectionOrder(sectionsPerAct);
        var act2 = config.Act2.GetSectionOrder(sectionsPerAct);
        var act3 = config.Act3.GetSectionOrder(sectionsPerAct);
        
        foreach (var section in prologue)
            SpawnNextSection(section);
        
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
            new Vector3(0f, _nextSpawnY, 0f),
            Quaternion.identity,
            transform
        );
        
        section.name = $"Section_{_nextSpawnIndex:000}_{spawnSection.name}";

        // Переходим к следующей секции
        _nextSpawnY += _sectionHeight;
        _nextSpawnIndex++;
    }
}